using System.Collections;
using UnityEngine;


public class RimLightComponent : MonoBehaviour
{
    [System.Serializable]
    struct RimLightInfo
    {
        public float Power;
        public float Width;
        public float Intensity;
        public Color RimLightColor;
    }

    [SerializeField] private RimLightInfo   Info;
    [SerializeField] private float          Duration;
    [SerializeField] private Material       RimLightMaterial = null;

    private Texture2D           NormalTex = null;
    private MeshRenderer        meshRenderer = null;
    private Material            OrizinMatrial;
  
    private Coroutine           RimLightCoroutine = null;

    public void Initialize()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if(meshRenderer != null )
        {
            OrizinMatrial = meshRenderer.material;
            NormalTex = OrizinMatrial.GetTexture("_NormalTex") as Texture2D;
        }
    }

    [ContextMenu("Rim Light дя╠Б")]
    public void Play()
    {
        RimLightMaterial.SetFloat("_Enable", 1);
        SettingMatrial();

        if (RimLightCoroutine != null)
            StopCoroutine(RimLightCoroutine);

        meshRenderer.material = RimLightMaterial;
        RimLightCoroutine = StartCoroutine(RimLightCorutine());
    }

    private void SettingMatrial()
    {
        RimLightMaterial.SetTexture("_NormalTexture", NormalTex);
        RimLightMaterial.SetTexture("_BaseTexture", OrizinMatrial.mainTexture);
        
        RimLightMaterial.SetFloat("_Power", Info.Power);
        RimLightMaterial.SetFloat("_Intensity", Info.Intensity);

        RimLightMaterial.SetVector("_Color", Info.RimLightColor);
    }

    IEnumerator RimLightCorutine()
    {
        yield return new WaitForSeconds(Duration);

        meshRenderer.material = OrizinMatrial;
        RimLightMaterial.SetFloat("_Enable", 0);
        RimLightCoroutine = null;
    }
}
