using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class InGameButton : MonoBehaviour,
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    IPointerEnterHandler,
    IPointerExitHandler
#else
    IPointerDownHandler,
    IPointerUpHandler
#endif
{
    [SerializeField] private string   _AltasURL;
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

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseHoverEvent(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseHoverEvent(false);
    }
#else
 public void OnPointerDown(PointerEventData eventData)
    {
        OnMouseHoverEvent(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnMouseHoverEvent(false);
    }
#endif

    private void OnMouseHoverEvent(bool bIsHover)
    {
        if(_Textures.Length < 2)
        {
            Debug.Log("UnBind Hover Teuxtre");
            return;
        }

        if(bIsHover)
            _Image.sprite = _Textures[1];
        else
            _Image.sprite = _Textures[0];
    }
}
