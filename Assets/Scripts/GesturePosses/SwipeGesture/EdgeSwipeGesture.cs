using UnityEngine;
using Leap;
using Leap.Unity;

public class EdgeSwipeGesture : MonoBehaviour
{
     LeapProvider leapProvider;
    public float swipeThreshold = 0.1f; // Minimum distance for swipe detection
    public float swipeSpeedThreshold = 1.5f; // Minimum speed for swipe detection

    private Vector3[] indexFingerStartPositions = new Vector3[2]; // Store start positions for both hands
    private bool[] isIndexExtended = new bool[2]; // Track index finger extension for both hands

    void Start()
    {
        if (leapProvider == null)
        {
            leapProvider = FindObjectOfType<LeapProvider>();
        }
    }

    void Update()
    {
        CheckEdgeSwipeGestures();
    }

    private void CheckEdgeSwipeGestures()
    {
        Frame frame = leapProvider.CurrentFrame;
        for (int i = 0; i < frame.Hands.Count; i++)
        {
            Hand hand = frame.Hands[i];
            int handIndex = hand.IsRight ? 0 : 1; // 0 for right hand, 1 for left hand

            // Check if the index finger is extended
            Finger indexFinger = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
            isIndexExtended[handIndex] = indexFinger.IsExtended;

            // If index finger is extended, track its position and detect edge swipe gestures
            if (isIndexExtended[handIndex])
            {
                Vector3 currentTipPosition = new Vector3(indexFinger.TipPosition.x, indexFinger.TipPosition.y, indexFinger.TipPosition.z);

                // Detect edge leftward swipe
                if (currentTipPosition.x < indexFingerStartPositions[handIndex].x - swipeThreshold)
                {
                    float swipeSpeed = (currentTipPosition - indexFingerStartPositions[handIndex]).magnitude / Time.deltaTime;
                    if (swipeSpeed > swipeSpeedThreshold)
                    {
                        Debug.Log($"{(handIndex == 0 ? "Right" : "Left")} Edge Left Swipe Detected");

                        // Reset start position for next swipe detection
                        indexFingerStartPositions[handIndex] = currentTipPosition;
                    }
                }
                // Detect edge rightward swipe
                else if (currentTipPosition.x > indexFingerStartPositions[handIndex].x + swipeThreshold)
                {
                    float swipeSpeed = (currentTipPosition - indexFingerStartPositions[handIndex]).magnitude / Time.deltaTime;
                    if (swipeSpeed > swipeSpeedThreshold)
                    {
                        Debug.Log($"{(handIndex == 0 ? "Right" : "Left")} Edge Right Swipe Detected");

                        // Reset start position for next swipe detection
                        indexFingerStartPositions[handIndex] = currentTipPosition;
                    }
                }
                else
                {
                    // Update start position if not swiping
                    indexFingerStartPositions[handIndex] = currentTipPosition;
                }
            }
        }
    }
}
