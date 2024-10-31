using Leap;
using UnityEngine;

public class DownwardSwipeDetection : MonoBehaviour
{
    private Controller controller;
    private bool indexFingerExtended = false;
    private bool swipeDetected = false;
    public GroupControllerPhase3 groupController;

    void Start()
    {
        controller = new Controller();
    }

    void Update()
    {
        Frame frame = controller.Frame();

        foreach (Hand hand in frame.Hands)
        {
            // Step 1: Detect when the index finger is extended
            if (!indexFingerExtended && IsIndexFingerOnlyExtended(hand))
            {
                indexFingerExtended = true;
                Debug.Log("Index finger is extended, ready for downward swipe");
            }

            // Step 2: Check if the hand is moving downward while the index finger remains extended
            if (indexFingerExtended && IsMovingDownward(hand) && !swipeDetected && Time.timeScale != 0)
            {
                swipeDetected = true;
                Debug.Log("Downward swipe detected");
                groupController.OnGestureDetected();

                // Reset after the swipe to track future gestures
                ResetGestureTracking();
            }

            // If the index finger is no longer extended, reset to allow for future detection
            if (!IsIndexFingerOnlyExtended(hand))
            {
                ResetGestureTracking();
            }
        }
    }

    void ResetGestureTracking()
    {
        indexFingerExtended = false;
        swipeDetected = false;
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

    bool IsMovingDownward(Hand hand)
    {
        // Check if the hand is moving downward
        return hand.PalmVelocity.y < -0.25f; // Adjust the threshold as needed
    }
}
