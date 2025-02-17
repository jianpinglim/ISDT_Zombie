using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Chase Settings")]
    public Transform player;
    public float chaseRange = 10f;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("References")]
    private Animator animator;
    private NavMeshAgent m_agent;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    private float nextAttackTime;

    [Header("Animation")]
    [SerializeField] private string chaseAnimParam = "IsChasing";
    [SerializeField] private string idleAnimParam = "IsIdle";
    [SerializeField] private string attackAnimParam = "IsAttacking";
    [SerializeField] private string dieAnimParam = "IsDead";
    private bool isDead = false;
    private float distanceToPlayer;

    void Start()
    {
        // Initialize components
        animator = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();

        if (m_agent == null)
        {
            Debug.LogError($"NavMeshAgent missing on {gameObject.name}!");
            return;
        }

        if (player == null)
        {
            // Try to find player in scene
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("Player reference not set and cannot be found!");
                return;
            }
        }

        m_agent.speed = moveSpeed;
        m_agent.angularSpeed = rotationSpeed * 100;
        m_agent.acceleration = 8f;
        m_agent.stoppingDistance = 2f;

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead || player == null || m_agent == null) return;

        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            StopChasing();
        }
    }

    private void AttackPlayer()
    {
        // Stop moving when attacking
        m_agent.isStopped = true;
        m_agent.velocity = Vector3.zero;

        // Face the player while attacking
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // Only attack if cooldown has passed
        if (Time.time >= nextAttackTime)
        {
            // Trigger attack animation
            animator.SetBool(chaseAnimParam, false);
            animator.SetBool(idleAnimParam, false);
            animator.SetBool(attackAnimParam, true);

            nextAttackTime = Time.time + attackCooldown;
            Debug.Log($"Zombie attacking! Animation param: {attackAnimParam}");
        }
        else
        {
            // Ensure attack animation is off if not attacking
            animator.SetBool(attackAnimParam, false);
        }
    }

    private void ChasePlayer()
    {
        m_agent.isStopped = false;
        m_agent.SetDestination(player.position);
        m_agent.speed = moveSpeed;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // Reset attack animation and set chase animation
        animator.SetBool(attackAnimParam, false);
        SetAnimationState(true);
    }

    private void StopChasing()
    {
        m_agent.isStopped = true;

        // Reset all animations except idle
        animator.SetBool(attackAnimParam, false);
        SetAnimationState(false);
    }

    private void UpdateAnimation()
    {
        // Check if the agent is actually moving
        float speed = m_agent.velocity.magnitude;
        bool isMoving = speed > 0.1f;

        // Update both animation states
        SetAnimationState(isMoving);
    }

    private void SetAnimationState(bool isChasing)
    {
        animator.SetBool(chaseAnimParam, isChasing);

        if (isChasing)
        {
            animator.SetBool(idleAnimParam, false);
        }
        else
        {
            animator.SetBool(idleAnimParam, true);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log($"Enemy took {damageAmount} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Prevent multiple calls

        isDead = true;
        Debug.Log("Enemy died");

        // Disable NavMeshAgent
        if (m_agent != null)
        {
            m_agent.enabled = false;
        }

        // Disable Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Make it kinematic so it doesn't fall
            rb.linearVelocity = Vector3.zero; // Stop any existing motion
            rb.detectCollisions = false; // Disable collision detection

        }

        // Trigger dying animation
        if (animator != null)
        {
            animator.SetBool(dieAnimParam, true);
            Debug.Log($"Set {dieAnimParam} to true");
        }
        else
        {
            Debug.LogError("Animator is null!");
        }

        // Destroy object after a delay
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // Wait for the animation to finish (adjust the time as needed)
        yield return new WaitForSeconds(3.2f);

        // Destroy object
        Destroy(gameObject);
    }

    // Visual debugging
    private void OnDrawGizmosSelected()
    {
        // Draw chase range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Draw line to player if within range
        if (player != null && Vector3.Distance(transform.position, player.position) <= chaseRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}