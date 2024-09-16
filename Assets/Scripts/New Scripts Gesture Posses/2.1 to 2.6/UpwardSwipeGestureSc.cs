using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class UpwardSwipeGestureSc : MonoBehaviour
{
    private Controller controller;
    private bool indexFingerExtendedUpward = false;
    public GroupControllerPhase3 groupController;

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

            // Check if only the index finger is extended and moving upward
            if (IsIndexFingerOnlyExtended(hand) && IsMovingUpward(hand) && Time.timeScale != 0)
            {
                indexFingerExtendedUpward = true;
                Debug.Log("Index finger extended upward swipe detected");
                groupController.OnGestureDetected();
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

    bool IsMovingUpward(Hand hand)
    {
        // Check if the palm is moving upward with sufficient velocity
        return hand.PalmVelocity.y > 0.25f; // Adjust the threshold as needed
    }

    bool IsSingleTapPose(Hand hand)
    {
        // Existing code for single tap pose detection
        // ...

        return Vector3.Angle(hand.Fingers[(int)Finger.FingerType.TYPE_INDEX].Direction, hand.Direction) < 20.0f;
    }
}
