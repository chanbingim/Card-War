using UnityEngine;

public class OutLineRenderer : MonoBehaviour
{
    [SerializeField]  private Material _OutLineMatrial;

    private MeshRenderer _meshRenderer;
    private Material    _MeshRenderMat;
    private Material    _OrizinMatrial;

    private void Awake()
    {
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (_meshRenderer == null)
            return;

        _MeshRenderMat = _meshRenderer.material;
        _OrizinMatrial = _meshRenderer.materials[0];
        _OutLineMatrial.mainTexture = _OrizinMatrial.mainTexture;
    }

    public void OnEnableOutLine(bool bIsEnable)
    {
        if(bIsEnable)
            _MeshRenderMat = _OutLineMatrial;
        else
            _MeshRenderMat = _OrizinMatrial;

        _OutLineMatrial.SetFloat("_Enable", bIsEnable ? 1f : 0f);
    }
}
