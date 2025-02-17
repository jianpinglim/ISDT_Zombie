using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketScaleManager : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;
    private Vector3 originalScale;
    private GameObject currentInteractable;

    void Start()
    {
        socketInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        socketInteractor.selectEntered.AddListener(OnSelectEntered);
        socketInteractor.selectExited.AddListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        currentInteractable = args.interactableObject.transform.gameObject;
        originalScale = currentInteractable.transform.localScale;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (currentInteractable != null)
        {
            currentInteractable.transform.localScale = originalScale;
        }
    }

    void OnDestroy()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.RemoveListener(OnSelectEntered);
            socketInteractor.selectExited.RemoveListener(OnSelectExited);
        }
    }
}