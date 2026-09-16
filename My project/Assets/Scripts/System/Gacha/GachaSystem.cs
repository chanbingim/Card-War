using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

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
    public class GachaDataList
    {
        public GachaData[]  groups;
    }
    #endregion

    [SerializeField] private float              PickUpWeight;
    [SerializeField] private TextAsset          _PickUpData;
    [SerializeField] private List<TextAsset>    _OldPickUpData;

    private float               _TotalPickWeight = 0.0f;
    private List<(int ID, float Weight)>     _PickUpList = new();

    public int RequestGachaResult()
    {
        // 여기서 캐릭터 데이터 검증
        var ClientData = GameClientManager.instance;
        if (ClientData == null)
        {
            Debug.Log("[GachaSystem] Not Find Client Data");
            return -1;
        }

        return GetGachaResult();
    }

    private int GetGachaResult()
    {
        float accumulator = 0.0f;
        float RandomWeight = Utility.GetRandomFloat(0, _TotalPickWeight);

        _PickUpList.Sort((item1, item2) =>
        {
            return item1.Weight.CompareTo(item2.Weight);
        });

        foreach (var item in _PickUpList)
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
        Initialize();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public UniTask Initialize()
    {
        return UniTask.WhenAll(LoadPickupList());
    }

    private UniTask LoadPickupList()
    {
        var DataList = JsonUtility.FromJson<GachaDataList>(_PickUpData.text);

        if(DataList == null)
        {
            Debug.Log("[GachaSystem] Not Find Json Gacha Data");
            return UniTask.CompletedTask;
        }

        foreach (var item in DataList.groups)
        {
            float weight = item.fWeight;

            if (item.bIsPickUp)
                weight *= PickUpWeight;

            _TotalPickWeight += weight;
            _PickUpList.Add((item.iID, weight));

        }

        return UniTask.CompletedTask;
    }

    #endregion
}
