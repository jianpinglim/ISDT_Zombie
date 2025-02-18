using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    public UnityEvent onPlayerDeath;
    public UnityEvent<int> onHealthChanged;


    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        AudioManager.instance.PlaySfx("playerHurt");
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        onHealthChanged?.Invoke(currentHealth);
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        onPlayerDeath?.Invoke();

        // Store data to pass to EndScene
        EndSceneData.zombiesKilled = ZombieKillsManager.totalKills;
        EndSceneData.leversPulled = LeverTracker.leversPulled;

        // Optionally disable player input/interactions here

        // Load the EndScene
        SceneManager.LoadScene("EndScene");
    }


    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}

public static class EndSceneData
{
    public static int zombiesKilled;
    public static int leversPulled;
}