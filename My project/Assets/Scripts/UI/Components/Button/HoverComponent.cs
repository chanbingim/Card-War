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
