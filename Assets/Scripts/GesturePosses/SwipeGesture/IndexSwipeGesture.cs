using UnityEngine;
using Leap;
using Leap.Unity;
using Unity.VisualScripting;

public class IndexSwipeGesture : MonoBehaviour
{
    public LeapProvider leapProvider;
    public float swipeThreshold = 0.1f; // Minimum distance for swipe detection
    public float swipeSpeedThreshold = 1.5f; // Minimum speed for swipe detection

    private bool IndexExtended = false;
    private Vector3 indexFingerStartPosition;
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
        HandSettelment();


    }
    #region HandEngagement
    void HandSettelment()
    {
        Frame frame = leapProvider.CurrentFrame;
        foreach (Hand hand in frame.Hands)
        {
            if (hand.IsRight) // Check for the right hand
            {
                // Check if the index finger is extended
                Finger indexFinger = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
                IndexExtended = indexFinger.IsExtended;

                // If index finger is extended, track its position
                if (IndexExtended)
                {
                    // Convert Leap Motion Vector to Unity Vector3
                    Vector3 currentTipPosition = new Vector3(indexFinger.TipPosition.x, indexFinger.TipPosition.y, indexFinger.TipPosition.z);

                    // Detect swipe gestures
                    DetectSwipeGestures(hand, currentTipPosition);
                }
            }
        }
    }
    #endregion

    #region DetectSwipeGestures
    private void DetectSwipeGestures(Hand hand, Vector3 currentTipPosition)
    {
        // Detect downward swipe
        if (currentTipPosition.y < indexFingerStartPosition.y - swipeThreshold)
        {
            float swipeSpeed = (currentTipPosition - indexFingerStartPosition).magnitude / Time.deltaTime;
            if (swipeSpeed > swipeSpeedThreshold)
            {
                Debug.Log("Downward Swipe Detected");

                // Reset start position for next swipe detection
                indexFingerStartPosition = currentTipPosition;
                return; // Exit early if a swipe is detected
            }
        }

        // Detect leftward swipe
        if (currentTipPosition.x < indexFingerStartPosition.x - swipeThreshold)
        {
            float swipeSpeed = (currentTipPosition - indexFingerStartPosition).magnitude / Time.deltaTime;
            if (swipeSpeed > swipeSpeedThreshold)
            {
                Debug.Log("Leftward Swipe Detected");

                // Reset start position for next swipe detection
                indexFingerStartPosition = currentTipPosition;
                groupController.ShowGameOverPopup();
                return; // Exit early if a swipe is detected
            }
        }

        // Detect rightward swipe
        if (currentTipPosition.x > indexFingerStartPosition.x + swipeThreshold)
        {
            float swipeSpeed = (currentTipPosition - indexFingerStartPosition).magnitude / Time.deltaTime;
            if (swipeSpeed > swipeSpeedThreshold)
            {
                Debug.Log("Rightward Swipe Detected");

                // Reset start position for next swipe detection
                indexFingerStartPosition = currentTipPosition;
                groupController.ShowGameOverPopup();

                return; // Exit early if a swipe is detected
            }
        }

        // Update start position if not swiping
        indexFingerStartPosition = currentTipPosition;
    }
 
}
#endregion