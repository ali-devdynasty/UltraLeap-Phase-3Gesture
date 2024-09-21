using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using SystemTask = System.Threading.Tasks.Task;



public class GroupControllerPhase3 : MonoBehaviour
{
    [Header("Task Related details")]
    public List<TaskScreen> taskScreens;
    public int currentScreenIndex = 0;
    TaskScreen currentTaskScreen = new TaskScreen();
    public TaskDetails currentTaskDetails;
    public int currentGroupNumber;
    public int demoTime = 10;
    private bool gestureMatch = false;
    private bool gestureConfirmed = false;   


    GameUi gameUi;
    DataManager dataManager;
    public bool isGameOver = false;
    public bool isDetecting = false;

    [Space(10)]
    [Header("Ui")]
    public GameObject gameOverUI;



    #region EventsBinding

    private void OnEnable()
    {
        gameUi = FindObjectOfType<GameUi>();
        dataManager = FindObjectOfType<DataManager>();
        gameUi.onStartClicked += OnStartClicked;
        gameUi.onRepeatClicked += OnRepeatClicked;
        gameUi.onSkipClicked += OnSkipClicked;
        gameUi.onWithDrawCLicked += OnWithDrawClicked;
        gameUi.onMatchClicked += OnMatchClicked;
        gameUi.onUnmatchClicked += OnUnMatchClicked;
        dataManager.OnNewTaskStarted(currentGroupNumber, currentTaskDetails);
        currentTaskDetails.playedTime++;
       

        //Set the Task Number on the UI
        gameUi.taskNo.text = currentGroupNumber.ToString() + "."+currentTaskDetails.taskNo;
    }

   

    private void OnDisable()
    {
        gameUi.onStartClicked -= OnStartClicked;
        gameUi.onRepeatClicked -= OnRepeatClicked;
        gameUi.onSkipClicked -= OnSkipClicked;
        gameUi.onWithDrawCLicked -= OnWithDrawClicked;
        gameUi.onMatchClicked -= OnMatchClicked;
        gameUi.onUnmatchClicked -= OnUnMatchClicked;


    }
    private void OnDestroy()
    {
        gameUi.onStartClicked -= OnStartClicked;
        gameUi.onRepeatClicked -= OnRepeatClicked;
        gameUi.onSkipClicked -= OnSkipClicked;
        gameUi.onWithDrawCLicked -= OnWithDrawClicked;
        gameUi.onMatchClicked -= OnMatchClicked;
        gameUi.onUnmatchClicked -= OnUnMatchClicked;



    }

    #endregion


    #region EventsFunctionality

    public void ActivateScreen(taskState taskState)
    {
        foreach (var taskScreen in taskScreens)
        {
            if (taskScreen.taskState == taskState)
            {
                taskScreen.Screen.SetActive(true);
                currentTaskScreen = taskScreen;
                currentScreenIndex = taskScreens.IndexOf(taskScreen);
            }
            else
            {
                taskScreen.Screen.SetActive(false);
            }
        }
    }

    private void OnWithDrawClicked()
    {
        if (dataManager.isRecording)
        {
            dataManager.OnEndTaskRecording(currentGroupNumber, currentTaskDetails);
        }
        dataManager.OnWithDrawPressedOnTask(currentGroupNumber, currentTaskDetails);

        SceneManager.LoadScene(2);
    }

    private void OnSkipClicked()
    {
        if (dataManager.isRecording)
        {
            dataManager.OnEndTaskRecording(currentGroupNumber, currentTaskDetails);
        }
        dataManager.OnSkipTask(currentGroupNumber, currentTaskDetails);

        //Load the next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public async void OnStartClicked()
    {
        ActivateScreen(taskState.demo);
        
        dataManager.OnStartButtonPressedOnTask(currentGroupNumber, currentTaskDetails);
        await SystemTask.Delay(demoTime * 1000); // Introduce a 10-second delay
        ActivateScreen(taskState.GestureDetecting);
        isDetecting = true;

    }

    private void OnRepeatClicked()
    {
        dataManager.OnRepeatTaskRecording(currentGroupNumber, currentTaskDetails);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    internal async void OnGestureDetected()
    {
        if (isGameOver || !isDetecting) return;
        isGameOver = true;
        ActivateScreen(taskState.GestureDetected);
        gestureConfirmed = false;
        while(!gestureConfirmed)
        {
            await SystemTask.Delay(100);
        }

        if (gestureMatch)
        {
            if (dataManager.isRecording)
            {
                dataManager.OnEndTaskRecording(currentGroupNumber, currentTaskDetails);
            }
            dataManager.OnLastTaskCompleted(currentGroupNumber, currentTaskDetails);
            await SystemTask.Delay(1000); // Introduce a 1-second delay
            ActivateScreen(taskState.FinalResult);
            dataManager.OnGroupCompletedPhase3(currentGroupNumber, currentTaskDetails);
            //Instantiate game over UI on the Canvas
            Instantiate(gameOverUI, gameUi.transform);

            await SystemTask.Delay(5000); // Introduce a 5-second delay
            SceneManager.LoadScene(2);
        }
        else
        {
            OnRepeatClicked();
        }



    }

    private void OnMatchClicked()
    {
        gestureMatch = true;
        gestureConfirmed = true;
    }
    private void OnUnMatchClicked()
    {
        gestureMatch = false;
        gestureConfirmed = true;

    }
    #endregion

}
public enum taskState
{
    demo,GestureDetecting,GestureDetected,FinalResult
}
[System.Serializable]
public class TaskScreen
{
    public taskState taskState;
    public GameObject Screen;
}