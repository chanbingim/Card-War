using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GAME_CONST;
using static PartyInfoView;

public sealed class CharacterBuildPartyView : MonoBehaviour
{
    [SerializeField] private Text                   PartyCountTxt;
    [SerializeField] private List<PartyInfoView>    PartyInfos;
    [SerializeField] private CharacterSelectorView  Selector;

    private int selectedSlot;

    private void Awake()
    {
        foreach(var InfoView in PartyInfos)
        {
            InfoView.m_OnClicked += Open;
        }

        SettingPartyData();
        var GameClientMgr = GameClientManager.instance;
        if (GameClientMgr == null)
            return;

        GameClientMgr._playerData.OnCharacterParty += OnChangePartyData;
        Selector.OnSelectCharacter += OnSelectCahracterID;
        Selector.Close();
    }

    private void OnDestroy()
    {
        foreach (var InfoView in PartyInfos)
        {
            InfoView.m_OnClicked -= Open;
        }

        var GameClientMgr = GameClientManager.instance;
        if (GameClientMgr == null)
            return;

        GameClientMgr._playerData.OnCharacterParty -= OnChangePartyData;
        Selector.OnSelectCharacter -= OnSelectCahracterID;
    }

    private void Open(int idx)
    {
        selectedSlot = idx;
        Selector.Open();
    }

    private void OnChangePartyData(IReadOnlyList<int> characterPartys)
    {
        for (int i = 0; i < PartyInfos.Count; i++)
        {
            PartyInfos[i].OnChangeValue(characterPartys[i]);
        }

        SettingPartyData();
    }

    private void OnSelectCahracterID(int ID)
    {
        var GameClientMgr = GameClientManager.instance;
        if (GameClientMgr != null)
            return;

        GameClientMgr._playerData.RquestADDPartData(selectedSlot, ID);
    }

    private void SettingPartyData()
    {
        int Count = 0;
        foreach (var InfoView in PartyInfos)
        {
            if(InfoView.bIsEnable)
                Count++;
        }

        PartyCountTxt.text = $"PARTY   {Count} / {Const.MaxPartyCount}";
    }
}
