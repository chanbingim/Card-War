using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TurnCardGame.Data;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.U2D;
using static CurrencyComponent;

public class DataManager : MonoBehaviour
{
    [Header("로드할 CharacterData 경로 (Resources 폴더 기준)")]
    [SerializeField] private string _CharacterDataloadPath = "SO/Characters";

    [Header("로드할 CardData 경로 (Resources 폴더 기준)")]
    [SerializeField] private string _CardDataloadPath = "SO/Cards";

    [Header("로드할 BmItem 경로 (Resources 폴더 기준)")]
    [SerializeField] private string _BmDataloadPath = "SO/BM";

    private Dictionary<CurrencyType, List<CurrencyProductData>> BmDatas = new Dictionary<CurrencyType, List<CurrencyProductData>>();
    private Dictionary<int, CharacterData>      CharacterDatas = new Dictionary<int, CharacterData>();
    private Dictionary<int, CardData>           CardDatas = new Dictionary<int, CardData>();
    private Sprite[] Cardsprites;


    public CharacterData GetCharacterById(int id)
    {
        if (CharacterDatas.TryGetValue(id, out var data))
            return data;

        UnityEngine.Debug.LogError($"[DataManager] ID {id}에 해당하는 캐릭터 데이터가 없습니다.");
        return null;
    }

    public List<CurrencyProductData> GetBMData(CurrencyType type)
    {
        if (BmDatas.TryGetValue(type, out var data))
            return data;

        UnityEngine.Debug.LogError($"[DataManager] {type}에 해당하는 BM 데이터가 없습니다.");
        return null;
    }

    public bool TryCharacterGetById(int id, out CharacterData data)
    {
        return CharacterDatas.TryGetValue(id, out data);
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

    public Sprite GetCardSprite(int ID) { return Cardsprites[ID]; }

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

    private static readonly ProfilerMarker Marker =
        new ProfilerMarker("DataManager.InitializeAsync");

    public async UniTask InitializeAsync()
    {
        await UniTask.WhenAll(
            LoadAllCharacterData(),
            LoadAllCardData(),
            LoadAllBMData(),
            LoadCardSprites()
        );
    }

    private async UniTask LoadAllCharacterData()
    {
        CharacterData[] allData = Resources.LoadAll<CharacterData>(_CharacterDataloadPath);

        await UniTask.RunOnThreadPool(() =>
        {
            foreach (var data in allData)
            {
                if (CharacterDatas.ContainsKey(data.Id))
                {
                    Debug.LogError($"[CharacterDataManager] 중복된 캐릭터 ID 발견: {data.Id} ({data.name})");
                    continue;
                }

                CharacterDatas.Add(data.Id, data);
            }
        });

        Debug.Log($"[CharacterDataManager] 캐릭터 데이터 {CharacterDatas.Count}개 로드 완료");
    }

    private async UniTask LoadAllCardData()
    {
        CardData[] allData = Resources.LoadAll<CardData>(_CardDataloadPath);
        await UniTask.RunOnThreadPool(() =>
        {
            foreach (var data in allData)
            {
                if (CardDatas.ContainsKey(data.CardId))
                {
                    Debug.LogError($"[DataManager] 중복된 카드 ID 발견: {data.CardId} ({data.name})");
                    continue;
                }

                CardDatas.Add(data.CardId, data);
            }
        });

        Debug.Log($"[DataManager] 카드 데이터 {CharacterDatas.Count}개 로드 완료");
    }

    private async UniTask LoadCardSprites()
    {
        AddressableManager AddressableMgr = AddressableManager.instance;
        if (AddressableMgr == null)
            throw new ArgumentException("어드레서블 매니저 생성 필요");

        SpriteAtlas spriteAtlas = AddressableMgr.Get<SpriteAtlas>("Atlas/CardAtlas");
        if (spriteAtlas == null)
            return;

        Cardsprites = new Sprite[spriteAtlas.spriteCount];
        spriteAtlas.GetSprites(Cardsprites);

        await UniTask.CompletedTask;
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
