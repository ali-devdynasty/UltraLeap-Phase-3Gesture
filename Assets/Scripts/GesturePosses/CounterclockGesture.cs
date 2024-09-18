using UnityEngine;
using Leap;
using Leap.Unity;
using System.Collections.Generic;
using System.Collections; // Needed for coroutines

public class CounterclockGesture : MonoBehaviour
{
    Controller leapController;
    const float rotationThreshold = 0.5f; // Adjust threshold as needed
    const float minRotationSpeed = 10f; // Minimum speed for rotation detection
    const Finger.FingerType fingerType = Finger.FingerType.TYPE_INDEX; // Finger type to track
    public GroupControllerPhase3 groupController;
    private Vector3 lastDirection;
    private bool isFingerExtended;
    private Hand currentHand;

    void Start()
    {
        leapController = new Controller();
        lastDirection = Vector3.zero;
        isFingerExtended = false;
    }

    void Update()
    {
        Frame frame = leapController.Frame();
        List<Hand> hands = frame.Hands;

        foreach (Hand hand in hands)
        {
            if (IsIndexExtended(hand))
            {
                Debug.Log("Index finger is extended.");
                if (!isFingerExtended)
                {
                    isFingerExtended = true;
                    StartCoroutine(WaitAndDetectRotation(0.001f));
                    currentHand = hand;
                }
            }
            else
            {
                // Reset the state if the finger is not extended
                isFingerExtended = false;
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

    IEnumerator WaitAndDetectRotation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (isFingerExtended && currentHand != null)
        {
            DetectCounterclockwiseRotation(currentHand);
        }
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

        if (angle > rotationThreshold && Time.timeScale != 0)
        {
            Debug.Log("Index finger rotating counterclockwise.");
            groupController.OnGestureDetected();
            // Add your counterclockwise rotation handling code here
        }
        else if (angle < -rotationThreshold && Time.timeScale != 0)
        {
            Debug.Log("Index finger rotating clockwise.");
            // Optionally handle clockwise rotation here
        }

        lastDirection = currentDirection;
    }
}
