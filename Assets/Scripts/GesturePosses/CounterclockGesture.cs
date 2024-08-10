using UnityEngine;
using Leap;
using Leap.Unity;
using System.Collections.Generic;

public class CounterclockGesture : MonoBehaviour
{
    Controller leapController;
    const float rotationThreshold = 0.5f; // Adjust threshold as needed
    const float minRotationSpeed = 10f; // Minimum speed for rotation detection
    const Finger.FingerType fingerType = Finger.FingerType.TYPE_INDEX; // Finger type to track
    public GroupController groupController;
    private Vector3 lastDirection;

    void Start()
    {
        leapController = new Controller();
        lastDirection = Vector3.zero;
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
        Finger index = hand.Fingers[(int)fingerType];
        return index.IsExtended && !hand.Fingers[(int)Finger.FingerType.TYPE_THUMB].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_MIDDLE].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_RING].IsExtended &&
               !hand.Fingers[(int)Finger.FingerType.TYPE_PINKY].IsExtended;
    }

    void DetectCounterclockwiseRotation(Hand hand)
    {
        Finger index = hand.Fingers[(int)fingerType];
        Vector3 currentDirection = new Vector3(index.Direction.x, 0, index.Direction.z); // Project onto x-z plane

        if (lastDirection == Vector3.zero)
        {
            lastDirection = currentDirection;
            return;
        }

        float angle = Vector3.SignedAngle(lastDirection, currentDirection, Vector3.up);

        if (angle > minRotationSpeed && Time.timeScale !=0)
        {
            Debug.Log("Index finger rotating counterclockwise.");
            groupController.ShowGameOverPopup();
            // Add your counterclockwise rotation handling code here
        }

        lastDirection = currentDirection;
    }
}
