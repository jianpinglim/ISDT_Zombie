using UnityEngine;
using TMPro;

public class TabletUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI leverCountText;
    [SerializeField] private TextMeshProUGUI killCountText;
    private int totalLevers;


    void Start()
    {
        // Check if we're loading from save
        if (SaveDataHolder.shouldLoadSave && SaveDataHolder.savedData != null)
        {
            // Update the managers with saved data
            ZombieKillsManager.totalKills = SaveDataHolder.savedData.zombiesKilled;
            LeverTracker.leversPulled = SaveDataHolder.savedData.leversPulled;

            // Restore tablet position if it was saved
            if (SaveDataHolder.savedData.tabletTransform != null)
            {
                transform.position = new Vector3(
                    SaveDataHolder.savedData.tabletTransform.position[0],
                    SaveDataHolder.savedData.tabletTransform.position[1],
                    SaveDataHolder.savedData.tabletTransform.position[2]
                );
                transform.rotation = new Quaternion(
                    SaveDataHolder.savedData.tabletTransform.rotation[0],
                    SaveDataHolder.savedData.tabletTransform.rotation[1],
                    SaveDataHolder.savedData.tabletTransform.rotation[2],
                    SaveDataHolder.savedData.tabletTransform.rotation[3]
                );
                Debug.Log($"Restored tablet position to: {transform.position}");
            }
        }

        InitializeUI();
    }

    void OnEnable()
    {
        // Subscribe to events when enabled
        LeverTracker.onLeverPulled += UpdateLeverDisplay;
        ZombieKillsManager.onZombieKilled += UpdateKillDisplay;
    }

    void OnDisable()
    {
        // Unsubscribe from events when disabled
        LeverTracker.onLeverPulled -= UpdateLeverDisplay;
        ZombieKillsManager.onZombieKilled -= UpdateKillDisplay;
    }

    private void InitializeUI()
    {
        totalLevers = LeverTracker.totalLevers;

        // Update both displays immediately
        UpdateLeverDisplay();
        UpdateKillDisplay();

        Debug.Log($"TabletUI initialized - Levers: {LeverTracker.leversPulled}/{totalLevers}, Kills: {ZombieKillsManager.totalKills}");
    }

    private void UpdateLeverDisplay()
    {
        if (leverCountText != null)
        {
            leverCountText.text = $"Levers Pulled: {LeverTracker.leversPulled}/{totalLevers}";
            Debug.Log($"Lever UI Updated: {leverCountText.text}");
        }
    }

    private void UpdateKillDisplay()
    {
        if (killCountText != null)
        {
            killCountText.text = $"Zombies Killed: {ZombieKillsManager.totalKills}";
            Debug.Log($"Kill UI Updated: {killCountText.text}");
        }
    }
}