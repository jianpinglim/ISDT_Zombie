using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
public class GrabHandPose : MonoBehaviour
{
    public HandData rightHandPose;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(SetUpPose);
        rightHandPose.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void SetUpPose(BaseInteractionEventArgs arg){
        if(arg.interactorObject is XRDirectInteractor){
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = false;
        }
    }
}
