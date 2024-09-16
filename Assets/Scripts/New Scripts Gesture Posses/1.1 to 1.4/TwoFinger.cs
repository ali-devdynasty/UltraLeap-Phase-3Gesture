using Leap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoFinger : MonoBehaviour
{
    private Controller controller;
    private bool twoFingerPoseDetected = false;
    public GroupControllerPhase3 groupController;

    void Start()
    {
        controller = new Controller();
    }

    void Update()
    {
       

        Frame frame = controller.Frame();
        bool currentTwoFingerPoseDetected = false;

        foreach (Hand hand in frame.Hands)
        {
            if (IsTwoFingerTapPose(hand))
            {
                currentTwoFingerPoseDetected = true;
                if (!twoFingerPoseDetected && Time.timeScale != 0)
                {
                    Debug.Log("Single Tap (two-finger) detected");
                    twoFingerPoseDetected = true;
                    groupController.OnGestureDetected();
                }
            }
        }

        if (!currentTwoFingerPoseDetected)
        {
            twoFingerPoseDetected = false;
        }
    }

    bool IsTwoFingerTapPose(Hand hand)
    {
        // Check if the thumb and index finger are extended
        Finger thumb = hand.Fingers[(int)Finger.FingerType.TYPE_THUMB];
        Finger indexFinger = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
        if (!thumb.IsExtended || !indexFinger.IsExtended)
            return false;

        // Check if other fingers are not extended
        for (int i = 0; i < hand.Fingers.Count; i++)
        {
            if (i != (int)Finger.FingerType.TYPE_THUMB && i != (int)Finger.FingerType.TYPE_INDEX && hand.Fingers[i].IsExtended)
            {
                return false;
            }
        }

        // Check if the thumb and index finger are spread out
        float distance = Vector3.Distance(thumb.TipPosition, indexFinger.TipPosition);
        return distance > 0.05f; // Adjust the threshold based on your needs
    }
}


