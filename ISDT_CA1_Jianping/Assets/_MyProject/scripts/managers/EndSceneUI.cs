using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndSceneUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI zombieKillsText;
    [SerializeField] private TextMeshProUGUI leversPulledText;

    void Start()
    {
        zombieKillsText.text = $"Zombies Killed: {EndSceneData.zombiesKilled}";
        leversPulledText.text = $"Levers Pulled: {EndSceneData.leversPulled}";
    }

    public void backtomenu()
    {
        SceneManager.LoadScene("Menu");

    }
}