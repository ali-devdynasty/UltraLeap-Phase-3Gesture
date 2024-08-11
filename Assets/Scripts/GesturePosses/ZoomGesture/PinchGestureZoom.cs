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
    public GroupController groupController;

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

            float pinchDistance = hand.PinchDistance;

            if (!isInitialPinch && pinchDistance < pinchThreshold)
            {
                // Initial pinch detected
                isInitialPinch = true;
                initialPinchDistance = pinchDistance;
                Debug.Log("Initial Pinch Detected");
            }
            else if (isInitialPinch && pinchDistance >= pinchThreshold && Time.timeScale!= 0)
            {
                // Fingers spread out after initial pinch
                isInitialPinch = false;

                // Calculate zoom factor based on pinch distance change
                float zoomFactor = initialPinchDistance / pinchDistance;
                float newScale = transform.localScale.x * zoomFactor;

                // Apply zoom (e.g., scale the object)
                transform.localScale = Vector3.one * Mathf.Clamp(newScale, 0.5f, 2f); // Limit zoom range

                Debug.Log($"Zooming In)");
                groupController.ShowGameOverPopup();

            }
        }
        if (groupController == null)
        {
            groupController = FindObjectOfType<GroupController>();
        }
    }
}
