using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float chaseRange = 10f; // Range at which the enemy starts chasing
    public int maxHealth = 100; // Maximum health of the enemy
    private int currentHealth; // Current health of the enemy
    private Animator animator;
    private Material enemyMaterial; // Material using the dissolve shader
    private bool isDead = false; // Track if the enemy is dead

    [Header("Dissolve Settings")]
    public float dissolveSpeed = 1f; // Speed of the dissolve effect

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth; // Initialize health

        // Get the material using the dissolve shader
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            enemyMaterial = renderer.material;
        }
        else
        {
            Debug.LogError("No renderer found on the enemy!");
        }
    }

    void Update()
    {
        if (isDead) return; // Stop all behavior if the enemy is dead

        // Calculate the distance between the enemy and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is within chase range
        if (distanceToPlayer <= chaseRange)
        {
            animator.SetBool("IsChasing", true); // Transition to chasing animation
        }
        else
        {
            animator.SetBool("IsChasing", false); // Transition back to idle animation
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

        // Start the dissolve effect
        StartCoroutine(DissolveEffect());
    }

    // Coroutine to handle the dissolve effect
    private IEnumerator DissolveEffect()
    {
        float dissolveProgress = 0f;

        while (dissolveProgress < 1f)
        {
            dissolveProgress += Time.deltaTime * dissolveSpeed;
            enemyMaterial.SetFloat("_DissolveThreshold", dissolveProgress); // Update the dissolve threshold
            yield return null;
        }

        // Fully dissolve the enemy
        enemyMaterial.SetFloat("_DissolveThreshold", 1f);

        // Destroy the enemy object after the dissolve effect is complete
        Destroy(gameObject);
    }
}