using UnityEngine;
using Leap;
using Leap.Unity;
using System.Collections.Generic; // Add this using directive

public class IndexFingerRotationGesture : MonoBehaviour
{
    Controller leapController;
    const float minRotationAngle = 30f; // Minimum angle threshold for rotation
    const Finger.FingerType fingerType = Finger.FingerType.TYPE_INDEX;
    public GroupControllerPhase3 groupController;
    private Vector3 lastDirection;
    private float cumulativeAngle;

    void Start()
    {
        leapController = new Controller();
        lastDirection = Vector3.zero;
        cumulativeAngle = 0f;
    }

    void Update()
    {
        Frame frame = leapController.Frame();
        List<Hand> hands = frame.Hands;

        foreach (Hand hand in hands)
        {
            if (IsIndexExtended(hand))
            {
                DetectClockwiseRotation(hand);
            }
            else
            {
                ResetGesture(); // Reset if the index is not extended
            }
        }
    }

    bool IsIndexExtended(Hand hand)
    {
        Finger index = hand.Fingers[(int)fingerType];
        return index.IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_THUMB].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_MIDDLE].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_RING].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_PINKY].IsExtended;
    }

    void DetectClockwiseRotation(Hand hand)
    {
        Finger index = hand.Fingers[(int)fingerType];
        Vector3 currentDirection = new Vector3(index.Direction.x, 0, index.Direction.z); // Project onto XZ plane

        // Initialize lastDirection on first detection
        if (lastDirection == Vector3.zero)
        {
            lastDirection = currentDirection;
            return;
        }

        // Calculate the signed angle for clockwise movement
        float angle = Vector3.SignedAngle(lastDirection, currentDirection, Vector3.up);

        // Only accumulate clockwise rotation (negative angle values)
        if (angle < 0)
        {
            cumulativeAngle += -angle; // Add the absolute value to cumulativeAngle
        }

        // Check if cumulative clockwise rotation exceeds the threshold
        if (cumulativeAngle >= minRotationAngle)
        {
            Debug.Log("Index finger rotated clockwise.");
            groupController.OnGestureDetected();
            ResetGesture(); // Reset after successful detection
        }

        // Update lastDirection for the next frame
        lastDirection = currentDirection;
    }

    void ResetGesture()
    {
        lastDirection = Vector3.zero;
        cumulativeAngle = 0f;
    }
}

