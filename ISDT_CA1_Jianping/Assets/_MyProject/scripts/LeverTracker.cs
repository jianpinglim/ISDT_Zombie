using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LeverTracker : MonoBehaviour
{
    //initialize the hinge joint and conditons for the lever
    private new HingeJoint hingeJoint;
    public float pullThreshold = 45f;
    public static int leversPulled = 0;
    public static event System.Action onLeverPulled;
    private bool isPulled = false;

    // Data structure to store lever data from labtask
    [Header("Tracking Settings")]
    public int leverNumber = 1; // Assign different numbers for each lever
    public static int totalLevers = 4;
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
    }

    void Update()
    {
        // Get the current angle of the hinge
        float currentAngle = hingeJoint.angle;

        // Check if lever state has changed
        bool wasPulled = isPulled;
        isPulled = currentAngle >= pullThreshold;

        // If state changed, you can trigger events
        if (wasPulled != isPulled)
        {
            if (isPulled)
            {
                OnLeverPulled();
            }
            else
            {
                OnLeverReleased();
            }
        }
    }

    private void OnLeverPulled()
    {
        if (!isPulled)  // Only increment if this lever hasn't been pulled before
        {
            leversPulled++;
            Debug.Log($"Lever pulled. Total levers: {leversPulled}");
            onLeverPulled?.Invoke();  // Make sure this is being called
        }

        completionTime = Time.time - startTime;
        Debug.Log($"Lever {leverNumber} was pulled! Time: {completionTime:F2} seconds");

        // Record and export data immediately for each lever pull
        leverDataList.Add(new LeverData(leverNumber, completionTime));
        ExportToCSV();
    }

    // Add this method to reset the counter
    public static void ResetLeverCount()
    {
        leversPulled = 0;
    }


    private void ExportToCSV()
    {
        Debug.Log("Exporting lever data to CSV...");
        // Use Application.dataPath to get the Assets folder path
        string dataPath = Path.Combine(Application.dataPath, "_MyProject", "LeverData");

        Debug.Log($"Attempting to save to: {dataPath}");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
            Debug.Log($"Created directory at: {dataPath}");
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
                    Debug.Log("Created new CSV file with headers");
                }

                // Write only the most recent lever pull
                var latestData = leverDataList[leverDataList.Count - 1];
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                writer.WriteLine($"{timestamp},{latestData.leverNumber},{latestData.completionTime:F2}");
            }
            Debug.Log($"Data exported successfully to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to export data: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }

    private void OnLeverReleased()
    {
        Debug.Log("Lever was released!");
        // Add your release logic here
    }

    // Public method to check lever state
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