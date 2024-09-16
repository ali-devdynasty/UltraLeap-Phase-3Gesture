using UnityEngine;
using Leap;
using Leap.Unity;

public class IndexFingerExtendedSc : MonoBehaviour
{

    private Controller controller;
    private bool singleFingerPoseDetected = false;
    public GroupControllerPhase3 groupController;

    private void Awake()
    {
        groupController = FindObjectOfType<GroupControllerPhase3>();
    }

    void Start()
    {
        controller = new Controller();
    }

    void Update()
    {
        

        Frame frame = controller.Frame();
        bool currentSingleFingerPoseDetected = false;

        foreach (Hand hand in frame.Hands)
        {
            if (IsSingleTapPose(hand))
            {
                currentSingleFingerPoseDetected = true;
                if (!singleFingerPoseDetected && Time.timeScale !=0)
                {
                    Debug.Log("Single Tap (one-finger) detected");
                    singleFingerPoseDetected = true;
                    groupController.OnGestureDetected();
                }
            }
        }

        if (!currentSingleFingerPoseDetected)
        {
            singleFingerPoseDetected = false;
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


}
