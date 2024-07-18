using UnityEngine;
using Leap;
using Leap.Unity;

public class PinchZoomGesture : MonoBehaviour
{
    public LeapProvider leapProvider;
    public float pinchThreshold = 30f; // Threshold for considering a pinch gesture

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
            Debug.Log("Zooming In");
            groupController.ShowGameOverPopup();
        }
    }
}
