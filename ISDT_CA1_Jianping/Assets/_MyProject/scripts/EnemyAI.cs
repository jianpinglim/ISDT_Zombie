using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int Health = 20;
    private bool isDead = false;

    [Header("Target and Distances")]
    public LayerMask PlayerLayer;         // Layer assigned to the player
    public float DetectionRange = 30f;   // How far the enemy can detect the player
    public float AttackDistance = 2f;   // How close before attacking
    private Transform Target;

    [Header("Optional Visualization")]
    public bool ShowDetectionRange = true; // For debugging - shows detection radius in editor

    // Private variables
    private Animator m_Animator;
    private NavMeshAgent m_Agent;
    private float m_Distance;

    // Animation parameter names
    private readonly string IS_WALKING = "IsWalking";
    private readonly string IS_ATTACKING = "IsAttacking";
    private readonly string IS_IDLE = "IsIdle";
    private readonly string IS_DEAD = "IsDead";

    private bool isTransitioning = false;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        if (m_Animator == null)
        {
            Debug.LogError("Animator component missing from enemy!");
        }

        // Dynamically find the player at the start
        FindPlayer();

        // Start in idle state
        SetIdleState();
    }

    private void Update()
    {
        if (isDead) return;

        if (Target == null)
        {
            // Continuously attempt to find the player if not already set
            FindPlayer();
            return;
        }

        m_Distance = Vector3.Distance(Target.position, transform.position);

        // Check if player is within detection range
        if (m_Distance <= DetectionRange)
        {
            // Player detected - check if in attack range
            if (m_Distance <= AttackDistance)
            {
                SetAttackState();
            }
            else
            {
                SetWalkingState();
            }
        }
        else
        {
            // Player out of detection range - return to idle
            SetIdleState();
        }
    }

    private void FindPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, DetectionRange, PlayerLayer);

        if (hits.Length > 0)
        {
            // Set the first player found as the target
            Target = hits[0].transform;
            Debug.Log("Player found!");
        }
        else
        {
            Debug.LogWarning("No player found within detection range.");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        Health -= damage;
        Debug.Log($"Enemy hit! Remaining health: {Health}");

        if (Health <= 0)
        {
            Die();
        }
        AudioManager.instance.PlaySfx("Zombie Die");
    }

    private void Die()
    {

        if (isDead) return;
        isDead = true;
        Debug.Log("Enemy died!");
        AudioManager.instance.PlaySfx("Zombie Die");


        // Stop NavMeshAgent
        if (m_Agent != null)
        {
            m_Agent.isStopped = true;
            m_Agent.enabled = false;
        }

        if (m_Animator != null)
        {
            // Reset all other animation states
            m_Animator.SetBool(IS_WALKING, false);
            m_Animator.SetBool(IS_ATTACKING, false);
            m_Animator.SetBool(IS_IDLE, false);
            m_Animator.SetBool(IS_DEAD, true);

            // Start coroutine to handle destruction
            StartCoroutine(WaitForDeathAnimation());
        }
        else
        {
            Debug.Log("No animator found, destroying immediately.");
            Destroy(gameObject);
        }
    }

    private IEnumerator WaitForDeathAnimation()
    {
        float animationLength = m_Animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animationLength);

        Destroy(gameObject);
    }

    private void SetIdleState()
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionToIdle());
    }

    private IEnumerator TransitionToIdle()
    {
        isTransitioning = true;
        m_Agent.isStopped = true;
        m_Animator.SetBool(IS_WALKING, false);
        m_Animator.SetBool(IS_ATTACKING, false);
        m_Animator.SetBool(IS_IDLE, true);
        yield return null;
        isTransitioning = false;
    }

    private void SetWalkingState()
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionToWalking());
    }

    private IEnumerator TransitionToWalking()
    {
        isTransitioning = true;
        m_Agent.isStopped = false;
        m_Agent.destination = Target.position;
        m_Animator.SetBool(IS_WALKING, true);
        m_Animator.SetBool(IS_ATTACKING, false);
        m_Animator.SetBool(IS_IDLE, false);
        yield return null;
        isTransitioning = false;
    }

    private void SetAttackState()
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionToAttack());
    }

    private IEnumerator TransitionToAttack()
    {
        isTransitioning = true;
        m_Agent.isStopped = true;
        m_Animator.SetBool(IS_WALKING, false);
        m_Animator.SetBool(IS_ATTACKING, true);
        m_Animator.SetBool(IS_IDLE, false);
        yield return null;
        isTransitioning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Sword"))
        {
            Debug.Log("Enemy hit by Sword!");
            Die();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (ShowDetectionRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, DetectionRange);
        }
    }
}
