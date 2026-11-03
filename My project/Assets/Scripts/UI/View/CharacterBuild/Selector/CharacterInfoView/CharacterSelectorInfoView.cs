using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class CharacterSelectorInfoView : MonoBehaviour
{
    public event Action<int> OnClickedCharacter;
    private int CharacterID = -1;

    public void Init(int ID)
    {
        CharacterID = ID;
        var Button = GetComponentInChildren<Button>();
        Button.onClick.AddListener(OnClick);

        // 여기서 데이터 세팅
    }

    private void OnDestroy()
    {
        var Button = GetComponentInChildren<Button>();
        Button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        OnClickedCharacter?.Invoke(CharacterID);
    }
}
