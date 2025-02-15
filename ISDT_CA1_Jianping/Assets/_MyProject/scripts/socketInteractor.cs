using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class socketInteractor : XRSocketInteractor
{
    private Gun gunScript;

    protected override void Start()
    {
        base.Start();
        gunScript = GetComponentInParent<Gun>();
        Debug.Log("Socket interactor initialized"); // Debug
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        Debug.Log("Something entered socket"); // Debug
        
        if (args.interactableObject.transform.TryGetComponent<VRMagazine>(out var magazine))
        {
            Debug.Log("Found VRMagazine component"); // Debug
            gunScript.AttachMagazine(magazine);
        }
        else
        {
            Debug.LogError("No VRMagazine component found on inserted object!");
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        gunScript.ReleaseMagazine();
        Debug.Log("Magazine removed from socket");
    }
}