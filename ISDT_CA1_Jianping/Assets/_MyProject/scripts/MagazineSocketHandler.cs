using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MagazineSocketHandler : MonoBehaviour
{
    private XRSocketInteractor socketInteractor;
    private Gun gunScript;

    private void Start()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
        gunScript = GetComponentInParent<Gun>();
        
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.AddListener(OnMagazineInserted);
            socketInteractor.selectExited.AddListener(OnMagazineRemoved);
            Debug.Log("Socket handler initialized");
        }
    }

    private void OnMagazineInserted(SelectEnterEventArgs args)
    {
        Debug.Log("Magazine insertion attempted");
        if (args.interactableObject.transform.TryGetComponent<VRMagazine>(out var magazine))
        {
            gunScript.AttachMagazine(magazine);
            Debug.Log("Magazine successfully attached");
        }
    }

    private void OnMagazineRemoved(SelectExitEventArgs args)
    {
        gunScript.ReleaseMagazine();
        Debug.Log("Magazine removed");
    }

    private void OnDestroy()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.RemoveListener(OnMagazineInserted);
            socketInteractor.selectExited.RemoveListener(OnMagazineRemoved);
        }
    }
}