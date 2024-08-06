using UnityEngine;
using Leap;
using Leap.Unity;

public class RightwardSwipeGestureSc : MonoBehaviour
{
    private Controller controller;
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

            // Check if only the index finger is extended and moving rightward
            if (IsIndexFingerOnlyExtended(hand) && IsMovingRightward(hand))
            {
                Debug.Log("Index finger extended rightward swipe detected");
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

    bool IsMovingRightward(Hand hand)
    {
        // Check if the palm is moving rightward with sufficient velocity
        return hand.PalmVelocity.x > 0.20f && Mathf.Abs(hand.PalmVelocity.y) < 0.10f && Mathf.Abs(hand.PalmVelocity.z) < 0.10f; // Adjust thresholds as needed
    }

    bool IsSingleTapPose(Hand hand)
    {
        // Existing code for single tap pose detection
        // ...

        return Vector3.Angle(hand.Fingers[(int)Finger.FingerType.TYPE_INDEX].Direction, hand.Direction) < 20.0f;
    }
}
