using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class GameUi : MonoBehaviour
{
    public Button back, withdraw, skip, next, start , repeat, end;
    public Action onBackCLicked, onWithDrawCLicked, onSkipClicked, onNextClicked ,onStartClicked,onRepeatClicked,onEndClicked;
    public GameObject rec;
    public Text taskNo;
    public VideoController videoController;
    public NotificationManager notificationManager;

    private void Awake()
    {
        Debug.Log("Awake called");
        back.onClick.AddListener(OnBackCLicked);
        withdraw.onClick.AddListener(OnWithDrawClicked);
        skip.onClick.AddListener(OnSkipCLicked);
        next.onClick.AddListener(OnNextClicked);
        start.onClick.AddListener(OnStartClicked);
        repeat.onClick.AddListener(OnRepeatClicked);
        end.onClick.AddListener(OnEndClicked);
        repeat.interactable = false;
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable called");
        Events.OnRecordingStart += OnRecordingStarts;
        Events.OnRecordingRepeat += OnRecordingRepeat;
        Events.OnRecordingEnd += OnRecordingEnd;
    }

    private void OnRecordingEnd()
    {
        Debug.Log("OnRecordingEnd called");
        start.interactable = true;
        rec.SetActive(false);
        repeat.interactable = false;
    }
    private void OnDisable()
    {
        Debug.Log("OnDisable called");
        Events.OnRecordingStart -= OnRecordingStarts;
        Events.OnRecordingRepeat -= OnRecordingRepeat;
        Events.OnRecordingEnd -= OnRecordingEnd;
    }
    public void Notify()
    {
        if(!notificationManager.isOn)
        notificationManager.OpenNotification();
    }
    private void OnRecordingRepeat()
    {
        Debug.Log("OnRecordingRepeat called");
        start.interactable = false;
    }

    private void OnRecordingStarts()
    {
        Debug.Log("OnRecordingStarts called");
        start.interactable = false;
        rec.SetActive(true);
        repeat.interactable = true;
    }

    private void OnEndClicked()
    {
        Debug.Log("OnEndClicked called");
        onEndClicked?.Invoke();
    }

    private void OnRepeatClicked()
    {
        Debug.Log("OnRepeatClick called");
        onRepeatClicked?.Invoke();
    }

    private void OnStartClicked()
    {
        Debug.Log("OnStartClick called");
        onStartClicked?.Invoke();
        videoController.PlayVideo();

    }

    private void OnNextClicked()
    {
        Debug.Log("OnNextClicked called");
        onNextClicked?.Invoke();
        Events.OnRecordingEnd?.Invoke();
    }

    private void OnSkipCLicked()
    {
        Debug.Log("OnSkipClicked called");
        onSkipClicked?.Invoke();
        Events.OnRecordingEnd?.Invoke();
    }

    private void OnWithDrawClicked()
    {
        Debug.Log("OnWithDrawClicked called");
        onWithDrawCLicked?.Invoke();
        Events.OnRecordingEnd?.Invoke();
    }

    private void OnBackCLicked()
    {
        Debug.Log("OnBackCLicked called");
        onBackCLicked?.Invoke();
    }
  
}
