using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
   public float dissolveSpeed = 2f; // Speed of the dissolve effect
   public float edge_width; // Strength of the dissolve effect

   public void StartDissolve(){
       StartCoroutine(dissolver());
   }

   public IEnumerator dissolver(){
        float elaspesTime = 0;
        Material dissolveMaterial = GetComponent<Renderer>().material;

        while(elaspesTime < dissolveSpeed){
            elaspesTime += Time.deltaTime;
            edge_width = Mathf.Lerp(0, 1, elaspesTime / dissolveSpeed);
            dissolveMaterial.SetFloat("_edge_width", edge_width);

            yield return null;
        }
   }
}