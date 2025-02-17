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
    private DissolveEffect dissolveEffect;
    [Header("Animation")]
    [SerializeField] private string chaseAnimParam = "IsChasing";
    [SerializeField] private string idleAnimParam = "IsIdle";
    [SerializeField] private float animationDampTime = 0.1f;

    private bool isDead = false;
    private float distanceToPlayer;

    void Start()
    {
        // Initialize components
        animator = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
        dissolveEffect = GetComponent<DissolveEffect>();

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
        m_agent.stoppingDistance = 2f; // Add stopping distance

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead || player == null || m_agent == null) return;

        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            StopChasing();
        }

        UpdateAnimation();
    }

    private void ChasePlayer()
    {
        m_agent.isStopped = false;
        m_agent.SetDestination(player.position);

        // Ensure speed is maintained
        m_agent.speed = moveSpeed;

        // Face the target
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        SetAnimationState(true);
    }

    private void StopChasing()
    {
        m_agent.isStopped = true;
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
        isDead = true;
        Debug.Log("Enemy died");

        // Disable NavMeshAgent and physics
        m_agent.enabled = false;
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().enabled = false;
        }

        // Stop animations
        animator.SetBool("IsChasing", false);

        // Start dissolve effect if available
        if (dissolveEffect != null)
        {
            dissolveEffect.StartDissolve();
            // Destroy object after dissolve effect
            Destroy(gameObject, dissolveEffect.dissolveSpeed + 0.1f);
        }
        else
        {
            // If no dissolve effect, destroy immediately
            Destroy(gameObject);
        }
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
    }
}