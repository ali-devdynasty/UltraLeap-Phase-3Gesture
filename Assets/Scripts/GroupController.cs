using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GroupController : MonoBehaviour
{
    public Group group;
    GameUi gameUi;
    DataManager dataManager;

    public TaskList currentTask;
    private int taskIndex = 0;

    public GameObject gameOverPrefab;
    public Canvas gameOverCanvas;
    private GameObject instantiatedPopup;
 
    private void Start()
    {
        // Start the group when the script starts
        Debug.Log("GroupController Start");
        dataManager = FindObjectOfType<DataManager>();
        gameUi = FindObjectOfType<GameUi>();
        //dataManager.OnGroupStarted(group.groupNo);
        ////StartNextTask();
        //dataManager.OnNewTaskStarted(group.groupNo, currentTask);
    }

    
    public void ShowGameOverPopup()
    {
        // Check if currentTask is task number 2
        if (currentTask != null && currentTask.taskNo == 2)
        {
            Debug.Log("Showing Game Over Popup");
            // Move to the next task (task number 3)
            MoveToNextTask();
        }
    }
   

    private void ShowCompletionPopup()
    {
        Debug.Log("Showing completion popup.");
        if (AllTasksCompleted())
        {
            HandleSceneTransition();

        }
    }

    private void HandleSceneTransition()
    {
        Debug.Log("Handling scene transition");
        dataManager.OnGroupCompleted(group);
        Destroy(instantiatedPopup);
    }

    private void StartNextTask(bool repeatTask = false)
    {
        Debug.Log("Starting next task");
        if (AllTasksCompleted())
        {
            Debug.Log("All tasks completed in StartNextTask.");
            dataManager.OnGroupCompleted(group);
            ShowCompletionPopup();
            return; // All tasks completed
        }

        // Close all tasks
        foreach (var task in group.tasks)
        {
            task.taskObj.SetActive(false);
        }

        if (!repeatTask)
        {
            // Find the next uncompleted task in order
            for (int i = 0; i < group.tasks.Count; i++)
            {
                TaskList task = group.tasks[taskIndex];
                taskIndex = (taskIndex + 1) % group.tasks.Count;
                Debug.Log($"Task index updated to: {taskIndex}");

                if (!task.isCompleted && task.playedTime < 2)
                {
                    currentTask = task;
                    currentTask.playedTime++;

                    task.taskObj.SetActive(true);
                    //gameUi.taskNo.text = $"/*{group.groupNo}.{task.taskNo}*/";
                    gameUi.taskNo.text = $"{group.taskNumberGet}";

                    Debug.Log("Current task is No " + task.taskNo);
                    if (currentTask.taskNo == 4)
                    {
                        instantiatedPopup = Instantiate(gameOverPrefab, gameOverCanvas.transform);
                    }

                    // Check task number and set the appropriate timer
                    if (currentTask.taskNo != 2)
                    {
                        Invoke(nameof(TaskTimerCompleted), GetTaskDuration(currentTask.taskNo));
                    }
                    return;
                }
            }
        }
        else
        {
            // Handle task repetition
            if (currentTask != null)
            {
                currentTask.playedTime++;
                currentTask.taskObj.SetActive(true);
                gameUi.taskNo.text = $"{group.taskNumberGet}";

                Debug.Log("Repeating current task No " + currentTask.taskNo);
                if (currentTask.taskNo == 4)
                {
                    instantiatedPopup = Instantiate(gameOverPrefab, gameOverCanvas.transform);
                }

                StartTaskTimer();
                
            }
        }
    }
     
    private void StartTaskTimer()
    {
        CancelInvoke(nameof(TaskTimerCompleted));

        if (currentTask != null && currentTask.taskNo != 2)
        {
            Invoke(nameof(TaskTimerCompleted), GetTaskDuration(currentTask.taskNo));
        }
    }
    private void TaskTimerCompleted()
    {
        Debug.Log("Task timer completed");
        if (currentTask != null)
        {
            currentTask.isCompleted = true;
            dataManager.OnLastTaskCompleted(group.groupNo, currentTask);
            Debug.Log($"Task {currentTask.taskNo} is marked as completed.");

            if (AllTasksCompleted())
            {
                Debug.Log("All tasks completed in TaskTimerCompleted.");
                dataManager.OnGroupCompleted(group);
                ShowCompletionPopup();
            }
            else
            {
                StartNextTask();
                dataManager.OnNewTaskStarted(group.groupNo, currentTask);
            }
        }
    }

    private float GetTaskDuration(int taskNo)
    {
        switch (taskNo)
        {
            case 1:
            case 3:
            case 4:
                return 5f; // Example duration, set your actual duration here
            default:
                return 0f;
        }
    }

    private void MoveToNextTask()
    {
        currentTask.isCompleted = true;
        dataManager.OnLastTaskCompleted(group.groupNo, currentTask);
        StartNextTask();
        dataManager.OnNewTaskStarted(group.groupNo, currentTask);
    }

    private bool AllTasksCompleted()
    {
        foreach (var task in group.tasks)
        {
            if (!task.isCompleted)
            {
                return false;
            }
        }
        return true;
    }


    private void OnEnable()
    {
        gameUi = FindObjectOfType<GameUi>();
        gameUi.onWithDrawCLicked += OnWithDraw;
        gameUi.onStartClicked += OnStart;
        gameUi.onSkipClicked += OnSkip;
        gameUi.onRepeatClicked += OnRepeat;
        gameUi.onEndClicked += OnEnd;
        gameUi.onBackCLicked += OnBack;
        gameUi.onNextClicked += OnNext;
    }

    private void OnDisable()
    {
        gameUi.onWithDrawCLicked -= OnWithDraw;
        gameUi.onStartClicked -= OnStart;
        gameUi.onSkipClicked -= OnSkip;
        gameUi.onRepeatClicked -= OnRepeat;
        gameUi.onEndClicked -= OnEnd;
        gameUi.onBackCLicked -= OnBack;
        gameUi.onNextClicked -= OnNext;
    }

    private void OnNext()
    {
        if (dataManager.isRecording)
        {
            OnEnd();
        }
        dataManager.OnLastTaskCompleted(group.groupNo, currentTask);
        if (AllTasksCompleted())
        {
            dataManager.OnGroupCompleted(group);
        }
        else
        {
            StartNextTask();
            dataManager.OnNewTaskStarted(group.groupNo, currentTask);
        }
    }

    private void OnBack()
    {
        dataManager.OnBackTaskClicked(group.groupNo, currentTask);
        SceneManager.LoadScene(2);
    }

    private void OnEnd()
    {
        dataManager.OnEndTaskRecording(group.groupNo, currentTask);
    }

    private void OnRepeat()
    {
        dataManager.OnRepeatTaskRecording(group.groupNo, currentTask);
        StartNextTask(true);    
    }

  

    private void OnSkip()
    {
        if (dataManager.isRecording)
        {
            OnEnd();
        }
        dataManager.OnSkipTask(group.groupNo, currentTask);
        if (currentTask.playedTime == 2)
        {
            currentTask.isCompleted = true;
        }
        // Load the next group scene
        int nextGroupNo = group.groupNo + 1;
        int nextSceneNo = nextGroupNo + 2; // Assuming your scenes are numbered accordingly

        Debug.Log($"Loading next group: {nextGroupNo}, scene: {nextSceneNo}");

        // Ensure the next scene exists and can be loaded
        if (nextSceneNo < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneNo);
        }
        else
        {
            Debug.LogWarning("Next scene number is out of build settings range.");
        }


    }


    private void OnStart()
    {
        //dataManager.OnStartButtonPressedOnTask(group.groupNo, currentTask);
        dataManager.OnGroupStarted(group.groupNo);
        StartNextTask();
        dataManager.OnNewTaskStarted(group.groupNo, currentTask);
    }

    private void OnWithDraw()
    {
        if (dataManager.isRecording)
        {
            OnEnd();
        }
        dataManager.OnWithDrawPressedOnTask(group.groupNo, currentTask);
        SceneManager.LoadScene(2);
    }
}

[Serializable]
public class Group
{
    public int groupNo;
    public string taskNumberGet;
    public List<TaskList> tasks;
}

[Serializable]
public class TaskList
{
    public GameObject taskObj;
    public int taskNo;
    [HideInInspector]
    public int playedTime;
    public bool isCompleted = false;
}