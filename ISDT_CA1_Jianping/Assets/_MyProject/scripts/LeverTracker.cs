using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LeverTracker : MonoBehaviour
{
    [Header("Lever Settings")]
    private new HingeJoint hingeJoint;
    public float pullThreshold = 45f;
    public static int leversPulled = 0;
    public static int totalLevers = 4;
    public static event System.Action onLeverPulled;
    public static HashSet<string> pulledLeverIds = new HashSet<string>();

    [Header("Lever Identity")]
    public string leverID;  // Unique ID for each lever
    private bool isPulled = false;

    [Header("Data Tracking")]
    public int leverNumber = 1;
    private float startTime;
    private float completionTime;
    private static List<LeverData> leverDataList = new List<LeverData>();
    private static bool gameStarted = false;

    void Start()
    {
        hingeJoint = GetComponent<HingeJoint>();
        
        if (!gameStarted)
        {
            gameStarted = true;
            startTime = Time.time;
        }

        // Check if this lever was previously pulled
        if (pulledLeverIds.Contains(leverID))
        {
            isPulled = true;
            // Set the lever to pulled position
            JointSpring spring = hingeJoint.spring;
            spring.targetPosition = pullThreshold;
            hingeJoint.spring = spring;
        }
    }

    void Update()
    {
        if (isPulled) return;

        float currentAngle = hingeJoint.angle;
        if (currentAngle >= pullThreshold)
        {
            OnLeverPulled();
        }
    }

    private void OnLeverPulled()
    {
        if (!isPulled && !pulledLeverIds.Contains(leverID))
        {
            // AudioManager.instance.PlaySfx("trainEngine");
            isPulled = true;
            leversPulled++;
            pulledLeverIds.Add(leverID);
            
            Debug.Log($"Lever {leverID} pulled. Total levers: {leversPulled}");
            onLeverPulled?.Invoke();

            // Record completion time and export data
            completionTime = Time.time - startTime;
            leverDataList.Add(new LeverData(leverNumber, completionTime));
            ExportToCSV();
        }
    }

    public static void ResetAllLevers()
    {
        leversPulled = 0;
        pulledLeverIds.Clear();
        gameStarted = false;
    }

    public static string[] GetPulledLeverIds()
    {
        return pulledLeverIds.ToArray();
    }

    public static void RestoreLeverStates(string[] pulledIds)
    {
        ResetAllLevers();
        if (pulledIds != null)
        {
            foreach (string id in pulledIds)
            {
                pulledLeverIds.Add(id);
            }
            leversPulled = pulledLeverIds.Count;
        }
    }

    private void ExportToCSV()
    {
        string dataPath = Path.Combine(Application.dataPath, "_MyProject", "LeverData");

        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        string filePath = Path.Combine(dataPath, "LeverResults.csv");
        bool fileExists = File.Exists(filePath);

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("Session Date,Lever Number,Completion Time (seconds)");
                }

                var latestData = leverDataList[leverDataList.Count - 1];
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                writer.WriteLine($"{timestamp},{latestData.leverNumber},{latestData.completionTime:F2}");
            }
            Debug.Log($"Data exported to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to export data: {e.Message}");
        }
    }

    public bool IsLeverPulled()
    {
        return isPulled;
    }
}

public class LeverData
{
    public int leverNumber;
    public float completionTime;

    public LeverData(int number, float time)
    {
        leverNumber = number;
        completionTime = time;
    }
}