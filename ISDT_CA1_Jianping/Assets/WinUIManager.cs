using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUIManager : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
