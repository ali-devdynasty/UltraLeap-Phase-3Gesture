using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    public GameObject gameOverPrefab;
    public Canvas gameOverCanvas;
    private GameObject instantiatedPopup;

    public void ShowGameOverPopup()
    {
        if (instantiatedPopup == null)
        {
            instantiatedPopup = Instantiate(gameOverPrefab, gameOverCanvas.transform);
            StartCoroutine(HandleSceneTransition());
        }
    }

    private IEnumerator HandleSceneTransition()
    {
        yield return new WaitForSeconds(5.0f);
        
        Destroy(instantiatedPopup);
    }
}
