using UnityEngine;

public class OutLineRenderer : MonoBehaviour
{
    [SerializeField]  private Material _OutLineMatrial;

    private MeshRenderer _meshRenderer;
    private Material    _OrizinMatrial;

    public void Initialize()
    {
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (_meshRenderer == null)
            return;

        _OrizinMatrial = _meshRenderer.materials[0];
       
    }

    public void OnEnableOutLine(bool bIsEnable)
    {
        if(bIsEnable)
        {
            _OutLineMatrial.mainTexture = _OrizinMatrial.mainTexture;
            _meshRenderer.material = _OutLineMatrial;
        }
        else
            _meshRenderer.material = _OrizinMatrial;

        _OutLineMatrial.SetFloat("_Enable", bIsEnable ? 1f : 0f);
    }
}
