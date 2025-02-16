using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float chaseRange = 10f; // Range at which the enemy starts chasing
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
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
}