using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIPartial : MonoBehaviour

{
    public Button start, withdraw, skip;
    public Action onStartClicked, onWithDrawCLicked, onSkipClicked;
    public VideoController videoController;

    private void Awake()
    {
        Debug.Log("GameUiPartial Awake called");
        start.onClick.AddListener(OnStartClicked);
        withdraw.onClick.AddListener(OnWithDrawClicked);
        skip.onClick.AddListener(OnSkipCLicked);
    }

    private void OnStartClicked()
    {
        Debug.Log("GameUiPartial OnStartClick called");
        onStartClicked?.Invoke();
        videoController.PlayVideo();
    }

    private void OnWithDrawClicked()
    {
        Debug.Log("GameUiPartial OnWithDrawClicked called");
        onWithDrawCLicked?.Invoke();
        Events.OnRecordingEnd?.Invoke();
    }

    private void OnSkipCLicked()
    {
        Debug.Log("GameUiPartial OnSkipClicked called");
        onSkipClicked?.Invoke();
        Events.OnRecordingEnd?.Invoke();
    }
}


