using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class PartyInfoView : MonoBehaviour
{
    public struct PartyInfoDesc
    {
        public int      ChracterID;
    }

    public event Action<int>            m_OnClicked;
    public bool bIsEnable { get; private set; }

    [SerializeField] private Image      m_CharacterIcon;
    [SerializeField] private Image      m_PartyAddIcon;

    [SerializeField] private Text       m_NameTxt;
    [SerializeField] private Text       m_HPTxt;
    [SerializeField] private Text       m_AttackTxt;

    private int m_Index = -1;


    private void Awake()
    {
        Button button = GetComponentInChildren<Button>();

        button.onClick.AddListener(OnClicked);
        OnChangeValue(-1);
    } 

    private void OnDestroy()
    {
        Button button = GetComponentInChildren<Button>();

        button.onClick.RemoveListener(OnClicked);
    }

    public void OnChangeValue(int CharacterID)
    {
        if(CharacterID == -1)
        {
            VisibleChildText(false);
            m_PartyAddIcon.gameObject.SetActive(true);
            m_CharacterIcon.gameObject.SetActive(false);
            bIsEnable = false;
        }
        else
        {
            var DataMgr = DataManager.instance;
            if (DataMgr == null)
                return;

            var CharacterData = DataMgr.GetCharacterById(CharacterID);
            if (CharacterData.Item1 == null)
                return;

            VisibleChildText(true);
            bIsEnable = true;

            m_PartyAddIcon.gameObject.SetActive(false);
            m_CharacterIcon.gameObject.SetActive(true);

            m_NameTxt.text = CharacterData.Item2.NameKo;
            m_HPTxt.text = CharacterData.Item2.MaxHp.ToString();
            m_AttackTxt.text = CharacterData.Item2.Attack.ToString();
        }
    }

    private void VisibleChildText(bool Value)
    {
        m_NameTxt.enabled = Value;
        m_HPTxt.enabled = Value;
        m_AttackTxt.enabled = Value;
    }


    private void OnClicked()
    {
        m_OnClicked.Invoke(m_Index);
    }
}
