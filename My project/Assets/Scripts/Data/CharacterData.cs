using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TurnCardGame.Data
{
    public enum EATTAK_TYPE { Attacker, Mage, END };

    public class CharacterInfo
    {
        public int CharacterId { get; set; }

        public string NameEn { get; set; }
        public string NameKo { get; set; }

        public int MaxHp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }

        public int NormalActionId { get; set; }
        public int SkillActionId { get; set; }
    }

    [CreateAssetMenu(menuName = "Turn Card Game/Character", fileName = "CharacterData")]
    public sealed class CharacterData : ScriptableObject
    {
        public int Id => _ID;
        public EATTAK_TYPE ATKType => _ATKType;

        // Icon Image Key
        public AssetReferenceAtlasedSprite CharacterIcon => _CharacterImage;

        // Animdation Data
        public CharacterFsmConfig FSMConfig => _FSMConfig;
        public TextAsset AnimationDatas => _AnimationDatas;
        public string SkeletonDataKey => _SkeletonDataKey;
        public bool AnimReverse => _AnimationReverse;

        [SerializeField] private int    _ID;
        [SerializeField] private EATTAK_TYPE _ATKType = EATTAK_TYPE.END;

        [Header("Icon Image")]
        [SerializeField] AssetReferenceAtlasedSprite  _CharacterImage;

        [Header("FSM Type")]
        [SerializeField] CharacterFsmConfig _FSMConfig;

        [Header("Skelton Anmiation")]
        [SerializeField] string             _SkeletonDataKey;
        [SerializeField] bool               _AnimationReverse;
        [SerializeField] private TextAsset  _AnimationDatas = null;
    }

    
    /*
    // CharacterData(원본 설계 데이터)를 기반으로 생성되는
    // 인게임 런타임 캐릭터 상태.
    // 새로운 스탯이 CharacterData에 추가될 때마다
    // 이 클래스에도 대응하는 Current 필드를 추가한다.
    */

    public class CharacterRuntimeData
    {
        public CharacterData SourceAsset { get; private set; }
        public CharacterInfo SourceInfo { get; private set; }

        public int CurrentHealth { get; private set; }
        public int CurrentATKPower { get; private set; }

        // 앞으로 스탯 추가 시 여기에 계속 추가
        public bool IsDead => CurrentHealth <= 0;
        public float HealthRatio =>
            SourceInfo.MaxHp > 0 ? (float)CurrentHealth / SourceInfo.MaxHp : 0f;

        public CharacterRuntimeData(CharacterData source, CharacterInfo Info)
        {
            SourceAsset = source;
            SourceInfo = Info;

            CurrentHealth = Info.MaxHp;
            CurrentATKPower = Info.Attack;
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0) return;
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        }

        public void Heal(int amount)
        {
            if (amount <= 0) return;
            CurrentHealth = Mathf.Min(SourceInfo.MaxHp, CurrentHealth + amount);
        }

        public void ResetState()
        {
            CurrentHealth = SourceInfo.MaxHp;
            CurrentATKPower = SourceInfo.Attack;
        }
    }
}
