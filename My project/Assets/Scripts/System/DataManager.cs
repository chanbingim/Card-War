using Cysharp.Threading.Tasks;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections.Generic;
using System.IO;
using TurnCardGame.Data;
using UnityEngine;
using UnityEngine.U2D;
using static PlayerData;

public class DataManager : MonoBehaviour
{
    [Header("로드할 CharacterData 경로 (Resources 폴더 기준)")]
    [SerializeField] private TextAsset      _CardData;

    [Header("로드할 CardData 경로 (Resources 폴더 기준)")]
    [SerializeField] private string _CharacterDataloadPath = "SO/Characters";

    [Header("로드할 BmItem 경로 (Resources 폴더 기준)")]
    [SerializeField] private string _BmDataloadPath = "SO/BM";

    private Dictionary<ECurrency, List<CurrencyProductData>> BmDatas = new Dictionary<ECurrency, List<CurrencyProductData>>();
    private Dictionary<int, CharacterData>      CharacterDatas = new Dictionary<int, CharacterData>();
    private Dictionary<int, CardData>           CardDatas = new Dictionary<int, CardData>();

    public CharacterData GetCharacterById(int id)
    {
        if (CharacterDatas.TryGetValue(id, out var data))
            return data;

        UnityEngine.Debug.LogError($"[DataManager] ID {id}에 해당하는 캐릭터 데이터가 없습니다.");
        return null;
    }

    public List<CurrencyProductData> GetBMData(ECurrency type)
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

    public async UniTask InitializeAsync()
    {
        await UniTask.WhenAll(
            LoadAllCharacterData(),
            LoadAllCardData(),
            LoadAllBMData()
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
