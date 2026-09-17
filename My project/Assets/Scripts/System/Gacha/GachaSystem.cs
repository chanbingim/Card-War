using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GachaSystem;

// 가챠 함수를 만든다.
// 함수는 Random 또는 특정 확률알고리즘을 통해서 가챠 목록의 데이터에서 뽑아온다.
public class GachaSystem : MonoBehaviour, IInitialize
{
    // 가챠 데이터
    #region GachData
    [System.Serializable]
    public class GachaData
    {
        public int      iID;
        public bool     bIsPickUp;
        public float    fWeight;
    }

    [System.Serializable]
    public class Banner
    {
        public string startsAtUtc;
        public string endsAtUtc;
    }

    [System.Serializable]
    public class GachaDataList
    {
        public Banner       banners;
        public GachaData[]  groups;
    }
    #endregion

    [SerializeField] private float              PickUpWeight;
    [SerializeField] private string             _PickUpAssetURL;
    [SerializeField] private List<TextAsset>    _OldPickUpData;

    #region Banner
    public event Action<Banner> OnChangeBanner;

    public Banner               _SelectPickUpData { get; private set; }
    List<Banner>                _banners = new();
    #endregion

    private float               _TotalPickWeight = 0.0f;
    private Dictionary<Banner, List<(int ID, float Weight)>>   _PickUpList = new();

    public void ChangeBanner(int index)
    {
        if (0 > index || _banners.Count <= index)
            return;

        _SelectPickUpData = _banners[index];
        OnChangeBanner?.Invoke(_SelectPickUpData);
    }

    public int RequestGachaResult()
    {
        // 여기서 캐릭터 데이터 검증
        var ClientData = GameClientManager.instance;
        if (ClientData == null)
        {
            Debug.Log("[GachaSystem] Not Find Client Data");
            return -1;
        }

        if(_SelectPickUpData == null)
        {
            Debug.Log("[GachaSystem] Not Select PickupList");
            return -1;
        }

        return GetGachaResult();
    }

    private int GetGachaResult()
    {
        float accumulator = 0.0f;
        float RandomWeight = Utility.GetRandomFloat(0, _TotalPickWeight);

        _PickUpList[_SelectPickUpData].Sort((item1, item2) =>
        {
            return item1.Weight.CompareTo(item2.Weight);
        });

        foreach (var item in _PickUpList[_SelectPickUpData])
        {
            accumulator += item.Weight;

            if (RandomWeight <= accumulator)
            {
                Debug.Log($"Get Character : {item.ID}");
                return item.ID;
            }
        }

        return 0;
    }

    #region Default
    static public GachaSystem instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public UniTask Initialize()
    {
        var task = UniTask.WhenAll(LoadPickupList());
        ChangeBanner(0);

        return task;
    }

    private UniTask LoadPickupList()
    {
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("PickUpList");
        if (jsonFiles == null)
            return UniTask.CompletedTask;

        foreach (var json in jsonFiles)
        {
            var DataList = JsonUtility.FromJson<GachaDataList>(json.text);
            if (DataList == null)
            {
                Debug.Log("[GachaSystem] Not Find Json Gacha Data");
                return UniTask.CompletedTask;
            }

            List<(int ID, float Weight)> newList = new();
            foreach (var item in DataList.groups)
            {
                float weight = item.fWeight;

                if (item.bIsPickUp)
                    weight *= PickUpWeight;

                _TotalPickWeight += weight;
             
                newList.Add((item.iID, weight));
            }

            _banners.Add(DataList.banners);
            _PickUpList.Add(_banners.Last(), newList);
        }
       
        return UniTask.CompletedTask;
    }

    #endregion
}
