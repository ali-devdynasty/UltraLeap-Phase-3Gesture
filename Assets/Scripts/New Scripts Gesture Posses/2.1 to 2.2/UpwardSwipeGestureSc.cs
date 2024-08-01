using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class UpwardSwipeGestureSc : MonoBehaviour
{
    private LeapProvider provider;
    private Vector3 previousFingerPosition;
    private Vector3 previousHandPosition;
    private bool isSwipingUp;

    //public GroupController groupController; // Make sure to assign this in the Inspector

    void Start()
    {
        provider = FindObjectOfType<LeapProvider>();
        previousFingerPosition = Vector3.zero;
        previousHandPosition = Vector3.zero;
        isSwipingUp = false;
    }

    void Update()
    {
        Frame frame = provider.CurrentFrame;

        foreach (Hand hand in frame.Hands)
        {
            if (IsOnlyIndexFingerExtended(hand))
            {
                Finger indexFinger = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
                Vector3 currentFingerPosition = indexFinger.TipPosition;
                Vector3 currentHandPosition = hand.PalmPosition;

                if (IsSwipingUp(currentFingerPosition) && IsHandStable(currentHandPosition))
                {
                    if (!isSwipingUp)
                    {
                        Debug.Log("Swipe detected: Upward");
                        Debug.Log($"Finger position: {currentFingerPosition}");
                        isSwipingUp = true;
                        //groupController.ShowGameOverPopup();
                    }
                }
                else
                {
                    isSwipingUp = false;
                }

                previousFingerPosition = currentFingerPosition;
                previousHandPosition = currentHandPosition;
            }
            else
            {
                isSwipingUp = false;
            }
        }
    }

    private bool IsOnlyIndexFingerExtended(Hand hand)
    {
        return hand.Fingers[(int)Finger.FingerType.TYPE_INDEX].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_THUMB].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_MIDDLE].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_RING].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_PINKY].IsExtended;
    }

    private bool IsSwipingUp(Vector3 currentFingerPosition)
    {
        float swipeThreshold = 0.02f; // Adjust this threshold as needed
        return currentFingerPosition.y - previousFingerPosition.y > swipeThreshold;
    }

    private bool IsHandStable(Vector3 currentHandPosition)
    {
        float stabilityThreshold = 0.01f; // Adjust this threshold as needed
        return (currentHandPosition - previousHandPosition).magnitude < stabilityThreshold;
    }
}
