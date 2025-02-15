using UnityEngine;

public class BackBtn : MonoBehaviour
{
    public void OnBackButtonClicked()
    {
        // Calls the BackToMenu function in LevelManager to return to the Menu scene
        LevelManager.Instance.BackToMenu();
    }
}
