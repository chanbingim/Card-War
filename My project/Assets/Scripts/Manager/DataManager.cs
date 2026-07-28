using System;
using System.Collections.Generic;
using TurnCardGame.Data;
using UnityEngine;
using UnityEngine.U2D;

public class DataManager : MonoBehaviour
{
    [Header("로드할 CharacterData 경로 (Resources 폴더 기준)")]
    [SerializeField] private string _CharacterDataloadPath = "SO/Characters";

    [Header("로드할 CardData 경로 (Resources 폴더 기준)")]
    [SerializeField] private string _CardDataloadPath = "SO/Cards";


    private Dictionary<int, CharacterData>  CharacterDatas = new Dictionary<int, CharacterData>();
    private Dictionary<int, CardData>       CardDatas = new Dictionary<int, CardData>();
    private Sprite[] Cardsprites;

    public CharacterData GetCharacterById(int id)
    {
        if (CharacterDatas.TryGetValue(id, out var data))
            return data;

        Debug.LogError($"[CharacterDataManager] ID {id}에 해당하는 캐릭터 데이터가 없습니다.");
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

        Debug.LogError($"[DataManager] ID {id}에 해당하는 카드 데이터가 없습니다.");
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

    public void Initialize()
    {
        LoadAllCharacterData();
        LoadAllCardData();
        LoadCardSprites();
    }

    private void LoadAllCharacterData()
    {
        CharacterData[] allData = Resources.LoadAll<CharacterData>(_CharacterDataloadPath);

        foreach (var data in allData)
        {
            if (CharacterDatas.ContainsKey(data.Id))
            {
                Debug.LogError($"[CharacterDataManager] 중복된 캐릭터 ID 발견: {data.Id} ({data.name})");
                continue;
            }

            CharacterDatas.Add(data.Id, data);
        }

        Debug.Log($"[CharacterDataManager] 캐릭터 데이터 {CharacterDatas.Count}개 로드 완료");
    }

    private void LoadAllCardData()
    {
        CardData[] allData = Resources.LoadAll<CardData>(_CardDataloadPath);

        foreach (var data in allData)
        {
            if (CardDatas.ContainsKey(data.CardId))
            {
                Debug.LogError($"[DataManager] 중복된 카드 ID 발견: {data.CardId} ({data.name})");
                continue;
            }

            CardDatas.Add(data.CardId, data);
        }

        Debug.Log($"[DataManager] 카드 데이터 {CharacterDatas.Count}개 로드 완료");
    }

    private void LoadCardSprites()
    {
        AddressableManager AddressableMgr = AddressableManager.instance;
        if(AddressableMgr == null)
            throw new ArgumentException("어드레서블 매니저 생성 필요");

        SpriteAtlas spriteAtlas = AddressableMgr.Get<SpriteAtlas>("Atlas/CardAtlas");
        if (spriteAtlas == null)
            return;

        Cardsprites = new Sprite[spriteAtlas.spriteCount];
        spriteAtlas.GetSprites(Cardsprites);
    }

    #endregion


}
