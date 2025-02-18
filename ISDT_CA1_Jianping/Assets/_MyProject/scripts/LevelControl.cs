using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private string playSceneName = "PlayScene";

    public void OnLoadSceneButtonClicked(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // Get current scene
        Scene currentScene = SceneManager.GetActiveScene();

        // Load new scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log($"Loaded scene: {sceneName}");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}