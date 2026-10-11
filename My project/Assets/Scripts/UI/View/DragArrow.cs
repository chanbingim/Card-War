using UnityEngine;

public class DragArrow : UIBase
{
    [SerializeField] private Texture    _trailTex;
    [SerializeField] private RectTransform _Trailtransform;
    private Material                       _material;

    private void Awake()
    {

    }

    public void UpdateArrow(Vector3 start, Vector3 end)
    {
        Vector2 size = _Trailtransform.sizeDelta;
        size.x = Vector3.Distance(start, end);

        float length = Vector3.Magnitude(end - start) + 5f;
        Vector3 vDir = (end - start).normalized;

        transform.position = end;
        _Trailtransform.position = start + vDir * length * 0.5f;
        _Trailtransform.sizeDelta = size;

        float angle = Mathf.Atan2(vDir.y, vDir.x) * Mathf.Rad2Deg;
        Quaternion Rot = Quaternion.Euler(0, 0, angle);
        _Trailtransform.rotation = Rot;
        transform.localRotation = Rot;
        //UpdateMatrial(start, end);
    }

    private void UpdateMatrial(Vector3 start, Vector3 end)
    {
        _material.SetVector("_start", start);
        _material.SetVector("_end", end);

        _material.SetTexture("TrailTex", _trailTex);
    }
}
