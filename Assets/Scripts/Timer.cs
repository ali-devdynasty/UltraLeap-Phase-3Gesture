using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    private bool isRunning;
    private float elapsedTime;

    // Start the timer coroutine
    public Coroutine StartTimer()
    {
        if (!isRunning)
        {
            isRunning = true;
            return StartCoroutine(TimerCoroutine());
        }
        else
        {
            Debug.LogWarning("Timer is already running. Stopping Timer and Running Again");
            StopAllCoroutines();
            isRunning = true;
            return StartCoroutine(TimerCoroutine());
        }
    }

    // Stop the timer coroutine and return the elapsed time
    public float StopTimer(Coroutine timerCoroutine)
    {
        if (isRunning && timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            isRunning = false;
            return elapsedTime;
        }
        else
        {
            Debug.LogWarning("Timer is not running or coroutine is invalid.");
            return 0f;
        }
    }

    // Coroutine for the timer
    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
