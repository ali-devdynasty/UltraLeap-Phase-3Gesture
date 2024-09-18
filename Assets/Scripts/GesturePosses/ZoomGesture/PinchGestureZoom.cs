using UnityEngine;
using Leap;
using Leap.Unity;

public class PinchZoomGesture : MonoBehaviour
{
    public LeapProvider leapProvider;
    public float pinchThreshold = 30f; // Threshold for considering a pinch gesture
    public float movementThreshold = 10f; // Threshold for hand movement to ignore gestures
    public float zoomSpeed = 0.01f; // Adjust this value for desired zoom speed

    private bool isPinchStart = false;
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
            // Ignore significant hand movement
            if (hand.PalmVelocity.magnitude > movementThreshold)
            {
                isPinchStart = false;
                continue;
            }

            float pinchDistance = hand.PinchDistance;

            // Detect when the thumb and index finger are pinched (closed)
            if (!isPinchStart && pinchDistance < pinchThreshold)
            {
                isPinchStart = true;
                initialPinchDistance = pinchDistance;
                Debug.Log("Initial Pinch Detected: Thumb and index finger closed");
            }
            // Detect when the thumb and index finger are spread out after a pinch
            else if (isPinchStart && pinchDistance >= pinchThreshold)
            {
                isPinchStart = false;

                // Calculate zoom based on the pinch distance change
                float zoomFactor = initialPinchDistance / pinchDistance;
                float newScale = transform.localScale.x * zoomFactor;

                // Apply zoom with clamping
                transform.localScale = Vector3.one * Mathf.Clamp(newScale, 0.5f, 2f);

                Debug.Log("Zooming In: Fingers spread out");
                groupController.OnGestureDetected();
            }
        }

        if (groupController == null)
        {
            groupController = FindObjectOfType<GroupControllerPhase3>();
        }
    }
}
