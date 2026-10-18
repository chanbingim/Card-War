using Custom.Struct;
using DG.Tweening;
using TurnCardGame.Data;
using UnityEngine;
using UnityEngine.UI;

public class BattleCharacterUI : MonoBehaviour
{
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Slider _BackGroundBar;
    [SerializeField] private Text   _Text;

    private Character     _Owner = null;

    public void Initalize(Character Owner)
    {
        _Owner = Owner;

        OnChangeState(_Owner.Data);
        _Owner.OnChangedState += OnChangeState;
    }

    public void Release()
    {
        if (_Owner == null)
            return;

        _Owner.OnChangedState -= OnChangeState;
        _Owner = null;
    }

    public void OnDisable()
    {
        _progressBar.DOKill();
        _BackGroundBar.DOKill();
        _Text.DOKill();
        Release();
    }

    private void OnChangeState(CharacterRuntimeData Data)
    {
        _progressBar.value = Data.HealthRatio;

        HPBackGroundAnim();
        _Text.text = $" {Data.CurrentHealth} : {Data.SourceInfo.MaxHp}";
    }

    private void HPBackGroundAnim()
    {
        _BackGroundBar.DOValue(_progressBar.value, 0.7f)
                    .SetDelay(0.7f);
    }
}
