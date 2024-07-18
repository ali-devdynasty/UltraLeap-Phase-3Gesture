using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeInterface : MonoBehaviour
{
    public Button Phase2Button,datasheet,exit;

    private void Awake()
    {
        Phase2Button.onClick.AddListener(OnPhase2ButtonCLicked);
        datasheet.onClick.AddListener(OnDataSheetClicked);
        exit.onClick.AddListener(OnExit);
    }

    private void OnExit()
    {
        Application.Quit();
    }

    private void OnDataSheetClicked()
    {
        SceneManager.LoadScene(21);
    }

    private void OnPhase2ButtonCLicked()
    {
        SceneManager.LoadScene(1);
    }
}
