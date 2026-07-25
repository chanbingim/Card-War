using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class StageClearComponent : MonoBehaviour
{
    [SerializeField] private Image          _LockImage;
    [SerializeField] private Text           _Text;
    [SerializeField] private List<Image>    _Images;

    private List<DoTweenAnimator>    _ImageAnim;
    private Sprite[]                 _Sprites;
    private int                      _StarCount = -1;

    private void Awake()
    {
        var addressableMgr = AddressableManager.instance;
        if (Utility.CHECK(addressableMgr) == false)
            return;

        var atlas = addressableMgr.Get<SpriteAtlas>("Atlas/StageClear");
        if (Utility.CHECK(atlas) == false)
            return;

        _ImageAnim = new List<DoTweenAnimator>();
        _ImageAnim.Capacity = _Images.Count;
        foreach (var image in _Images)
        {
            image.gameObject.SetActive(false);
            _ImageAnim.Add(image.gameObject.GetComponent<DoTweenAnimator>());
            var Animator = _ImageAnim.Last();
            Animator.Pause_Animation();
            Animator.Initialize();
        }

        _Sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(_Sprites);
        CloseStage();
    }
    
    public void OpenStage(string Stagename)
    {
        _Text.gameObject.SetActive(true);
        _LockImage.gameObject.SetActive(false);

        _Text.text = Stagename;
    }

    public void CloseStage()
    {
        _Text.gameObject.SetActive(false);
        _LockImage.gameObject.SetActive(true);
        _StarCount = -1;
    }

    public void ClearStage(int StarCount, bool IsClear)
    {
        for (int i = 0; i < StarCount; ++i)
        {
            if (_StarCount < i)
            {
                _Images[i].gameObject.SetActive(true);
                _Images[i].sprite = _Sprites[1];
                _ImageAnim[i].Initialize();
                _ImageAnim[i].Play_Animation();
            }
        }
        _StarCount = StarCount;
    }
}
