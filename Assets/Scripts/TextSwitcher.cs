using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;


public class TextSwitcher : MonoBehaviour
{
    Text text;

    // public OnDestroyAction onDestroyAction; 

    public string English, Arabis;
    void Start()
    {

        text = gameObject.GetComponent<Text>();


        TextSwitchController.instance.isEnglish += SwitchLanguage;
        SwitchLanguage(TextSwitchController.instance.isenglish);

    }
    
    private void OnDisable()
    {
        TextSwitchController.instance.isEnglish -= SwitchLanguage;
    }
    public void SwitchLanguage(bool isEnglish)
    {
        
        if (isEnglish)
        {
            text.text = English;
        }
        else
        {
            text.text = Fa.faConvert(Arabis);
        }
    }
    [ContextMenu("TrasToArabic")]
    public void translatetoarabic()
    {
        text.text = Fa.faConvert(Arabis);
    }
}
