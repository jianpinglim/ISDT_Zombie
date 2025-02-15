using UnityEngine;

public class VRMagazine : MonoBehaviour
{
    [SerializeField] private MagazineData magazineData;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;

    private void Start()
    {
        Debug.Log($"VRMagazine {gameObject.name} starting");
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        if (magazineData == null)
        {
            Debug.LogError($"No magazine data assigned to {gameObject.name}!");
            return;
        }
        
        // Create a new instance of the magazine data
        magazineData = Instantiate(magazineData);
        
        // Explicitly set the current ammo to max capacity
        magazineData.currentAmmo = magazineData.maxCapacity;
        
        Debug.Log($"Magazine {gameObject.name} initialized with {magazineData.currentAmmo}/{magazineData.maxCapacity} rounds");
    }

    public MagazineData GetMagazineData()
    {
        Debug.Log($"GetMagazineData called on {gameObject.name}"); // Debug line
        return magazineData;
    }

    // Add these debug methods
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Magazine {gameObject.name} triggered with {other.gameObject.name}");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Magazine {gameObject.name} collided with {collision.gameObject.name}");
    }
}