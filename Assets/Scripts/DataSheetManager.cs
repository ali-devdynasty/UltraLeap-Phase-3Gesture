using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;


// Android-specific namespaces
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
public class DataSheetManager : MonoBehaviour
{
    public Button enterButton, backbtn, exportButton;
    public TMP_InputField sessionid, participantid;
    public TextMeshProUGUI displaytest, expottext;



    // Start is called before the first frame update
    void Start()
    {
        enterButton.onClick.AddListener(OnEnterButtonClicked);
        backbtn.onClick.AddListener(OnBackClicked);
        exportButton.onClick.AddListener(OnExportButton);


    }

  

    private void OnExportButton()
    {
        if (sessionid.text != "" && participantid.text != "")
        {
            
            string alltext = DataManager.instance.GetData(sessionid.text, participantid.text);

            // Get the persistent data path for the platform
            string path = Path.Combine(Application.persistentDataPath, participantid.text + sessionid.text + ".txt");

            // Write the text to the file
            File.WriteAllText(path, alltext);

            StartCoroutine(ShowExport(path));

#if UNITY_ANDROID
            // Request permission to access external storage
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }

            // Open file explorer
            OpenFileExplorer(Application.persistentDataPath);

#endif
        }
    }

    IEnumerator ShowExport(string path)
    {
        expottext.gameObject.SetActive(true);
        expottext.text = "Exported data to: " + path;
        yield return new WaitForSeconds(10);
        expottext.text = "";
    }

    private void OpenFileExplorer(string directoryPath)
    {
#if UNITY_ANDROID
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        // Set action for the intent
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_GET_CONTENT"));

        // Create a URI from the directory path
        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + directoryPath);

        // Set the URI for the intent
        intentObject.Call<AndroidJavaObject>("setDataAndType", uriObject, "*/*");

        // Get the current Unity activity
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

        // Start activity with the intent
        currentActivity.Call("startActivity", intentObject);
#endif
    }







    private void OnBackClicked()
    {
        SceneManager.LoadScene(0);
    }

    private void OnEnterButtonClicked()
    {
        if (sessionid.text != "" && participantid.text != "")
        {
            string alltext = DataManager.instance.GetData(sessionid.text, participantid.text);
            Debug.Log(alltext); 
            displaytest.text = alltext;
        }
    }
}
