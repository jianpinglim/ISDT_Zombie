using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private bool AddBulletSpread = true;
    [SerializeField] private Vector3 BulletSpreadVariance = new Vector3(0.05f, 0.05f, 0.05f);
    [SerializeField] private Vector3 TightSpreadVariance = new Vector3(0.01f, 0.01f, 0.01f);
    [SerializeField] private float ShootDelay = 0.5f;
    [SerializeField] private float BulletSpeed = 100;

    [Header("Visual Components")]
    [SerializeField] private ParticleSystem ShootingSystem; // For shooting effects
    [SerializeField] private ParticleSystem MuzzleFlash;    // Muzzle flash effect
    [SerializeField] private Transform BulletSpawnPoint;
    [SerializeField] private ParticleSystem ImpactParticleSystem;
    [SerializeField] private TrailRenderer BulletTrail;
    [SerializeField] private Transform slideTransform;
    [SerializeField] private LayerMask Mask;

    [Header("Two-Handed Setup")]
    [SerializeField] private XRGrabInteractable primaryGrabInteractable;
    [SerializeField] private XRSimpleInteractable secondaryGrabInteractable;
    [SerializeField] private Transform secondaryGrabPoint;

    [Header("Magazine System")]
    [SerializeField] private Transform magazineSocket;
    [SerializeField] private MagazineData defaultMagazine;

    private bool isTwoHanded = false;
    private MagazineData currentMagazine;
    private float LastShootTime;
    private Vector3 currentSpread;
    private bool isSlideBack = false;
    private Coroutine slideCoroutine;

    private void Awake()
    {
        Debug.Log("Gun Initializing...");

        if (primaryGrabInteractable != null)
        {
            primaryGrabInteractable.activated.AddListener(OnTriggerPull);
            Debug.Log("Primary grab interactable setup complete");
        }

        currentSpread = BulletSpreadVariance;
        currentMagazine = null; // Start with no magazine

        SetupTwoHandedGrab();
    }

    private void OnDestroy()
    {
        if (primaryGrabInteractable != null)
        {
            primaryGrabInteractable.activated.RemoveListener(OnTriggerPull);
        }

        if (secondaryGrabInteractable != null)
        {
            secondaryGrabInteractable.selectEntered.RemoveListener(OnSecondHandGrab);
            secondaryGrabInteractable.selectExited.RemoveListener(OnSecondHandRelease);
        }
    }

    private void SetupTwoHandedGrab()
    {
        if (secondaryGrabInteractable != null)
        {
            secondaryGrabInteractable.selectEntered.AddListener(OnSecondHandGrab);
            secondaryGrabInteractable.selectExited.AddListener(OnSecondHandRelease);
        }
    }

    private void OnTriggerPull(ActivateEventArgs args)
    {
        Debug.Log("Trigger pulled");
        Shoot();
    }

    public void OnSecondHandGrab(SelectEnterEventArgs args)
    {
        Debug.Log("Second hand grabbed");
        isTwoHanded = true;
        currentSpread = TightSpreadVariance;
    }

    public void OnSecondHandRelease(SelectExitEventArgs args)
    {
        Debug.Log("Second hand released");
        isTwoHanded = false;
        currentSpread = BulletSpreadVariance;
    }

    public void Shoot()
{
    Debug.Log("Shoot method called");

    if (LastShootTime + ShootDelay >= Time.time)
    {
        Debug.Log("Shot delayed - too soon since last shot");
        return;
    }

    if (currentMagazine == null || currentMagazine.currentAmmo <= 0)
    {
        Debug.Log("Cannot shoot - no ammo");
        if (!isSlideBack)
            StartCoroutine(AnimateSlideBack());
        return;
    }

    // Play muzzle flash
    if (MuzzleFlash != null)
    {
        MuzzleFlash.Play();
        Debug.Log("Playing muzzle flash effect");
    }

    // Play shooting effects
    if (ShootingSystem != null)
    {
        ShootingSystem.Play();
        Debug.Log("Playing shooting system effect");
    }

    Vector3 direction = GetDirection();

    if (Physics.Raycast(BulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, Mask))
    {
        // Create a trail effect
        TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));
        Debug.Log($"Hit target at {hit.point}");

        // Check if the hit object has an EnemyAI script
        EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            // Apply damage to the enemy
            enemy.TakeDamage(20); // Adjust the damage value as needed
        }
    }
    else
    {
        TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(trail, BulletSpawnPoint.position + direction * 100, Vector3.zero, false));
        Debug.Log("Shot fired but missed");
    }

    currentMagazine.currentAmmo--;
    LastShootTime = Time.time;

    Debug.Log($"Rounds remaining: {currentMagazine.currentAmmo}");

    if (currentMagazine.currentAmmo <= 0)
        StartCoroutine(AnimateSlideBack());
    else
        StartCoroutine(AnimateSlide());
}

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;

        if (AddBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-currentSpread.x, currentSpread.x),
                Random.Range(-currentSpread.y, currentSpread.y),
                Random.Range(-currentSpread.z, currentSpread.z)
            );

            direction.Normalize();
        }

        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
    {
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= BulletSpeed * Time.deltaTime;
            yield return null;
        }

        Trail.transform.position = HitPoint;
        if (MadeImpact)
        {
            Instantiate(ImpactParticleSystem, HitPoint, Quaternion.LookRotation(HitNormal));
        }

        Destroy(Trail.gameObject, Trail.time);
    }

    private IEnumerator AnimateSlide()
    {
        Vector3 initialPosition = slideTransform.localPosition;
        Vector3 backPosition = initialPosition + Vector3.back * 0.1f;

        float elapsed = 0f;
        float duration = 0.05f;
        while (elapsed < duration)
        {
            slideTransform.localPosition = Vector3.Lerp(initialPosition, backPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            slideTransform.localPosition = Vector3.Lerp(backPosition, initialPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        slideTransform.localPosition = initialPosition;
    }

    private IEnumerator AnimateSlideBack()
    {
        if (isSlideBack) yield break;

        isSlideBack = true;
        Vector3 initialPosition = slideTransform.localPosition;
        Vector3 backPosition = initialPosition + Vector3.back * 0.1f;

        float elapsed = 0f;
        float duration = 0.1f;
        while (elapsed < duration)
        {
            slideTransform.localPosition = Vector3.Lerp(initialPosition, backPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        slideTransform.localPosition = backPosition;
    }

    public void AttachMagazine(VRMagazine magazineObject)
    {
        if (magazineObject != null)
        {
            currentMagazine = magazineObject.GetMagazineData();
            Debug.Log($"New magazine attached with {currentMagazine.currentAmmo} rounds");

            if (isSlideBack && currentMagazine.currentAmmo > 0)
                ReleaseSlideLock();
        }
    }

    public void ReleaseMagazine()
    {
        currentMagazine = null;
        Debug.Log("Magazine released");
    }

    public void ReleaseSlideLock()
    {
        if (!isSlideBack) return;
        StartCoroutine(AnimateSlide());
    }
}
