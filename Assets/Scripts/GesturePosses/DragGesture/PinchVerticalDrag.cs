using Leap;
using Leap.Unity;
using UnityEngine;

public class PinchVerticalDrag : MonoBehaviour
{
    private Hand _hand;
    private Vector3 _previousPinchPosition;
    private bool _isPinching = false;
    private const float PinchStrengthThreshold = 0.8f;
    private const float DragThreshold = 0.05f;
    public GroupController groupController;
    void Update()
    {
        // Get the first hand detected

        //try getting the hand from the provider , first try for hands[0] , if not found then try for hands[1]

        if(Hands.Provider.CurrentFrame.Hands[0] != null)
        {
            _hand = Hands.Provider.CurrentFrame.Hands[0];
        }
        else if (Hands.Provider.CurrentFrame.Hands[1] != null)
        {
            _hand = Hands.Provider.CurrentFrame.Hands[1];
        }
        else
        {
            return;
        }

        if (_hand != null)
        {
            // Check if the hand is pinching with sufficient strength
            if (_hand.IsPinching() && _hand.PinchStrength > PinchStrengthThreshold)
            {
                Vector3 currentPinchPosition = _hand.GetPinchPosition();

                if (!_isPinching)
                {
                    // Start pinching
                    _isPinching = true;
                    _previousPinchPosition = currentPinchPosition;
                }
                else
                {
                    // Check for upward drag with a threshold
                    if (currentPinchPosition.y - _previousPinchPosition.y > DragThreshold)
                    {
                        Debug.Log("Pinch Upward Drag detected");
                        groupController.ShowGameOverPopup();
                    }
                    // Check for downward drag with a threshold
                    else if (_previousPinchPosition.y - currentPinchPosition.y > DragThreshold)
                    {
                        Debug.Log("Pinch Downward Drag detected");
                        groupController.ShowGameOverPopup();

                    }

                    // Update previous pinch position
                    _previousPinchPosition = Vector3.Lerp(_previousPinchPosition, currentPinchPosition, Time.deltaTime * 10);
                }
            }
            else
            {
                // Reset pinch state
                _isPinching = false;
            }
        }
    }
}


