using Custom.Struct;
using UnityEngine;

public class BattleCharacterUIController : MonoBehaviour
{
    [SerializeField] private BattleCharacterUI  HPView;
    [SerializeField] private ChracterBuffView   BuffView;

    private Character   _Owner = null;
    private float       Pivot = 0.0f;

    public void Initalize(Character Owner, Transform WorldCanvas)
    {
        _Owner = Owner;

        HPView.Initalize(Owner);
        BuffView.Initalize(Owner);

        transform.SetParent(WorldCanvas.transform);

        // 전체 위치에서 더하기
        BoxInfo CharacterSize = Owner.GetPivotPoint();
        Pivot =  transform.localScale.y;
        Pivot += CharacterSize.size.y * 1.05f;

        SettingTransformPosition();
    }

    private void Update()
    {
       SettingTransformPosition();
    }

    private void SettingTransformPosition()
    {
        transform.position = _Owner.transform.position + new Vector3(0.0f, Pivot, 0.0f);
    }
}
