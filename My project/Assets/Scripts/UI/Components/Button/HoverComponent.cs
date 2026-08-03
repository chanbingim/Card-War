using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class HoverComponent : HoverHandler
{
    [SerializeField] private string         _AltasURL;

    private SpriteAtlas     _Atlas;
    private Sprite[]        _Textures;
    private Image           _Image;

    protected void Awake()
    {
        _Atlas = Resources.Load<SpriteAtlas>(_AltasURL);
        _Textures = new Sprite[_Atlas.spriteCount];
        _Atlas.GetSprites(_Textures);
        Array.Sort(_Textures, (item1, item2) =>
        {
            int num1 = int.Parse(item1.name.Split('_')[1].Split('(')[0]);
            int num2 = int.Parse(item2.name.Split('_')[1].Split('(')[0]);

            return num1.CompareTo(num2);
        });

        _Image = GetComponent<Image>();
        _Image.sprite = _Textures[0];
    }

    protected override void OnMouseHoverEvent(bool IsHover)
    {
        if (_Textures.Length < 2)
        {
            Debug.Log("UnBind Hover Teuxtre");
            return;
        }

        if (IsHover)
            _Image.sprite = _Textures[1];
        else
            _Image.sprite = _Textures[0];
    }
}
