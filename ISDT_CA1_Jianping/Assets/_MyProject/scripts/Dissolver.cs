using System.Collections;
using UnityEngine;

public class Dissolver : MonoBehaviour
{
    public Material dissolveMaterial;
    public float dissolveDuration = 0.6f;
    public float dissolveStrength;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Material[] originalMaterials; // Changed to array

    void Awake()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError($"SkinnedMeshRenderer not found in {gameObject.name} or its children!");
            return;
        }

        // Store all original materials
        originalMaterials = skinnedMeshRenderer.materials;
        if (dissolveMaterial == null)
        {
            Debug.LogError($"Dissolve material not assigned on {gameObject.name}!");
        }
    }

    public IEnumerator dissolver()
    {
        if (dissolveMaterial == null)
        {
            Debug.LogError($"Dissolve material not assigned on {gameObject.name}!");
            yield break;
        }

        if (skinnedMeshRenderer == null)
        {
            Debug.LogError($"SkinnedMeshRenderer not found on {gameObject.name}!");
            yield break;
        }

        // Create array of dissolve materials
        Material[] dissolveMaterials = new Material[skinnedMeshRenderer.materials.Length];
        for (int i = 0; i < dissolveMaterials.Length; i++)
        {
            dissolveMaterials[i] = dissolveMaterial;
        }

        // Apply dissolve materials to all slots
        skinnedMeshRenderer.materials = dissolveMaterials;
        float elapsedTime = 0;

        while (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;
            dissolveStrength = Mathf.Lerp(0, 1, elapsedTime / dissolveDuration);
            
            // Apply dissolve effect to all materials
            foreach (Material mat in skinnedMeshRenderer.materials)
            {
                mat.SetFloat("_DissolveStrength", dissolveStrength);
            }
            yield return null;
        }
    }

    void OnDestroy()
    {
        // Restore original materials when destroyed
        if (skinnedMeshRenderer != null && originalMaterials != null)
        {
            skinnedMeshRenderer.materials = originalMaterials;
        }
    }
}