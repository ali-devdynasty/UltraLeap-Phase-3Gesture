using Leap;
using UnityEngine;

public class DownwardSwipeDetection : MonoBehaviour
{
    private Controller controller;
    private bool indexFingerExtendedDownward = false;
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
            // Check if only the index finger is extended and moving downward
            if (IsIndexFingerOnlyExtended(hand) && -hand.PalmVelocity.y > 0.25f && Time.timeScale != 0) // Adjust the threshold as needed
            {
                indexFingerExtendedDownward = true;
                Debug.Log("Downward swipe detected");
                groupController.ShowGameOverPopup();
            }

            if (IsSingleTapPose(hand))
            {
                currentSingleFingerPoseDetected = true;
                // Existing code for single tap pose detection
                // ...
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

    bool IsSingleTapPose(Hand hand)
    {
        // Existing code for single tap pose detection
        // ...

        return Vector3.Angle(hand.Fingers[(int)Finger.FingerType.TYPE_INDEX].Direction, hand.Direction) < 20.0f;
    }
}
