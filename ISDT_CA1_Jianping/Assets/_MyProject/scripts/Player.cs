using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform defaultSpawnPoint; // Reference to spawn point in scene

    void Start()
    {
        if (SaveDataHolder.shouldLoadSave && SaveDataHolder.savedData != null)
        {
            Debug.Log("Loading saved game state...");
            // Restore game state
            ZombieKillsManager.totalKills = SaveDataHolder.savedData.zombiesKilled;
            LeverTracker.leversPulled = SaveDataHolder.savedData.leversPulled;

            // Restore position
            Vector3 savedPosition = new Vector3(
                SaveDataHolder.savedData.playerTransform.position[0],
                SaveDataHolder.savedData.playerTransform.position[1],
                SaveDataHolder.savedData.playerTransform.position[2]
            );

            if (savedPosition.y > -100)
            {
                transform.position = savedPosition;
                transform.rotation = new Quaternion(
                    SaveDataHolder.savedData.playerTransform.rotation[0],
                    SaveDataHolder.savedData.playerTransform.rotation[1],
                    SaveDataHolder.savedData.playerTransform.rotation[2],
                    SaveDataHolder.savedData.playerTransform.rotation[3]
                );
            }

            // Reset the load flag
            SaveDataHolder.shouldLoadSave = false;
            SaveDataHolder.savedData = null;
        }
        else
        {
            Debug.Log("Starting new game...");
            // Reset game state for new game
            ZombieKillsManager.totalKills = 0;
            LeverTracker.leversPulled = 0;

            // Use spawn point
            GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
            if (spawnPoint != null)
            {
                transform.position = spawnPoint.transform.position;
                transform.rotation = spawnPoint.transform.rotation;
            }
        }
    }

    public void SaveAndQuit()
    {
        SaveManager.SaveGame(this);
    }

    public void LoadGame()
    {
        Debug.Log("Loading saved game...");
        SaveData saveData = SaveManager.LoadGame();
        if (saveData != null)
        {
            // Store save data in static class for access after scene load
            SaveDataHolder.savedData = saveData;
            SaveDataHolder.shouldLoadSave = true;

            // Load game scene
            SceneManager.LoadScene("Game"); // Replace "Game" with your actual game scene name
        }
    }
}

public static class SaveDataHolder
{
    public static SaveData savedData;
    public static bool shouldLoadSave;
}