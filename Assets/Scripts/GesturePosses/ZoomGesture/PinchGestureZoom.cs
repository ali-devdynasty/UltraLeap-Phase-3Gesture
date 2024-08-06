using UnityEngine;
using Leap;
using Leap.Unity;

public class PinchZoomGesture : MonoBehaviour
{
    public LeapProvider leapProvider;
    public float pinchThreshold = 30f; // Threshold for considering a pinch gesture
    public float movementThreshold = 10f; // Threshold for hand movement to ignore gestures

    private bool isRightHandInitialPinch = false;
    private bool isLeftHandInitialPinch = false;
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
            if (hand.PalmVelocity.x > movementThreshold)
            {
                // Hand movement is significant, skip pinch detection
                continue;
            }

            if (hand.IsRight)
            {
                HandlePinchZoom(hand, ref isRightHandInitialPinch);
            }
            else if (hand.IsLeft)
            {
                HandlePinchZoom(hand, ref isLeftHandInitialPinch);
            }
        }
    }

    private void HandlePinchZoom(Hand hand, ref bool isInitialPinch)
    {
        float pinchDistance = hand.PinchDistance;

        if (!isInitialPinch && pinchDistance < pinchThreshold)
        {
            // Initial pinch detected
            isInitialPinch = true;
            Debug.Log("Initial Pinch Detected");
        }
        else if (isInitialPinch && pinchDistance >= pinchThreshold)
        {
            // Fingers spread out after initial pinch
            isInitialPinch = false;

            // Add your logic here to zoom in (e.g., scale the object)
            // based on the direction of the hand movement

            Debug.Log("Zooming In");
            groupController.ShowGameOverPopup();
        }
    }

}
