using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class CharacterBuildPartyUI : MonoBehaviour
{
    [Serializable] public class Slot { public Button button; public Text nameText; public Text roleText; public Image portrait; }
    [Serializable] public class Character { public string characterName; public string role; public Color color; }
    [SerializeField] private Slot[] slots;
    [SerializeField] private GameObject selector;
    [SerializeField] private Text selectorTitle;
    [SerializeField] private Button[] options;
    [SerializeField] private Character[] characters;
    private int selectedSlot;

    private void Awake()
    {
        for (int i = 0; i < slots.Length; i++) { int n = i; slots[i].button.onClick.AddListener(() => Open(n)); }
        for (int i = 0; i < options.Length; i++) { int n = i; options[i].onClick.AddListener(() => Select(n)); }
        selector.SetActive(false);
    }

    public void Open(int index) { selectedSlot = index; selectorTitle.text = $"파티 {index + 1}번 캐릭터 선택"; selector.SetActive(true); }
    public void Close() => selector.SetActive(false);
    public void Select(int index)
    {
        Character c = characters[index]; Slot s = slots[selectedSlot];
        s.nameText.text = c.characterName; s.roleText.text = c.role; s.portrait.color = c.color; Close();
    }
}
