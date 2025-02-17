using UnityEngine;

public class VRMagazine : MonoBehaviour
{
    [SerializeField] private MagazineData magazineData;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;

    private void Start()
    {
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
        
    }

    public MagazineData GetMagazineData()
    {
        return magazineData;
    }

}