using System.Collections;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float dissolveTime = 2f;
    private float dissolveValue = 0f;
    private static readonly int EdgeWidth = Shader.PropertyToID("_edge_width");

    public void StartDissolve()
    {
        dissolveValue = 0f;
        StartCoroutine(DissolveCoroutine());
    }

    private IEnumerator DissolveCoroutine()
    {
        while (dissolveValue < 1f)
        {
            dissolveValue += Time.deltaTime / dissolveTime;
            // Update the edge_width property which controls the dissolve threshold
            dissolveMaterial.SetFloat(EdgeWidth, dissolveValue);
            yield return null;
        }
        
        // Optional: Destroy the object when fully dissolved
        Destroy(gameObject);
    }
}