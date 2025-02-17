using UnityEngine;
using TMPro;

public class TabletUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI leverCountText;
    [SerializeField] private TextMeshProUGUI killCountText;

    private int totalLevers;

    void Start()
    {
        totalLevers = LeverTracker.totalLevers;
        UpdateLeverDisplay();

        // Subscribe to the event
        LeverTracker.onLeverPulled += UpdateLeverDisplay;
        Debug.Log("TabletUIManager started and subscribed to events");

        ZombieKillsManager.onZombieKilled += UpdateKillDisplay;
        UpdateKillDisplay();  // Show the initial 0 kills
    }

    void OnDestroy()
    {
        // Unsubscribe when destroyed
        LeverTracker.onLeverPulled -= UpdateLeverDisplay;
        ZombieKillsManager.onZombieKilled -= UpdateKillDisplay;

    }

    private void UpdateLeverDisplay()
    {
        if (leverCountText != null)
        {
            Debug.Log($"Updating UI Display. Current count: {LeverTracker.leversPulled}");
            leverCountText.text = $"Levers Pulled: {LeverTracker.leversPulled}/{totalLevers}";
            Debug.Log($"UI Updated: {leverCountText.text}");
        }
        else
        {
            Debug.LogError("leverCountText is null! Make sure it's assigned in the Inspector.");
        }
    }

    private void UpdateKillDisplay()
    {
        if (killCountText != null)
        {
            killCountText.text = $"Zombies Killed: {ZombieKillsManager.totalKills}";
        }
    }
}