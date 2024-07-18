using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class interface2 : MonoBehaviour
{
    public TMP_InputField participantId, SessionId;
    public Button startPhase2Button,backButton;

    private void Awake()
    {
        startPhase2Button.onClick.AddListener(OnStartPhase2ButtonCLicked);
        backButton.onClick.AddListener(OnBAckCLicekd);
    }

    private void OnBAckCLicekd()
    {
        SceneManager.LoadScene(0);
    }

    private void OnStartPhase2ButtonCLicked()
    {
        if (participantId.text == "" || SessionId.text == "") return;

        DataManager.instance.sessionData.ParticipantId = participantId.text;
        DataManager.instance.sessionData.SessionId = SessionId.text;

        DataManager.instance.OnSessionStart();

        SceneManager.LoadScene(2);
    }
}
