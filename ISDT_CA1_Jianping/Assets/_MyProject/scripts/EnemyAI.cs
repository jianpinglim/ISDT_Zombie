using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public int attackDamage = 10;  // Add this line
    private float nextAttackTime;

    [Header("Chase Settings")]
    private GameObject player;  // Changed to private since we'll find it automatically
    public float chaseRange = 10f;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("References")]
    private Animator animator;
    private NavMeshAgent m_agent;
    private Dissolver dissolver; // Add this reference


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
        dissolver = GetComponent<Dissolver>();

        if (m_agent == null)
        {
            Debug.LogError($"NavMeshAgent missing on {gameObject.name}!");
            return;
        }

        // Find player by tag
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Cannot find object with tag 'Player' in the scene!");
            return;
        }

        // Set up NavMeshAgent parameters
        m_agent.speed = moveSpeed;
        m_agent.stoppingDistance = attackRange; // Stop when within attack range
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead || player == null || m_agent == null) return;

        if (!animator.gameObject.activeSelf || !animator.enabled)
        {
            Debug.LogWarning($"Animator disabled on {gameObject.name}. Re-enabling...");
            animator.enabled = true;
        }

        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= chaseRange)
        {
            m_agent.SetDestination(player.transform.position);

            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
        }
        else
        {
            if (m_agent.hasPath)
            {
                m_agent.ResetPath();
            }
        }

        // Update animations last
        UpdateAnimationState();
    }

    public void InitializeAnimations()
    {
        if (animator != null)
        {
            // Reset all animation states
            animator.Rebind();
            animator.Update(0f);

            // Set initial state
            animator.SetBool(idleAnimParam, true);
            animator.SetBool(chaseAnimParam, false);
            animator.SetBool(attackAnimParam, false);
            animator.SetBool(dieAnimParam, false);
        }
    }



    private void AttackPlayer()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            // Try to get PlayerHealth component
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"Zombie dealt {attackDamage} damage to player!");
            }

            // Trigger attack animation
            animator.SetBool(attackAnimParam, true);
        }
    }

    private void ChasePlayer()
    {
        m_agent.isStopped = false;
        m_agent.SetDestination(player.transform.position);
        m_agent.speed = moveSpeed;

        Vector3 direction = (player.transform.position - transform.position).normalized;
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

    private void UpdateAnimationState()
    {
        if (animator == null) return;

        // Reset all states first
        animator.SetBool(idleAnimParam, false);
        animator.SetBool(chaseAnimParam, false);
        animator.SetBool(attackAnimParam, false);

        // Then set the appropriate state
        if (distanceToPlayer <= attackRange)
        {
            animator.SetBool(attackAnimParam, true);
        }
        else if (distanceToPlayer <= chaseRange)
        {
            animator.SetBool(chaseAnimParam, true);
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
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Prevent multiple calls
        ZombieKillsManager.ZombieKilled();

        isDead = true;

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

        if (dissolver != null)
        {
            StartCoroutine(dissolver.dissolver());
            Debug.Log("Started dissolve effect");
        }
        else
        {
            Debug.LogError("Dissolver component not found!");
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
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= chaseRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}