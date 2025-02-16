using UnityEngine;

public class HandData : MonoBehaviour
{
    public enum HandModelType
    {
        Left,
        Right
    }

    public HandModelType handModelType;
    public Transform root;
    public Animator animator;
    public Transform[] fingerBones;
}
