using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
   public int maxHealth = 3;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is a zombie
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        currentHealth--;
        Debug.Log("Player hit! Remaining health: " + currentHealth);
        AudioManager.instance.PlaySfx("playerHurt");

        if (currentHealth <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        Debug.Log("Player has been defeated. Loading end scene...");
        SceneManager.LoadScene("EndScene");
    }
}
