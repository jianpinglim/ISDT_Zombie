using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float chaseRange = 10f; // Range at which the enemy starts chasing
    public int maxHealth = 100; // Maximum health of the enemy
    private int currentHealth; // Current health of the enemy
    private Animator animator;
    private bool isDead = false; // Track if the enemy is dead
    private NavMeshAgent m_agent;  
    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth; // Initialize health
        m_agent = GetComponent<NavMeshAgent>();  // Add this line
    }

    void Update()
    {
        if (isDead) return; // Stop all behavior if the enemy is dead

        // Calculate the distance between the enemy and the player
        // Check if the player is within chase range
        if (m_agent.velocity.magnitude != 0)
        {
            animator.SetBool("IsChasing", true);
        }
        else
        {
            animator.SetBool("IsChasing", false);
        }
    }

    void OnAnimatorMove()
    {
        if(animator.GetBool("IsChasing"))
        {
            m_agent.speed = (animator.deltaPosition / Time.deltaTime).magnitude;
        }
    }

    // Method to handle taking damage
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return; // If already dead, do nothing

        currentHealth -= damageAmount; // Reduce health
        Debug.Log($"Enemy took {damageAmount} damage. Current health: {currentHealth}");

        // Check if the enemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to handle enemy death
    private void Die()
    {
        isDead = true;
        Debug.Log("Enemy died");

        // Disable chasing behavior
        animator.SetBool("IsChasing", false);

        // Get the DissolveEffect component and start the dissolve
    }

}