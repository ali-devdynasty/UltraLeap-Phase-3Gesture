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

    public List<TaskPlayedState> TaskPlayedStates;

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

    public void SaveData()
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
        foreach (var group in TaskPlayedStates)
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

    internal void OnStartButtonPressedOnTask(int groupNo, TaskDetails currentTask)
    {
        Debug.Log("onStartbtn press finger movement trying to save");
        if (leftHandCoroutine != null)
            StopCoroutine(leftHandCoroutine);
        if (rightHandCoroutine != null)
            StopCoroutine(rightHandCoroutine);

        leftHandCoroutine = null;
        rightHandCoroutine = null;

        rightHandData = new List<HandAndFingureMovement>();
        leftHandData = new List<HandAndFingureMovement>();


        leftHandCoroutine = StartCoroutine(startHandRecording(Chirality.Left, groupNo, currentTask));
        rightHandCoroutine = StartCoroutine(startHandRecording(Chirality.Right, groupNo, currentTask));
        isRecording = true;
        Events.OnRecordingStart?.Invoke();
    }

    internal void OnRepeatTaskRecording(int groupNo, TaskDetails currentTask)
    {
        StopCoroutine(leftHandCoroutine);
        StopCoroutine(rightHandCoroutine);
        leftHandCoroutine = null;
        rightHandCoroutine = null;

        rightHandData = new List<HandAndFingureMovement>();
        leftHandData = new List<HandAndFingureMovement>();


        leftHandCoroutine = StartCoroutine(startHandRecording(Chirality.Left, groupNo, currentTask));
        rightHandCoroutine = StartCoroutine(startHandRecording(Chirality.Right, groupNo, currentTask));
        Events.OnRecordingRepeat?.Invoke();
        Debug.Log($"Task {currentTask.taskNo} in Group {groupNo} has been reset and recording restarted.");
    }



    internal List<HandAndFingureMovement> GetLeftHandData()
    {
        return leftHandData;
    }

    internal void OnEndTaskRecording(int groupNo, TaskDetails currentTask)
    {
        StopCoroutine(leftHandCoroutine);
        StopCoroutine(rightHandCoroutine);
        leftHandCoroutine = null;
        rightHandCoroutine = null;

        sessionData.groupData[groupNo - 1].tasks[currentTask.taskNo - 1].subTasks[currentTask.playedTime - 1].LefthandAndFingureMovement = leftHandData;
        sessionData.groupData[groupNo - 1].tasks[currentTask.taskNo - 1].subTasks[currentTask.playedTime - 1].RighthandAndFingureMovement = rightHandData;
        isRecording = false;
        Events.OnRecordingEnd?.Invoke();
    }




    internal void OnWithDrawPressedOnTask(int groupNo, TaskDetails currentTask)
    {
        sessionData.groupData[groupNo - 1].State = State.withdraw;
        //Get the task with taskno
        var taskno = float.Parse($"{groupNo}.{currentTask.taskNo}");
        //Find the task from the taskplayed list and set it to played
        foreach (var task in TaskPlayedStates)
        {
            if (task.TaskNo == taskno)
            {
                task.isPlayed = true;
                break;
            }
        }
        if (sessionData.groupData[groupNo - 1].tasks[currentTask.taskNo - 1].subTasks[currentTask.playedTime - 1] != null)
        {
            var subtask = sessionData.groupData[groupNo - 1].tasks[currentTask.taskNo - 1].subTasks[currentTask.playedTime - 1];
            subtask.state = State.Skip;

            var totaltime = TaskTimer.StopTimer(TaskTimerCourtine);

            subtask.totalTime = totaltime.ToString();

            subtask.timeWhenTask_COmpleted = GetCurrentTime();

            if (currentTask.playedTime == 2)
            {
                currentTask.isCompleted = true;
            }
        }

        SetTaskPlayed(groupNo, currentTask);
    }

    private void SetTaskPlayed(int groupNo, TaskDetails currenttask)
    {
        var taskno = float.Parse($"{groupNo}.{currenttask.taskNo}");
        foreach (var task in TaskPlayedStates)
        {
            if (task.TaskNo == taskno)
            {
                task.isPlayed = true;
            }
        }
    }

    internal void OnBackTaskClicked(int groupNo, TaskDetails currentTask)
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

    internal void OnSkipTask(int groupNo, TaskDetails currentTask)
    {
        var subtask = sessionData.groupData[groupNo - 1].tasks[currentTask.taskNo - 1].subTasks[currentTask.playedTime - 1];
        subtask.state = State.Skip;

        var totaltime = TaskTimer.StopTimer(TaskTimerCourtine);

        subtask.totalTime = totaltime.ToString();

        subtask.timeWhenTask_COmpleted = GetCurrentTime();

        if (currentTask.playedTime == 2)
        {
            currentTask.isCompleted = true;
        }
        SetTaskPlayed(groupNo, currentTask);
    }

    internal void OnNewTaskStarted(int groupNo, TaskDetails currentTask)
    {
        if (groupNo <= 0 || groupNo > sessionData.groupData.Count)
        {
            Debug.LogError($"Invalid group number: {groupNo}");
            return;
        }

        // Initialize subTasks list if it's null
        if (sessionData.groupData[groupNo - 1].tasks[currentTask.taskNo - 1].subTasks == null)
        {
            sessionData.groupData[groupNo - 1].tasks[currentTask.taskNo - 1].subTasks = new List<SubTasks>();
        }

        // Create and add a new subTask
        SubTasks subTask = new SubTasks();
        subTask.state = State.NotPlayed;
        subTask.SubTaskNo = (sessionData.groupData[groupNo - 1].tasks[currentTask.taskNo - 1].subTasks.Count + 1).ToString();

        sessionData.groupData[groupNo - 1].tasks[currentTask.taskNo - 1].subTasks.Add(subTask);
        SaveData();
    }


    private string GetCurrentTime()
    {
        DateTime currentDateTime = DateTime.Now;

        // Convert the DateTime to a string in a specific format
        string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        return formattedDateTime;
    }

    internal void OnLastTaskCompleted(int groupNo, TaskDetails currentTask)
    {
        var task = sessionData.groupData[groupNo - 1].tasks[currentTask.taskNo - 1].subTasks[currentTask.playedTime - 1];

        SetTaskPlayed(groupNo, currentTask);

        if (currentTask.playedTime == 2)
        {
            currentTask.isCompleted = true;
        }

        task.state = State.Completed;

        var totaltime = TaskTimer.StopTimer(TaskTimerCourtine);

        task.totalTime = totaltime.ToString();

        task.timeWhenTask_COmpleted = GetCurrentTime();


    }
    IEnumerator startHandRecording(Chirality hand, int groupNo, TaskDetails currenttask)
    {
        bool firstDetected = false;
        while (true)
        {
            var data = handTrackingDataRecorder.GetHandTrackingData(hand);
            if (hand == Chirality.Left && data != null)
            {
                if (firstDetected == false)
                {
                    firstDetected = true;
                    sessionData.groupData[groupNo - 1].tasks[currenttask.taskNo - 1].subTasks[currenttask.playedTime - 1].handDetectedForFirstTime = GetCurrentTime(); // index
                }
                leftHandData.Add(data);
            }
            else if (hand == Chirality.Right && data != null)
            {
                if (firstDetected == false)
                {
                    firstDetected = true;
                    sessionData.groupData[groupNo - 1].tasks[currenttask.taskNo - 1].subTasks[currenttask.playedTime - 1].handDetectedForFirstTime = GetCurrentTime(); //idex   
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
            var gameUiInstance = FindAnyObjectByType<GameUi>();
            if (gameUiInstance != null)
            {
                Debug.Log("GameUi instance found, calling Notify...");
                gameUiInstance.Notify();
            }
            else
            {
                Debug.LogWarning("GameUi instance not found");
            }
        }
    }

    internal void OnGroupCompleted(Group group)
    {
        TaskPlayedStates[group.groupNo - 1].isPlayed = true;
        sessionData.groupData[group.groupNo - 1].State = State.Completed;
        SceneManager.LoadScene(2);
    }

    internal void OnGroupCompletedPhase3(int groupNo, TaskDetails currentTask)
    {
        TaskPlayedStates[groupNo - 1].isPlayed = true;

        // Get the correct group data based on the group number
        var groupData = sessionData.groupData[groupNo - 1];

        // Mark the group as completed
        groupData.State = State.Completed;

        // Iterate through tasks and mark them as completed
        foreach (var task in groupData.tasks)
        {
            foreach (var subTask in task.subTasks)
            {
                subTask.state = State.Completed;
            }
        }

        // Save progress or call a scene load (based on your logic)
        SaveData();

        // Load the next scene or move to the next step of your game
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
public class TaskPlayedState
{
    public float TaskNo;
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