using Leap;
using Leap.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
public class HandTrackingData : MonoBehaviour
{
    public LeapProvider leapProvider;

    public HandAndFingureMovement GetHandTrackingData(Chirality _hand)
    {
        HandAndFingureMovement TrackingData = new HandAndFingureMovement();

        if (Hands.Provider.GetHand(_hand) == null)
        {
            //Debug.Log("Hand Not Founded");
            return null;
        }
       
        Hand Hand = Hands.Provider.GetHand(_hand);
       

        DateTime currentDateTime = DateTime.Now;

        // Convert the DateTime to a string in a specific format
        string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        TrackingData.time = formattedDateTime;

        TrackingData.handPos = Hand.PalmPosition;
        TrackingData.handRotation = Hand.Rotation;
        TrackingData.pinchStrenght = Hand.PinchStrength;
        TrackingData.grabStrenght = Hand.GrabStrength;
        TrackingData.handVeclocity = Hand.PalmVelocity;

        List<Fingure> fingures = new List<Fingure>();
        for(int i = 0; i < 5; i++)
        {
            var finger = Hand.Fingers[i];
            Vector3 fingPos = finger.TipPosition;
            Vector3 direction = finger.Direction;
            bool isextended = finger.IsExtended;

            Fingure fing = new Fingure();
            
            fing.isExtended = isextended;
            fing.direction = direction;
            fing.fingurePos = fingPos;

            fingures.Add(fing);
        }

        TrackingData.finguePos = fingures;

        return TrackingData;
    }
}
