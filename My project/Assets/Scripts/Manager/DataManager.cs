using System;
using System.Collections.Generic;
using TurnCardGame.Data;
using UnityEngine;
using UnityEngine.U2D;

public class DataManager : MonoBehaviour
{
    [Header("로드할 CharacterData 경로 (Resources 폴더 기준)")]
    [SerializeField] private string _CharacterDataloadPath = "Data/Characters";

    private Dictionary<int, CharacterData>  CharacterDatas = new Dictionary<int, CharacterData>();
    private Sprite[] Cardsprites;

    public CharacterData GetById(int id)
    {
        if (CharacterDatas.TryGetValue(id, out var data))
            return data;

        Debug.LogError($"[CharacterDataManager] ID {id}에 해당하는 캐릭터 데이터가 없습니다.");
        return null;
    }

    public bool TryGetById(int id, out CharacterData data)
    {
        return CharacterDatas.TryGetValue(id, out data);
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
        instance.Initialize();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void Initialize()
    {
        LoadAllCharacterData();
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
