using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrenadeBehavior : MonoBehaviour
{
    [Header("Grenade Settings")]
    public float fuseTime = 3.0f;           // Seconds before detonation
    public float blastRadius = 5.0f;        // Radius of effect
    public float explosionForce = 500.0f;   // Force of explosion
    public int grenadeDamage = 50;          // Damage dealt to enemies
    public GameObject explosionEffect;      // Particle prefab for explosion

    private bool isArmed = false;           // Has the pin been pulled or is the grenade activated?
    private bool isHeld = false;            // Is the grenade currently being held?
    private bool triggerPressed = false;    // Has the trigger been pressed?
    private float timer = 0.0f;
    private XRGrabInteractable grabInteractable;
    //testing again asd

    void Awake()
    {
        // Get XR Interactable reference
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
            grabInteractable.activated.AddListener(OnTriggerPull);
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
            grabInteractable.activated.RemoveListener(OnTriggerPull);
        }
    }

    void Update()
    {
        // If grenade is armed and fuse has started, keep updating the timer
        if (isArmed)
        {
            timer += Time.deltaTime;
            if (timer >= fuseTime)
            {
                Explode();
            }
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true; // Mark as being held
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false; // Mark as no longer held
    }

    void OnTriggerPull(ActivateEventArgs args)
    {
        if (isHeld && !isArmed)
        {
            // Start the fuse countdown when the trigger is pulled while holding the grenade
            isArmed = true;
            timer = 0.0f;  // Reset the timer
            triggerPressed = true; // Mark that the trigger has been pressed
            Debug.Log("Trigger pulled! Fuse started.");
        }
    }

    void Explode()
    {
        // Play explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Play explosion sound
        AudioManager.instance.PlaySfx("Explosion");


        // Apply physics force to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider nearby in colliders)
        {
            // Apply force to rigidbodies
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, blastRadius);
            }

            // Damage enemies
            EnemyAI enemy = nearby.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(grenadeDamage);
            }
        }

        // Destroy grenade object
        Destroy(gameObject);
    }
}
