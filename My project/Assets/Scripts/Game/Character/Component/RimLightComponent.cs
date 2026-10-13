using System.Collections;
using UnityEngine;

public class RimLightComponent : MonoBehaviour
{
    [SerializeField] private float      Power = 1.0f;
    [SerializeField] private float      Duration = 1.0f;
    [SerializeField] private Color      RimLightColor = Color.white;
    [SerializeField] private Texture2D  NormalTex = null;
    [SerializeField] private Material   RimLightMaterial = null;

    private MeshRenderer        meshRenderer = null;
    private Material            OrizinMatrial;
  
    private Coroutine           RimLightCoroutine = null;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if(meshRenderer != null )
        {
            OrizinMatrial = meshRenderer.material;
        }

        RimLightMaterial.SetTexture("_NormalTexture", NormalTex);
        RimLightMaterial.SetTexture("_BaseTexture", OrizinMatrial.mainTexture);
        RimLightMaterial.SetFloat("_Power", Power);
        RimLightMaterial.SetVector("_Color", RimLightColor);
    }

    [ContextMenu("Rim Light дя╠Б")]
    public void Play()
    {
        RimLightMaterial.SetFloat("_OnEnable", 1);

        if (RimLightCoroutine != null)
            StopCoroutine(RimLightCoroutine);

        meshRenderer.material = RimLightMaterial;
        RimLightCoroutine = StartCoroutine(RimLightCorutine());
    }

    IEnumerator RimLightCorutine()
    {
        yield return new WaitForSeconds(Duration);

        meshRenderer.material = OrizinMatrial;
        RimLightMaterial.SetFloat("_OnEnable", 0);
        RimLightCoroutine = null;
    }
}
