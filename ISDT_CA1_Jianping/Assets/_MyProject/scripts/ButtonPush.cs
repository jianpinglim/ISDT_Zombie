using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ButtonPush : MonoBehaviour
{
    [SerializeField] 
    private float transitionDelay = 1f; // Delay before scene change
    
    private void Start()
    {
        XRSimpleInteractable button = GetComponent<XRSimpleInteractable>();
        if (button != null)
        {
            button.selectEntered.AddListener(OnButtonPressed);
        }
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        StartCoroutine(LoadEndScene());
    }

    private IEnumerator LoadEndScene()
    {
        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene("EndScene");
    }
}