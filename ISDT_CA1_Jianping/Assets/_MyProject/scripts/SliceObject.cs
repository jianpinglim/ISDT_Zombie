using UnityEngine;
using EzySlice;

public class SliceObject : MonoBehaviour
{
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public LayerMask sliceableLayer;
    public VelocityEstimator velocityEstimator;

    public Material crossSectionMaterials;
    public float cutForce = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate() {
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        if(hasHit){
            GameObject target = hit.transform.gameObject;
            Slice(target);
        }
    }

    public void Slice(GameObject target)
    {
        // Get the renderer to fetch the materials of the target object
        MeshRenderer targetRenderer = target.GetComponent<MeshRenderer>();
        if (targetRenderer == null)
        {
            Debug.LogWarning("Target does not have a MeshRenderer component!");
            return;
        }

        // Fetch all materials from the target and add the cross-section material at the end
        Material[] materials = targetRenderer.sharedMaterials;
        Material[] crossSectionMaterials = new Material[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            crossSectionMaterials[i] = this.crossSectionMaterials; // Use the cross-section material for all submeshes
        }

        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(velocity, endSlicePoint.position - startSlicePoint.position).normalized;

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal, crossSectionMaterials[0]);

        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterials[0]);
            SetupSlicedComponent(upperHull);

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterials[0]);
            SetupSlicedComponent(lowerHull);

            Destroy(target);
        }
    }


    public void SetupSlicedComponent(GameObject slicedObject){
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider mc = slicedObject.AddComponent<MeshCollider>();
        mc.convex = true;
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }
}
