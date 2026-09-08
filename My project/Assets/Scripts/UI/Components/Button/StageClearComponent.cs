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

    private int                      _StarCount = 0;
    private int                      _MaxStar = -1;

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
            _ImageAnim.Add(image.gameObject.GetComponent<DoTweenAnimator>());

            var Animator = _ImageAnim.Last();
            Animator.OnCompleted += OnCompoletedStarAnim;
            Animator.Pause_Animation();
            Animator.Initialize();
            Animator.enabled = false;
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
        _StarCount = 0;
    }

    public void ClearStage(int StarCount, bool IsClear)
    {
        if(IsClear)
        {
            _MaxStar = StarCount;
            OnCompoletedStarAnim();
        }
    }

    private void OnCompoletedStarAnim()
    {
        if (_StarCount < _MaxStar)
        {
            _Images[_StarCount].sprite = _Sprites[1];

            _ImageAnim[_StarCount].enabled = true;
            _ImageAnim[_StarCount].Initialize();
            _ImageAnim[_StarCount].Play_Animation();

            _StarCount++;
        }
    }

    private void OnDestroy()
    {
        foreach (var Animator in _ImageAnim)
            Animator.OnCompleted -= OnCompoletedStarAnim;
    }
}
