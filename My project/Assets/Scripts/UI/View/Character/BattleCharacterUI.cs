using Custom.Struct;
using DG.Tweening;
using TurnCardGame.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleCharacterUI : MonoBehaviour
{
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Text   _ATKtext;
    private Character     _Owner = null;

    public void Initalize(Character Owner, Vector3 Position, Transform Parent)
    {
        _Owner = Owner;

        Position.z = 0;

        SettingHpPosition();
        transform.SetParent(Parent);

        OnChangeState(_Owner.Data);
        _Owner.OnChangedState += OnChangeState;
    }

    public void Update()
    {
        SettingHpPosition();
    }

    public void Release()
    {
        if (_Owner == null)
            return;

        _Owner.OnChangedState -= OnChangeState;
        _Owner = null;
    }

    private void SettingHpPosition()
    {
        var BoxInfo = _Owner.GetPivotPoint();
        transform.position = _Owner.transform.position + new Vector3(0, BoxInfo.size.y * 1.2f, 0);
    }

    private void OnChangeState(CharacterRuntimeData Data)
    {
        _progressBar.DOValue(Data.HealthRatio, 0.7f);
        _ATKtext.DOText($"ATK : {Data.CurrentATKPower}", 0.7f);
    }

    public void OnDisable()
    {
        _progressBar.DOKill();
        Release();
    }
}
