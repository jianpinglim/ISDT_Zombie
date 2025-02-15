using UnityEditor;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    //menuControl
    [SerializeField] private string menuSceneName; // Reference to the Menu scene name

    public void OnLoadSceneButtonClicked(string sceneName)
    {
        LevelManager.Instance.LoadSceneAdditively(sceneName);
        LevelManager.Instance.UnloadScene(menuSceneName); // Unload the Menu scene using the serialized variable
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
