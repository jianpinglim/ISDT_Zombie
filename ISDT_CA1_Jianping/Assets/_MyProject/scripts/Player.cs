using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private const string MENU_SCENE = "Menu";
    private const string PLAY_SCENE = "PlayScene";

    void Start()
    {
        if (SaveDataHolder.shouldLoadSave && SaveDataHolder.savedData != null)
        {
            LoadSavedState(SaveDataHolder.savedData);
        }
    }

    private void LoadSavedState(SaveData saveData)
    {
        Vector3 newPosition = new Vector3(
            saveData.playerTransform.position[0],
            saveData.playerTransform.position[1],
            saveData.playerTransform.position[2]
        );

        // Get the root XR Rig transform
        Transform xrRigRoot = transform.root;
        xrRigRoot.position = newPosition;
        Debug.Log($"XR Rig Root position set to: {newPosition}");

        xrRigRoot.rotation = new Quaternion(
            saveData.playerTransform.rotation[0],
            saveData.playerTransform.rotation[1],
            saveData.playerTransform.rotation[2],
            saveData.playerTransform.rotation[3]
        );

        ZombieKillsManager.totalKills = saveData.zombiesKilled;
        LeverTracker.RestoreLeverStates(saveData.pulledLeverIds);

        SaveDataHolder.shouldLoadSave = false;
        SaveDataHolder.savedData = null;
    }

    public void LoadGame()
    {
        Debug.Log("Attempting to load saved game...");
        SaveData saveData = SaveManager.LoadGame();

        if (saveData != null)
        {
            Debug.Log($"Loading saved position: ({saveData.playerTransform.position[0]}, {saveData.playerTransform.position[1]}, {saveData.playerTransform.position[2]})");

            // Store the data before scene load
            SaveDataHolder.savedData = saveData;
            SaveDataHolder.shouldLoadSave = true;

            // Load scene - this will trigger Start() in the new scene
            // which will then call LoadSavedState with the stored data
            SceneManager.LoadScene(PLAY_SCENE);
        }
        else
        {
            Debug.LogWarning("No save file found!");
        }
    }

    public void SaveAndQuit()
    {
        SaveManager.SaveGame(this);
        Debug.Log("Game saved successfully");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}

public static class SaveDataHolder
{
    public static SaveData savedData;
    public static bool shouldLoadSave;
}