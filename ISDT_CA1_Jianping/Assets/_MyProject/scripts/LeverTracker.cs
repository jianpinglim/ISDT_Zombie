using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LeverTracker : MonoBehaviour
{
    //initialize the hinge joint and conditons for the lever
    private new HingeJoint hingeJoint;
    public float pullThreshold = 45f;
    private bool isPulled = false;

    // Data structure to store lever data from labtask
    [Header("Tracking Settings")]
    public int leverNumber = 1; // Assign different numbers for each lever
    public static int totalLevers = 3;
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
        completionTime = Time.time - startTime;
        Debug.Log($"Lever {leverNumber} was pulled! Time: {completionTime:F2} seconds");

        // Record the lever pull data
        leverDataList.Add(new LeverData(leverNumber, completionTime));

        // If all levers are pulled, export the data
        if (leverDataList.Count >= totalLevers)
        {
            ExportToCSV();
        }
    }

    private void ExportToCSV()
    {
        Debug.Log("Exporting lever data to CSV...");
        string dataPath = Path.Combine(Application.persistentDataPath, "LeverData");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        string filePath = Path.Combine(dataPath, "LeverResults.csv");
        bool fileExists = File.Exists(filePath);

        try
        {
            // If file doesn't exist, create it with headers
            // If it exists, append new data
            using (StreamWriter writer = new StreamWriter(filePath, true)) // true for append mode
            {
                // Write headers only if file is new
                if (!fileExists)
                {
                    writer.WriteLine("Session Date,Lever Number,Completion Time (seconds)");
                }

                // Add timestamp for this session's data
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Write data for each lever
                foreach (var data in leverDataList)
                {
                    writer.WriteLine($"{timestamp},{data.leverNumber},{data.completionTime:F2}");
                }

                writer.WriteLine(); // Add blank line between sessions
            }
            Debug.Log($"Data exported successfully to: {filePath}");

            // Clear the list after successful export
            leverDataList.Clear();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to export data: {e.Message}");
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