using UnityEngine;
using TMPro;

public class TabletUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI leverCountText;
    private int totalLevers;

    void Start()
    {
        totalLevers = LeverTracker.totalLevers;
        UpdateLeverDisplay();

        // Subscribe to the event
        LeverTracker.onLeverPulled += UpdateLeverDisplay;
        Debug.Log("TabletUIManager started and subscribed to events");
    }

    void OnDestroy()
    {
        // Unsubscribe when destroyed
        LeverTracker.onLeverPulled -= UpdateLeverDisplay;
    }

    private void UpdateLeverDisplay()
    {
        if(leverCountText != null)
        {
            leverCountText.text = $"Levers Pulled: {LeverTracker.leversPulled}/{totalLevers}";
            Debug.Log($"UI Updated: {LeverTracker.leversPulled}/{totalLevers}");
        }
        else
        {
            Debug.LogError("leverCountText is null! Make sure it's assigned in the Inspector.");
        }
    }
}