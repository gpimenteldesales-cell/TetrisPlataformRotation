using UnityEngine;

public class MobileSettings : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}