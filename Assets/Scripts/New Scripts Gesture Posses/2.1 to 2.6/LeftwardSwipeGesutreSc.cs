using Leap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftwardSwipeGestureSc : MonoBehaviour
{
    private Controller controller;
    private bool indexFingerExtendedLeftward = false;
    public GroupController groupController;

    void Start()
    {
        controller = new Controller();
    }

    void Update()
    {
        Frame frame = controller.Frame();
        bool currentSingleFingerPoseDetected = false;

        foreach (Hand hand in frame.Hands)
        {
            if (IsSingleTapPose(hand))
            {
                currentSingleFingerPoseDetected = true;
                // Existing code for single tap pose detection
                // ...
            }

            // Check if only the index finger is extended and moving leftward
            if (IsIndexFingerOnlyExtended(hand) && IsMovingLeftward(hand) && Time.timeScale != 0)
            {
                indexFingerExtendedLeftward = true;
                Debug.Log("Index finger extended leftward swipe detected");
                groupController.ShowGameOverPopup();
            }
        }

        if (!currentSingleFingerPoseDetected)
        {
            // Existing code for handling other cases
            // ...
        }
    }

    bool IsIndexFingerOnlyExtended(Hand hand)
    {
        // Check if the index finger is extended
        bool indexExtended = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX].IsExtended;
        bool otherFingersNotExtended = true;

        // Ensure no other fingers are extended
        for (int i = 0; i < hand.Fingers.Count; i++)
        {
            if (i != (int)Finger.FingerType.TYPE_INDEX && hand.Fingers[i].IsExtended)
            {
                otherFingersNotExtended = false;
                break;
            }
        }

        return indexExtended && otherFingersNotExtended;
    }

    bool IsMovingLeftward(Hand hand)
    {
        // Check if the palm is moving leftward with sufficient velocity
        return hand.PalmVelocity.x < -0.20f; // Adjust the threshold as needed
    }

    bool IsSingleTapPose(Hand hand)
    {
        // Existing code for single tap pose detection
        // ...

        return Vector3.Angle(hand.Fingers[(int)Finger.FingerType.TYPE_INDEX].Direction, hand.Direction) < 20.0f;
    }
}
