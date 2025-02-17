using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class LevelManager : Singleton<LevelManager>
{
    private string currentSceneName;

    private void Start()
    {
        LoadSceneAdditively("Menu"); // Load Menu when game starts
    }

    // Load a scene additively and set it as the active scene once loaded
    public void LoadSceneAdditively(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Load the scene additively
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadOperation.completed += OnLoadOperationComplete;
        
        currentSceneName = sceneName; // Store the name of the scene to track the current scene
        yield return null;
    }

    private void OnLoadOperationComplete(AsyncOperation obj)
    {
        // Set the new scene as the active scene
        Scene loadedScene = SceneManager.GetSceneByName(currentSceneName);
        SceneManager.SetActiveScene(loadedScene);

        // Find the PersistentXROrigin and set its position to the spawn point
        PersistentXROrigin xrRig = PersistentXROrigin.Instance;
        if (xrRig != null)
        {
            Transform spawnPoint = FindSceneSpawnPoint(); // Locate the spawn point
            if (spawnPoint != null)
            {
                xrRig.transform.position = spawnPoint.position;
                xrRig.transform.rotation = spawnPoint.rotation;
                Debug.Log($"XR Rig position set to {spawnPoint.position}");
            }
            else
            {
                Debug.LogWarning("No SpawnPoint found in the scene!");
            }
        }
    }

    // Helper method to find a spawn point in the new scene
    private Transform FindSceneSpawnPoint()
    {
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint"); // Tag your spawn points as "SpawnPoint"
        return spawnPoint != null ? spawnPoint.transform : null;
    }




    // Unload the current scene and load MenuScene additively
    public void BackToMenu()
    {
        if (!string.IsNullOrEmpty(currentSceneName) && SceneManager.GetSceneByName(currentSceneName).isLoaded)
        {
            UnloadScene(currentSceneName);
            LoadSceneAdditively("Menu");
        }
    }

    // Unload a scene
    public void UnloadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
