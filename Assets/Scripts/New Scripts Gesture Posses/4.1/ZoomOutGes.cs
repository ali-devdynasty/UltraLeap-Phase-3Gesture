using Leap.Unity;
using Leap;
using UnityEngine;

public class ZoomOutGes : MonoBehaviour
{
    [HideInInspector] LeapProvider leapProvider;
    public float pinchThreshold = 30f; // Threshold for considering a separation gesture
    private bool isRightHandInitialSeparation = false;
    private bool isLeftHandInitialSeparation = false;
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
            if (hand.IsRight)
            {
                HandlePinchZoomOut(hand, ref isRightHandInitialSeparation);
            }
            else if (hand.IsLeft)
            {
                HandlePinchZoomOut(hand, ref isLeftHandInitialSeparation);
            }
        }
    }

    private void HandlePinchZoomOut(Hand hand, ref bool isInitialSeparation)
    {
        Finger indexFinger = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
        Finger thumb = hand.Fingers[(int)Finger.FingerType.TYPE_THUMB];
        float pinchDistance = hand.PinchDistance;

        // Check if only the index finger and thumb are extended
        bool isPinchGesture = AreFingersInPinchPosition(hand, indexFinger, thumb);

        if (!isInitialSeparation && isPinchGesture && pinchDistance >= pinchThreshold)
        {
            // Initial separation detected (index and thumb spread out)
            isInitialSeparation = true;
            Debug.Log("Initial Separation Detected - Zoom Out");
        }
        else if (isInitialSeparation && pinchDistance < pinchThreshold && Time.timeScale != 0)
        {
            // Fingers close together after initial separation
            isInitialSeparation = false;
            Debug.Log("Zooming Out");
            groupController.OnGestureDetected();

        }
    }

    private bool AreFingersInPinchPosition(Hand hand, Finger indexFinger, Finger thumb)
    {
        // Check if index finger and thumb are extended and other fingers are not
        bool isIndexExtended = indexFinger.IsExtended;
        bool isThumbExtended = thumb.IsExtended;

        // Check if other fingers are not extended
        bool otherFingersNotExtended = true;
        foreach (Finger finger in hand.Fingers)
        {
            if (finger.Type != Finger.FingerType.TYPE_INDEX && finger.Type != Finger.FingerType.TYPE_THUMB && finger.IsExtended)
            {
                otherFingersNotExtended = false;
                break;
            }
        }

        return isIndexExtended && isThumbExtended && otherFingersNotExtended;
    }
}
