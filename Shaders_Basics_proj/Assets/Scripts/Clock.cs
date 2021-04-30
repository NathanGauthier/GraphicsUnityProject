using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Clock : MonoBehaviour
{
   const float hoursToDegrees = -30f , minutesToDegrees = -6f, secondsToDegrees = -6f;
    [SerializeField]
    Transform HoursPivot = default, MinutesPivot, SecondsPivot;


    private void Update()
    {

        TimeSpan time = DateTime.Now.TimeOfDay;
        
         HoursPivot.localRotation = Quaternion.Euler(0f, 0f, hoursToDegrees * (float) time.TotalHours);
         MinutesPivot.localRotation = Quaternion.Euler(0f, 0f, minutesToDegrees * (float) time.TotalMinutes);
         SecondsPivot.localRotation = Quaternion.Euler(0f, 0f, secondsToDegrees * (float) time.TotalSeconds);
    }
}
