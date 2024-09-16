using UnityEngine;
using Leap;
using Leap.Unity;

public class PinchLeftDragGesture : MonoBehaviour
{
    private Hand _hand;
    private Vector3 _previousPinchPosition;
    private bool _isPinching = false;
    private const float PinchStrengthThreshold = 0.8f;
    private const float DragThreshold = 0.05f;
    private const float PalmVelocityThreshold = -0.25f; // Threshold for leftward palm velocity (negative value)
    public GroupControllerPhase3 groupController;

    void Update()
    {
        // Get the first hand detected
        if (Hands.Provider.CurrentFrame.Hands.Count > 0)
        {
            _hand = Hands.Provider.CurrentFrame.Hands[0];

            if (_hand != null)
            {
                // Check if the hand is pinching with sufficient strength
                if (_hand.IsPinching() && _hand.PinchStrength > PinchStrengthThreshold)
                {
                    Vector3 currentPinchPosition = _hand.GetPinchPosition();
                    Vector3 palmVelocity = _hand.PalmVelocity;

                    if (!_isPinching)
                    {
                        // Start pinching
                        _isPinching = true;
                        _previousPinchPosition = currentPinchPosition;
                    }
                    else
                    {
                        // Check for leftward drag with a threshold and palm velocity
                        if (_previousPinchPosition.x - currentPinchPosition.x > DragThreshold && palmVelocity.x < PalmVelocityThreshold && Time.timeScale != 0)
                        {
                            Debug.Log("Pinch Leftward Drag detected");
                            groupController.OnGestureDetected();
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
}
