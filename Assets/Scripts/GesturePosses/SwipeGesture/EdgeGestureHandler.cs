using UnityEngine;
using Leap;
using Leap.Unity;

public class EdgeGestureHandler : MonoBehaviour
{
    private Hand _hand;
    private Vector3 _previousHandPosition;
    private bool _isSwiping = false;
    private const float SwipeThreshold = 0.2f; // Adjust this value based on your needs
    private const float EdgeThreshold = 0.8f; // Adjust this value based on your needs
    private const float MinDistanceThreshold = 0.1f; // Minimum distance from the device to consider the hand

    void Update()
    {
        // Get the first hand detected
        if (Hands.Provider.CurrentFrame.Hands.Count > 0)
        {
            _hand = Hands.Provider.CurrentFrame.Hands[0];

            if (_hand != null)
            {
                // Manually convert Leap Motion Vector to Unity Vector3
                Vector3 currentHandPosition = new Vector3(_hand.PalmPosition.x, _hand.PalmPosition.y, _hand.PalmPosition.z);

                // Check if the hand is within the minimum distance threshold
                if (currentHandPosition.z <= MinDistanceThreshold)
                {
                    // Check if the hand is near the edge of the detection area
                    if (Mathf.Abs(currentHandPosition.x) > EdgeThreshold)
                    {
                        if (!_isSwiping)
                        {
                            // Start swiping
                            _isSwiping = true;
                            _previousHandPosition = currentHandPosition;
                        }
                        else
                        {
                            // Check for leftward swipe
                            if (_previousHandPosition.x - currentHandPosition.x > SwipeThreshold)
                            {
                                Debug.Log("Edge Left Swipe detected at position: " + currentHandPosition);
                                _isSwiping = false; // Reset swipe state
                            }
                            // Check for rightward swipe
                            else if (currentHandPosition.x - _previousHandPosition.x > SwipeThreshold)
                            {
                                Debug.Log("Edge Right Swipe detected at position: " + currentHandPosition);
                                _isSwiping = false; // Reset swipe state
                            }

                            // Update previous hand position
                            _previousHandPosition = currentHandPosition;
                        }
                    }
                    else
                    {
                        // Reset swipe state if hand is not near the edge
                        _isSwiping = false;
                    }
                }
            }
        }
    }
}





