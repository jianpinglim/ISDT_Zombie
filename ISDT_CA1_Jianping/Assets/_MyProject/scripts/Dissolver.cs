using System.Collections;
using UnityEngine;

public class Dissolver : MonoBehaviour
{
    public float dissolveDuration = 2f;
    public float dissolveStrength;

    public IEnumerator dissolver(){
        float elapsedTime = 0;
        Material dissolveMaterial = GetComponent<Renderer>().material;

        while(elapsedTime < dissolveDuration){
            elapsedTime += Time.deltaTime;
            dissolveStrength = Mathf.Lerp(0, 1, elapsedTime / dissolveDuration);
            dissolveMaterial.SetFloat("_DissolveStrength", dissolveStrength);
            yield return null;
        }
    }
}
