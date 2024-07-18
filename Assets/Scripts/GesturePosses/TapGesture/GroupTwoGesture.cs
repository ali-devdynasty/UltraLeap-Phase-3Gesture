using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class GroupTwoGesture : MonoBehaviour
{
    private LeapProvider provider;
    private Vector3 previousFingerPosition;
    private bool isSwipingUp;
    private bool isSwipingDown;
    public GroupController groupController;

    // Start is called before the first frame update
    void Start()
    {
        provider = FindObjectOfType<LeapProvider>();
        previousFingerPosition = Vector3.zero;
        isSwipingUp = false;
        isSwipingDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        Frame frame = provider.CurrentFrame;

        foreach (Hand hand in frame.Hands)
        {
            // Check if only the index finger is extended
            if (IsOnlyIndexFingerExtended(hand))
            {
                Finger indexFinger = hand.Fingers[1];
                Vector3 currentFingerPosition = indexFinger.TipPosition;

                // Check for upward swipe
                if (IsSwipingUp(currentFingerPosition))
                {
                    if (!isSwipingUp)
                    {
                        Debug.Log("Swipe detected: Upward");
                        isSwipingUp = true;
                        groupController.ShowGameOverPopup();

                    }
                }
                else
                {
                    isSwipingUp = false;
                }

                // Check for downward swipe
                if (IsSwipingDown(currentFingerPosition))
                {
                    if (!isSwipingDown)
                    {
                        Debug.Log("Swipe detected: Downward");
                        isSwipingDown = true;
                        groupController.ShowGameOverPopup();

                    }
                }
                else
                {
                    isSwipingDown = false;
                }

                previousFingerPosition = currentFingerPosition;
            }
            else
            {
                isSwipingUp = false;
                isSwipingDown = false;
            }
        }
    }

    private bool IsOnlyIndexFingerExtended(Hand hand)
    {
        // Check if only the index finger is extended
        return hand.Fingers[1].IsExtended && !hand.Fingers[0].IsExtended && !hand.Fingers[2].IsExtended && !hand.Fingers[3].IsExtended && !hand.Fingers[4].IsExtended;
    }

    private bool IsSwipingUp(Vector3 currentFingerPosition)
    {
        float swipeThreshold = 0.05f; // Adjust this threshold based on your needs
        return currentFingerPosition.y - previousFingerPosition.y > swipeThreshold;
    }

    private bool IsSwipingDown(Vector3 currentFingerPosition)
    {
        float swipeThreshold = 0.05f; // Adjust this threshold based on your needs
        return previousFingerPosition.y - currentFingerPosition.y > swipeThreshold;
    }
}
