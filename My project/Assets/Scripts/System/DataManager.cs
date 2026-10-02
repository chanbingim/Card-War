using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TurnCardGame.Data;
using UnityEngine;
using CharacterInfo = TurnCardGame.Data.CharacterInfo;

public class DataManager : MonoBehaviour
{
    [Header("CardData CSV")]
    [SerializeField] private TextAsset      _CardData;

    [Header("캐릭터 Data CSV 또는 SO")]
    [SerializeField] private TextAsset      _CharacterInfoCSV;
    [SerializeField] private string         _CharacterDataloadPath = "SO/Characters";

    [Header("Action CSV")]
    [SerializeField] private TextAsset      _ActionCSV;

    [Header("로드할 BmItem 경로 (Resources 폴더 기준)")]
    [SerializeField] private string _BmDataloadPath = "SO/BM";

    private Dictionary<ECurrency, List<CurrencyProductData>>     BmDatas = new Dictionary<ECurrency, List<CurrencyProductData>>();
    private Dictionary<int, (CharacterData, CharacterInfo)>      CharacterDatas = new ();

    private Dictionary<int, CardData>                            CardDatas = new Dictionary<int, CardData>();
    private Dictionary<int, SkillInfo> SkillDatas = new();

    public (CharacterData, CharacterInfo) GetCharacterById(int id)
    {
        if (CharacterDatas.TryGetValue(id, out var data))
            return data;

        UnityEngine.Debug.LogError($"[DataManager] ID {id}에 해당하는 캐릭터 데이터가 없습니다.");
        return (null, null);
    }

    public List<CurrencyProductData> GetBMData(ECurrency type)
    {
        if (BmDatas.TryGetValue(type, out var data))
            return data;

        UnityEngine.Debug.LogError($"[DataManager] {type}에 해당하는 BM 데이터가 없습니다.");
        return null;
    }

    public bool TryCharacterGetById(int id, out (CharacterData, CharacterInfo) data)
    {
        return CharacterDatas.TryGetValue(id, out data);
    }

    public bool TryGetSkillByID(int id, out SkillInfo data)
    {
        return SkillDatas.TryGetValue(id, out data);
    }

    public CardData GetCardById(int id)
    {
        if (CardDatas.TryGetValue(id, out var data))
            return data;

        UnityEngine.Debug.LogError($"[DataManager] ID {id}에 해당하는 카드 데이터가 없습니다.");
        return null;
    }

    public bool TryCardDataGetById(int id, out CardData data)
    {
        return CardDatas.TryGetValue(id, out data);
    }

    #region Defualt
    static public DataManager instance { get; private set; }
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

    public async void Initalize()
    {
        await InitializeAsync();

        foreach(var iter in CardDatas)
        {
            iter.Value.ParseData();
        }
    }

    private async UniTask InitializeAsync()
    {
        await UniTask.WhenAll(
            LoadAllCharacterData(),
            LoadSkillDatas(),
            LoadAllCardData(),
            LoadAllBMData()
        );
    }

    private async UniTask LoadAllCharacterData()
    {
        CharacterData[] allData = Resources.LoadAll<CharacterData>(_CharacterDataloadPath);
        var allInfo = CsvReader.ReadCSV<CharacterInfo>(_CharacterInfoCSV);

        await UniTask.RunOnThreadPool(() =>
        {
            foreach (var data in allData)
            {
                if (CharacterDatas.ContainsKey(data.Id))
                {
                    Debug.LogError($"[CharacterDataManager] 중복된 캐릭터 ID 발견: {data.Id} ({data.name})");
                    continue;
                }
               
                CharacterDatas.Add(data.Id, (data, allInfo.FirstOrDefault(info => data.Id == info.CharacterId  )));
            }
        });

        Debug.Log($"[CharacterDataManager] 캐릭터 데이터 {CharacterDatas.Count}개 로드 완료");
    }

    private async UniTask LoadSkillDatas()
    {
        var datas = CsvReader.ReadCSV<SkillInfo>(_ActionCSV);

        await UniTask.RunOnThreadPool(() =>
        {
            foreach (var data in datas)
            {
                if (SkillDatas.ContainsKey(data.ActionId))
                {
                    Debug.LogError($"[CharacterDataManager] 중복된 Skill ID 발견: {data.ActionId} ({data.NameKo})");
                    continue;
                }

                SkillDatas.Add(data.ActionId, data);
            }
        });

        Debug.Log($"[CharacterDataManager] 스킬 데이터 {SkillDatas.Count}개 로드 완료");
    }

    private async UniTask LoadAllCardData()
    {
        var Datas = Utility.ReadCSV<CardData>(_CardData);
        await UniTask.RunOnThreadPool(() =>
        {
            foreach (var data in Datas)
            {
                if (CardDatas.ContainsKey(data.ID))
                {
                    Debug.LogError($"[DataManager] 중복된 카드 ID 발견: {data.ID} ({data.Name})");
                    continue;
                }

                CardDatas.Add(data.ID, data);
            }
        });

        Debug.Log($"[DataManager] 카드 데이터 {CharacterDatas.Count}개 로드 완료");
    }

    private async UniTask LoadAllBMData()
    {
        CurrencyProductData[] allData = Resources.LoadAll<CurrencyProductData>(_BmDataloadPath);

        await UniTask.RunOnThreadPool(() =>
        {
            foreach (var data in allData)
            {
                if (BmDatas.TryGetValue(data.CurrencyType, out var list))
                {
                    list.Add(data);
                }
                else
                {
                    var newList = new List<CurrencyProductData>();
                    newList.Add(data);

                    BmDatas.Add(data.CurrencyType, newList);
                }
            }
        });

        Debug.Log($"[DataManager] BM 데이터 {allData.Length}개 로드 완료");
    }
    #endregion


}
