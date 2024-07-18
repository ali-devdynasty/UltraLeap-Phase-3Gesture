using UnityEngine;
using Leap;
using Leap.Unity;

public class GroupOneGesture : MonoBehaviour
{ 

    private Controller controller;
    private bool singleFingerPoseDetected = false;
    private bool twoFingerPoseDetected = false;
    public GroupController groupController; 


    void Start()
    {
        controller = new Controller();
    }

    void Update()
    {
        Frame frame = controller.Frame();
        bool currentSingleFingerPoseDetected = false;
        bool currentTwoFingerPoseDetected = false;

        foreach (Hand hand in frame.Hands)
        {
            if (IsSingleTapPose(hand))
            {
                currentSingleFingerPoseDetected = true;
                if (!singleFingerPoseDetected)
                {
                    Debug.Log("Single Tap (one-finger) detected");
                    singleFingerPoseDetected = true;
                    groupController.ShowGameOverPopup();
                }
            }

            if (IsTwoFingerTapPose(hand))
            {
                currentTwoFingerPoseDetected = true;
                if (!twoFingerPoseDetected)
                {
                    Debug.Log("Single Tap (two-finger) detected");
                    twoFingerPoseDetected = true;
                    groupController.ShowGameOverPopup();


                }
            }
        }

        if (!currentSingleFingerPoseDetected)
        {
            singleFingerPoseDetected = false;
        }

        if (!currentTwoFingerPoseDetected)
        {
            twoFingerPoseDetected = false;
        }
    }

    bool IsSingleTapPose(Hand hand)
    {
        Finger indexFinger = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
        if (!indexFinger.IsExtended)
            return false;

        for (int i = 0; i < hand.Fingers.Count; i++)
        {
            if (i != (int)Finger.FingerType.TYPE_INDEX && hand.Fingers[i].IsExtended)
            {
                return false;
            }
        }

        Vector3 indexDirection = indexFinger.Direction;
        Vector3 handDirection = hand.Direction;
        return Vector3.Angle(indexDirection, handDirection) < 20.0f;
    }

    bool IsTwoFingerTapPose(Hand hand)
    {
        // Check if the thumb and index finger are extended
        Finger thumb = hand.Fingers[(int)Finger.FingerType.TYPE_THUMB];
        Finger indexFinger = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
        if (!thumb.IsExtended || !indexFinger.IsExtended)
            return false;

        // Check if other fingers are not extended
        for (int i = 0; i < hand.Fingers.Count; i++)
        {
            if (i != (int)Finger.FingerType.TYPE_THUMB && i != (int)Finger.FingerType.TYPE_INDEX && hand.Fingers[i].IsExtended)
            {
                return false;
            }
        }

        // Check if the thumb and index finger are spread out
        float distance = Vector3.Distance(thumb.TipPosition, indexFinger.TipPosition);
        return distance > 0.05f; // Adjust the threshold based on your needs
    }

   
}
