using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TextSwitchController : MonoBehaviour
{
    public static TextSwitchController instance;

    public Action<bool> isEnglish;

    public SwitchManager switchManager;

    public bool isenglish;

   
    private void Awake()
    {
        if (instance == null)
        {
             instance= this;
            DontDestroyOnLoad(gameObject);
            switchManager.onValueChanged.AddListener(OnSwitch);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSwitch(bool state)
    {
       isEnglish?.Invoke(state);
        isenglish = state;
    }
}
