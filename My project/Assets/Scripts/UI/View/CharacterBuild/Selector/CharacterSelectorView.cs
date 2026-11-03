using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public sealed class CharacterSelectorView : MonoBehaviour
{
    public event Action<int>    OnSelectCharacter;

    [SerializeField] private Text TitleTxt;
    [SerializeField] private List<CharacterSelectorInfoView> _CharacterInfoView;

    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);

    private void Awake()
    {
        int Count = 0;

        foreach (var item in _CharacterInfoView)
        {
            item.OnClickedCharacter += OnChangeItem;
            item.Init(Count);
            Count++;
        }
    }

    private void OnDestroy()
    {
        foreach (var item in _CharacterInfoView)
        {
            item.OnClickedCharacter -= OnChangeItem;
        }
    }

    private void OnChangeItem(int DataID)
    {
        if (DataID == -1)
            return;

        OnSelectCharacter?.Invoke(DataID);
        Close();
    }
}
