using UnityEngine;
using Leap;
using Leap.Unity;

public class PinchZoomGesture : MonoBehaviour
{
    public LeapProvider leapProvider;
    public float pinchThreshold = 30f; // Threshold for considering a pinch gesture
    public float movementThreshold = 10f; // Threshold for hand movement to ignore gestures
    public float zoomSpeed = 0.01f; // Adjust this value for desired zoom speed

    private bool isInitialPinch = false;
    private float initialPinchDistance = 0f;
    public GroupControllerPhase3 groupController;

    void Start()
    {
        if (leapProvider == null)
        {
            leapProvider = FindObjectOfType<LeapProvider>();
        }
    }

    void Update()
    {
        Frame frame = leapProvider.CurrentFrame;
        foreach (Hand hand in frame.Hands)
        {
            // Check if hand is moving significantly
            if (hand.PalmVelocity.magnitude > movementThreshold)
            {
                // Hand movement is significant, reset initial pinch state
                isInitialPinch = false;
                continue;
            }

            // Get thumb and index fingers
            Finger thumb = hand.Fingers[(int)Finger.FingerType.TYPE_THUMB];
            Finger indexFinger = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];

            // Check pinch distance between thumb and index finger
            //float pinchDistance = Vector3.Distance(thumb.TipPosition.ToVector3(), indexFinger.TipPosition.ToVector3());

            // Ensure no other fingers are extended
            bool otherFingersExtended = false;
            for (int i = 0; i < hand.Fingers.Count; i++)
            {
                if (i != (int)Finger.FingerType.TYPE_THUMB && i != (int)Finger.FingerType.TYPE_INDEX)
                {
                    Finger finger = hand.Fingers[i];
                    if (finger.IsExtended)
                    {
                        otherFingersExtended = true;
                        break;
                    }
                }
            }

            // Proceed only if no other fingers are extended
            if (otherFingersExtended)
            {
                isInitialPinch = false;
                continue;
            }

            // Initial pinch detection
            if (!isInitialPinch && pinchDistance < pinchThreshold)
            {
                isInitialPinch = true;
                initialPinchDistance = pinchDistance;
                Debug.Log("Initial Pinch Detected");
            }
            // Zoom logic when fingers spread after initial pinch
            else if (isInitialPinch && pinchDistance >= pinchThreshold && Time.timeScale != 0)
            {
                isInitialPinch = false;

                // Calculate zoom factor based on pinch distance change
                float zoomFactor = initialPinchDistance / pinchDistance;
                float newScale = transform.localScale.x * zoomFactor;

                // Apply zoom (e.g., scale the object)
                transform.localScale = Vector3.one * Mathf.Clamp(newScale, 0.5f, 2f); // Limit zoom range

                Debug.Log("Zooming In");
                groupController.OnGestureDetected();
            }
        }

        // Ensure groupController is assigned
        if (groupController == null)
        {
            groupController = FindObjectOfType<GroupControllerPhase3>();
        }
    }
}
