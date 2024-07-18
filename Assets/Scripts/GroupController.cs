using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        dataManager = FindObjectOfType<DataManager>();
        dataManager.OnGroupStarted(group.groupNo);
        StartNextTask();
        dataManager.OnNewTaskStarted(group.groupNo, currentTask);

    }
    public void ShowGameOverPopup()
    {
        // Check if currentTask is not task number 1
        if (currentTask != null && currentTask.taskNo != 1)
        {
            if (instantiatedPopup == null)
            {
                instantiatedPopup = Instantiate(gameOverPrefab, gameOverCanvas.transform);
                StartCoroutine(HandleSceneTransition());
            }
        }
    }
    private IEnumerator HandleSceneTransition()
    {
        yield return new WaitForSeconds(5.0f);
        dataManager.OnGroupCompleted(group);
        Destroy(instantiatedPopup);
    }


    private void StartNextTask()
    {
        bool allCompleted = true;
        foreach (var task in group.tasks)
        {
            if (!task.isCompleted)
            {
                allCompleted = false;
                break;
            }
        }

        if (allCompleted)
        {
            dataManager.OnGroupCompleted(group);
            return; // All tasks completed
        }

        // Close all tasks
        foreach (var task in group.tasks)
        {
            task.taskObj.SetActive(false);
        }

        // If the current task can be played again
        if (currentTask != null && currentTask.playedTime < 2 && currentTask.taskObj != null)
        {
            currentTask.playedTime++;
            currentTask.taskObj.SetActive(true);
            gameUi.taskNo.text = $"{group.groupNo}.{currentTask.taskNo}";
        }
        else
        {
            // Find the next uncompleted task in order
            for (int i = 0; i < group.tasks.Count; i++)
            {
                TaskList task = group.tasks[taskIndex];
                taskIndex = (taskIndex + 1) % group.tasks.Count;
                Debug.Log("the currently task No is  " + taskIndex);
               

                if (!task.isCompleted && task.playedTime < 2)
                {
                    currentTask = task;
                    currentTask.playedTime++;

                    task.taskObj.SetActive(true);
                    gameUi.taskNo.text = $"{group.groupNo}.{task.taskNo}";

                    Debug.Log(" current task is no " + taskIndex);

                    return;
                }

            }

        }

    }


    private void OnEnable()
    {
        gameUi = FindAnyObjectByType<GameUi>();
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
        StartNextTask();
        dataManager.OnNewTaskStarted(group.groupNo, currentTask);
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
        StartNextTask();
        dataManager.OnNewTaskStarted(group.groupNo, currentTask);
    }

    private void OnStart()
    {
        dataManager.OnStartButtonPressedOnTask(group.groupNo, currentTask);
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