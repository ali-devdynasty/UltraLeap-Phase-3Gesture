using UnityEngine;
using Leap;
using Leap.Unity;
using System.Collections.Generic; // Add this using directive

public class IndexFingerRotationGesture : MonoBehaviour
{
    Controller leapController;
    const float rotationThreshold = 0.5f; // Adjust threshold as needed
    const float referenceAngle = 90f; // Angle to compare for clockwise rotation
    const Finger.FingerType fingerType = Finger.FingerType.TYPE_INDEX; // Finger type to track
    public GroupControllerPhase3 groupController;

    void Start()
    {
        leapController = new Controller();
    }

    void Update()
    {
        Frame frame = leapController.Frame();
        List<Hand> hands = frame.Hands; // Use List<Hand> instead of HandList

        foreach (Hand hand in hands)
        {
            if (IsIndexExtended(hand))
            {
                // Log that the index finger is extended
                Debug.Log("Index finger is extended.");

                // Now check for clockwise rotation
                DetectClockwiseRotation(hand);
            }
        }
    }

    bool IsIndexExtended(Hand hand)
    {
        Finger index = hand.Fingers[(int)fingerType];
        return index.IsExtended && !hand.Fingers[(int)Finger.FingerType.TYPE_THUMB].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_MIDDLE].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_RING].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_PINKY].IsExtended;
    }

    void DetectClockwiseRotation(Hand hand)
    {
        Finger index = hand.Fingers[(int)fingerType];
        Vector3 indexDirection = new Vector3(index.Direction.x, index.Direction.y, index.Direction.z);

        // Calculate angle in x-z plane relative to a reference direction (assuming positive x-axis is the reference)
        float angle = Mathf.Atan2(indexDirection.z, indexDirection.x) * Mathf.Rad2Deg;

        // Adjust angle to range 0-360 degrees
        if (angle < 0)
        {
            angle += 360f;
        }

        // Check if angle indicates clockwise rotation
        if (angle < referenceAngle && angle > referenceAngle - rotationThreshold && Time.timeScale != 0)
        {
            Debug.Log("Index finger is extended and rotating clockwise.");
            groupController.OnGestureDetected();
            // Add your clockwise rotation handling code here
        }
    }
}