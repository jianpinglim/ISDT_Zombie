using UnityEngine;

public class LeverTracker : MonoBehaviour
{
    private new HingeJoint hingeJoint;
    public float pullThreshold = 45f; // Angle at which we consider the lever "pulled"
    private bool isPulled = false;

    void Start()
    {
        hingeJoint = GetComponent<HingeJoint>();
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
        Debug.Log("Lever was pulled!");
        // Add your pulled logic here
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