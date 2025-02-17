using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private const string MENU_SCENE = "Menu";
    private const string PLAY_SCENE = "PlayScene";
    [SerializeField] private Transform defaultSpawnPoint;

    void Start()
    {
        // Make XR Rig persistent if not already
        if (transform.root.gameObject.scene.name != "DontDestroyOnLoad")
        {
            DontDestroyOnLoad(transform.root.gameObject);
        }

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
        // Save the game data
        SaveManager.SaveGame(this);
        Debug.Log("Game saved successfully");

        // Quit the game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadGame()
    {
        Debug.Log("Attempting to load saved game...");
        SaveData saveData = SaveManager.LoadGame();

        if (saveData != null)
        {
            SaveDataHolder.savedData = saveData;
            SaveDataHolder.shouldLoadSave = true;
            SceneManager.LoadScene(PLAY_SCENE);
        }
        else
        {
            Debug.LogWarning("No save file found!");
        }
    }

    public void StartNewGame()
    {
        Debug.Log("Starting new game...");
        // Reset all game state
        ZombieKillsManager.totalKills = 0;
        LeverTracker.ResetAllLevers();

        // Use spawn point for new game
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
        }

        SaveDataHolder.shouldLoadSave = false;
        SaveDataHolder.savedData = null;

        // Load play scene
        SceneManager.LoadScene(PLAY_SCENE);
    }

}

public static class SaveDataHolder
{
    public static SaveData savedData;
    public static bool shouldLoadSave;
}