using Leap;
using Leap.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public float intervalBetweenHandTrackingData = 0.7f;

    public SessionData sessionData;

    //SessionTime
    private Timer SessionTimer;
    private Coroutine sessionTimerCourtine;

    //GroupTime
    private Timer GroupTimer;
    private Coroutine GroupTimerCourtine;

    //SubTaskTimer
    private Timer TaskTimer;
    private Coroutine TaskTimerCourtine;


    //LeftAndRightHand Coroutine
    private Coroutine leftHandCoroutine;
    private Coroutine rightHandCoroutine;
    private List<HandAndFingureMovement> rightHandData;
    private List<HandAndFingureMovement> leftHandData;

    public List<GroupPlayedState> groupPlayedStates;

    SaveManager SaveManager;
    HandTrackingData handTrackingDataRecorder;

    public bool isRecording = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SaveManager = new SaveManager();
        handTrackingDataRecorder = GetComponent<HandTrackingData>();
        TaskTimer = GetComponent<Timer>();
    }

    public string GetData(string sessionid, string participantid)
    {
        var sessionData = SaveManager.LoadSessionData(sessionid, participantid);
        if (sessionData == null)
        {
            return "No Data Found";
        }
        return SaveManager.GetFormattedDataString(sessionData);

    }
    internal void OnCancelClicked()
    {
        Reset();
    }

    private void SaveData()
    {
        SaveManager.SaveSessionData(sessionData);
    }

    internal void OnSessionStart()
    {
        SessionTimer = gameObject.AddComponent<Timer>();
        sessionTimerCourtine = SessionTimer.StartTimer();
    }
    public void StopGroupTimer()
    {
        float elapsedTime1 = SessionTimer.StopTimer(sessionTimerCourtine);
        sessionData.TotalSessionTime = elapsedTime1.ToString();
    }
  
    internal void onWithDrawFromSession()
    {
        StopGroupTimer();
        sessionData.sessionState = State.withdraw;
        SaveData();
        Reset();
    }
    internal void onCompleteSession()
    {
        StopGroupTimer();
        sessionData.sessionState = State.Completed;
        SaveData();
        Reset();
    }
    public void Reset()
    {
        foreach (var group in groupPlayedStates)
        {
            group.isPlayed = false;
        }
        foreach (var gp in sessionData.groupData)
        {
            gp.State = State.NotPlayed;
            gp.TotalTime = "";
            foreach (var task in gp.tasks)
            {
                task.subTasks.Clear();
                task.CompletedTimes = "";
            }
        }
    }

    internal void OnAllGroupPlayed()
    {
        sessionData.sessionState = State.Completed;
        StopGroupTimer();
        SaveData();
        Reset();
        SceneManager.LoadScene(0);
    }

    internal void OnGroupStarted(int groupNo)
    {
        GroupTimer = gameObject.AddComponent<Timer>();
        GroupTimerCourtine = GroupTimer.StartTimer();
    }

    internal void OnStartButtonPressedOnTask(int groupNo, TaskList currentTask)
    {
        if(leftHandCoroutine != null)
        StopCoroutine(leftHandCoroutine);
        if(rightHandCoroutine != null)
        StopCoroutine(rightHandCoroutine);

        leftHandCoroutine = null;
        rightHandCoroutine = null;

        rightHandData = new List<HandAndFingureMovement>();
        leftHandData = new List<HandAndFingureMovement>();


        leftHandCoroutine = StartCoroutine(startHandRecording(Chirality.Left ,groupNo,currentTask));
        rightHandCoroutine = StartCoroutine(startHandRecording(Chirality.Right, groupNo, currentTask));
        isRecording = true;
        Events.OnRecordingStart?.Invoke();
    }

    internal void OnRepeatTaskRecording(int groupNo, TaskList currentTask)
    {
        // Stop the existing coroutines
        if (leftHandCoroutine != null)
        {
            StopCoroutine(leftHandCoroutine);
        }
        if (rightHandCoroutine != null)
        {
            StopCoroutine(rightHandCoroutine);
        }

        leftHandCoroutine = null;
        rightHandCoroutine = null;

        rightHandData = new List<HandAndFingureMovement>();
        leftHandData = new List<HandAndFingureMovement>();

        leftHandCoroutine = StartCoroutine(startHandRecording(Chirality.Left, groupNo, currentTask));
        rightHandCoroutine = StartCoroutine(startHandRecording(Chirality.Right, groupNo, currentTask));

        currentTask.playedTime = 0;
        currentTask.taskObj.SetActive(true);
        currentTask.isCompleted = false;
        Events.OnRecordingRepeat?.Invoke();
        Debug.Log($"Task {currentTask.taskNo} in Group {groupNo} has been reset and recording restarted.");
    }


    internal void OnEndTaskRecording(int groupNo, TaskList currentTask)
    {
        StopCoroutine(leftHandCoroutine);
        StopCoroutine(rightHandCoroutine);
        leftHandCoroutine = null;
        rightHandCoroutine = null;

        var taskno = FindAnyObjectByType<GroupController>().group.tasks.IndexOf(currentTask);
        sessionData.groupData[groupNo - 1].tasks[taskno].subTasks[currentTask.playedTime-1].LefthandAndFingureMovement = leftHandData;
        sessionData.groupData[groupNo - 1].tasks[taskno].subTasks[currentTask.playedTime-1].RighthandAndFingureMovement = rightHandData;
        isRecording = false;
        Events.OnRecordingEnd?.Invoke();
    }

    internal void OnWithDrawPressedOnTask(int groupNo, TaskList currentTask)
    {
        var taskno = FindAnyObjectByType<GroupController>().group.tasks.IndexOf(currentTask);
        sessionData.groupData[groupNo - 1].State = State.withdraw;
        groupPlayedStates[groupNo - 1].isPlayed = true;
        if (sessionData.groupData[groupNo - 1].tasks[taskno].subTasks[currentTask.playedTime - 1] != null)
        {
            var subtask = sessionData.groupData[groupNo - 1].tasks[taskno].subTasks[currentTask.playedTime - 1];
            subtask.state = State.Skip;

            var totaltime = TaskTimer.StopTimer(TaskTimerCourtine);

            subtask.totalTime = totaltime.ToString();

            subtask.timeWhenTask_COmpleted = GetCurrentTime();

            if(currentTask.playedTime == 2)
            {
                currentTask.isCompleted = true;
            }
        }

        //SetGroupPlayed(groupNo);
    }

    private void SetGroupPlayed(int groupNo)
    {
        foreach (var grp in groupPlayedStates)
        {
            if (grp.groupNo == groupNo)
            {
                grp.isPlayed = true;
                return;
            }
        }
    }

    internal void OnBackTaskClicked(int groupNo, TaskList currentTask)
    {
        var taskno = FindAnyObjectByType<GroupController>().group.tasks.IndexOf(currentTask);
        sessionData.groupData[groupNo - 1].State = State.NotPlayed;
        if (sessionData.groupData[groupNo - 1].tasks[taskno].subTasks[currentTask.playedTime - 1] != null)
        {
            var subtask = sessionData.groupData[groupNo - 1].tasks[taskno].subTasks[currentTask.playedTime - 1];
            subtask.state = State.Skip;

            var totaltime = TaskTimer.StopTimer(TaskTimerCourtine);

            subtask.totalTime = totaltime.ToString();

            subtask.timeWhenTask_COmpleted = GetCurrentTime();

            if (currentTask.playedTime == 2)
            {
                currentTask.isCompleted = true;
            }
        }
    }

    internal void OnSkipTask(int groupNo, TaskList currentTask)
    {
        var taskno = FindAnyObjectByType<GroupController>().group.tasks.IndexOf(currentTask);
        var subtask = sessionData.groupData[groupNo - 1].tasks[taskno].subTasks[currentTask.playedTime - 1];
        subtask.state = State.Skip;

        var totaltime = TaskTimer.StopTimer(TaskTimerCourtine);

        subtask.totalTime = totaltime.ToString();

        subtask.timeWhenTask_COmpleted = GetCurrentTime();

        if (currentTask.playedTime == 2)
        {
            currentTask.isCompleted = true;
        }
    }

    internal void OnNewTaskStarted(int groupNo, TaskList currentTask)
    {
        SubTasks subTask = new SubTasks();
        subTask.state = State.NotPlayed;

        TaskTimer.StartTimer();
        var taskno = FindAnyObjectByType<GroupController>().group.tasks.IndexOf(currentTask);
        var subtaskNo = sessionData.groupData[groupNo - 1].tasks[taskno].subTasks.Count + 1; // + 1
        subTask.SubTaskNo = subtaskNo.ToString();
        sessionData.groupData[groupNo - 1].tasks[taskno].subTasks.Add(subTask);

    }

    private string GetCurrentTime()
    {
        DateTime currentDateTime = DateTime.Now;

        // Convert the DateTime to a string in a specific format
        string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        return formattedDateTime;
    }

    internal void OnLastTaskCompleted(int groupNo, TaskList currentTask)
    {
        var taskno = FindAnyObjectByType<GroupController>().group.tasks.IndexOf(currentTask);
        Debug.Log($"groupNo: {groupNo}, taskno: {taskno}, playedTime: {currentTask.playedTime}");

        if (groupNo - 1 < sessionData.groupData.Count &&
            taskno < sessionData.groupData[groupNo - 1].tasks.Count &&
            currentTask.playedTime - 1 < sessionData.groupData[groupNo - 1].tasks[taskno].subTasks.Count)
        {
            var task = sessionData.groupData[groupNo - 1].tasks[taskno].subTasks[currentTask.playedTime - 1];
            Debug.Log($"Accessing task: {task}");

            // Proceed with your logic
            if (currentTask.playedTime == 2)
            {
                currentTask.isCompleted = true;
            }

            task.state = State.Completed;

            var totaltime = TaskTimer.StopTimer(TaskTimerCourtine);
            task.totalTime = totaltime.ToString();
            task.timeWhenTask_COmpleted = GetCurrentTime();

            Debug.Log($"Task {currentTask.taskNo} in Group {groupNo} has been completed. Total time: {task.totalTime}");
        }
        else
        {
            Debug.LogError("Index out of range");
        }
    }

    IEnumerator startHandRecording(Chirality hand, int groupNo , TaskList currenttask)
    {
        bool firstDetected = false;
        while (true)
        {
            var data = handTrackingDataRecorder.GetHandTrackingData(hand);
            if (hand == Chirality.Left && data != null)
            {
                if(firstDetected == false)
                {
                    firstDetected = true;
                    var taskno = FindAnyObjectByType<GroupController>().group.tasks.IndexOf(currenttask);
                    sessionData.groupData[groupNo - 1].tasks[taskno].subTasks[currenttask.playedTime - 1].handDetectedForFirstTime = GetCurrentTime();
                }
                leftHandData.Add(data);
            }
            else if(hand == Chirality.Right && data != null)
            {
                if (firstDetected == false)
                {
                    firstDetected = true;
                    var taskno = FindAnyObjectByType<GroupController>().group.tasks.IndexOf(currenttask);
                    sessionData.groupData[groupNo - 1].tasks[taskno].subTasks[currenttask.playedTime - 1].handDetectedForFirstTime = GetCurrentTime();
                }
                rightHandData.Add(data);
            }
            CheckIfHandDetected();
            yield return new WaitForSeconds(intervalBetweenHandTrackingData);

        }
    }

    private void CheckIfHandDetected()
    {
        if (Hands.Provider.GetHand(Chirality.Right) == null && Hands.Provider.GetHand(Chirality.Left) == null)
        {
            Debug.Log("Hand Not Founded");
            if(FindAnyObjectByType<GameUi>() != null)
            {
                FindAnyObjectByType<GameUi>().Notify();
            }
           
        }
    }

    internal void OnGroupCompleted(Group group)
    {
        groupPlayedStates[group.groupNo - 1].isPlayed = true;
        sessionData.groupData[group.groupNo - 1].State = State.Completed;
        SceneManager.LoadScene(2);
    }
}
[Serializable]
public class SessionData
{
    public string SessionId;
    public string ParticipantId;
    public string TotalSessionTime;
    public State sessionState;
    public List<GroupData> groupData;
}
[Serializable]
public class GroupData
{
    public string GroupId;
    public State State;
    public string TotalTime;
    public List<Task> tasks;
}
[Serializable]
public class Task
{
    public string TaskNo;
    public string CompletedTimes;
    public List<SubTasks> subTasks = new List<SubTasks>();
}
[Serializable]
public class SubTasks
{
    public string SubTaskNo;
    public string handDetectedForFirstTime;
    public List<HandAndFingureMovement> LefthandAndFingureMovement;
    public List<HandAndFingureMovement> RighthandAndFingureMovement;
    public string timeWhenTask_COmpleted;
    public string totalTime;
    public State state;
}
public enum State
{
    NotPlayed, withdraw, cancel, Skip, Completed
}
[Serializable]
public class GroupPlayedState
{
    public int groupNo;
    public bool isPlayed;
}
[Serializable]
public class HandAndFingureMovement
{
    public string time;
    public List<Fingure> finguePos;
    public Vector3 handPos;
    public float pinchStrenght;
    public float grabStrenght;
    public Vector3 handVeclocity;
    public Quaternion handRotation;
}
[Serializable]
public class Fingure
{
    public Vector3 fingurePos;
    public bool isExtended;
    public Vector3 direction;
}