using UnityEngine;
using System;
[Serializable]
public class SaveData
{
    public SerializableTransform playerTransform;
    public SerializableTransform tabletTransform;  // Add this line
    public int zombiesKilled;
    public int leversPulled;
    public string[] pulledLeverIds;
}

[Serializable]
public class SerializableTransform
{
    public float[] position;
    public float[] rotation;

    public SerializableTransform(Transform transform)
    {
        position = new float[3] { transform.position.x, transform.position.y, transform.position.z };
        rotation = new float[4] { transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w };
    }
}