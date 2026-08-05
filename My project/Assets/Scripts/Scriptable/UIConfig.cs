using UI.Enum;
using UnityEngine;

[CreateAssetMenu(fileName = "UI", menuName = "Scriptable Objects/UI")]
public class UIConfig: ScriptableObject
{
    [Header("UI ID")]
    public UIID     ID;

    [Header("Addressable")]
    public string AddressKey;

    [Header("생성 위치")]
    public EUICanvas CanvasType;

    [Header("바인딩 Key")]
    public KeyCode Key = KeyCode.None;
}
