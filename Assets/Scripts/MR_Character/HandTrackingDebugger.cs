using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class HandTrackingDebugger : MonoBehaviour
{
    static List<XRHandSubsystem> s_Subsystems = new List<XRHandSubsystem>();

    void Update()
    {
        s_Subsystems.Clear();
        SubsystemManager.GetSubsystems(s_Subsystems);
        if (s_Subsystems.Count == 0)
        {
            if (Time.frameCount % 100 == 0) // Reduce log spam
                Debug.LogWarning("HandTrackingDebugger: No XRHandSubsystem found!");
        }
        else
        {
            foreach (var subsystem in s_Subsystems)
            {
                if (Time.frameCount % 100 == 0)
                    Debug.Log($"HandTrackingDebugger: Subsystem is running: {subsystem.running}");
            }
        }
    }
}
