using UnityEngine;
using System.Collections;

public class PerformanceMeter : MonoBehaviour
{
    public static string levelGenLog = "";
    public static float lastTime = 0F;

    //Time Logging
    public static void ResetLog()
    {
        levelGenLog = "";
    }

    public static void StartTimeLog(string message)
    {
        lastTime = Time.realtimeSinceStartup;
        levelGenLog += message;
    }

    public static void EndTimeLog()
    {
        levelGenLog += (Time.realtimeSinceStartup - lastTime).ToString("F4") + "\n";
    }

    public static void AddLog(string message)
    {
        levelGenLog += message + "\n";
    }

    public static string GetLog()
    {
        return levelGenLog;
    }
}