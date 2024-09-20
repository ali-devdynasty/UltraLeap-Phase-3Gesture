using UnityEngine;
using Leap;
using Leap.Unity;
using System.Collections.Generic;

public class CounterclockGesture : MonoBehaviour
{
    Controller leapController;
    const float rotationThreshold = 0.5f; // Adjust threshold as needed for sensitivity
    const float minRotationAngle = 30f; // Minimum angle to consider for counterclockwise rotation
    const Finger.FingerType fingerType = Finger.FingerType.TYPE_INDEX; // Track index finger
    public GroupControllerPhase3 groupController;
    private Vector3 lastDirection;

    void Start()
    {
        leapController = new Controller();
        lastDirection = Vector3.zero; // Initial direction
    }

    void Update()
    {
        Frame frame = leapController.Frame();
        List<Hand> hands = frame.Hands;

        foreach (Hand hand in hands)
        {
            if (IsIndexExtended(hand))
            {
                DetectCounterclockwiseRotation(hand);
            }
        }
    }

    bool IsIndexExtended(Hand hand)
    {
        // Detect if only the index finger is extended
        Finger index = hand.Fingers[(int)fingerType];
        return index.IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_THUMB].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_MIDDLE].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_RING].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_PINKY].IsExtended;
    }

    void DetectCounterclockwiseRotation(Hand hand)
    {
        Finger index = hand.Fingers[(int)fingerType];
        Vector3 currentDirection = new Vector3(index.Direction.x, 0, index.Direction.z); // Project onto XZ plane

        if (lastDirection == Vector3.zero)
        {
            lastDirection = currentDirection; // Store the initial direction
            return;
        }

        // Calculate the signed angle between the current direction and the last direction
        float angle = Vector3.SignedAngle(lastDirection, currentDirection, Vector3.up);

        // Check if the finger has rotated counterclockwise beyond the minimum rotation angle
        if (angle > minRotationAngle)
        {
            Debug.Log("Index finger rotating counterclockwise.");
                groupController.OnGestureDetected();
            lastDirection = currentDirection; // Update the last direction
        }
    }
}
  

