using UnityEngine;

namespace TurnCardGame.Data
{
    public enum CHARACTER_PROPERTY { LIGHT, WATER, FIRE, WIND, END };

    [CreateAssetMenu(menuName = "Turn Card Game/Character", fileName = "CharacterData")]
    public sealed class CharacterData : ScriptableObject
    {
        [SerializeField] private int    _ID;
        [SerializeField] private CHARACTER_PROPERTY   _Property = CHARACTER_PROPERTY.END;

        [SerializeField] private int _MaxHealth;
        [SerializeField] private int _ATKPower;

        public int                  Id => _ID;
        public CHARACTER_PROPERTY   Property => _Property;

        public int                  MaxHealth => Mathf.Max(1, _MaxHealth);
        public int                  AttackPower => Mathf.Max(0, _ATKPower);
    }

    /*
    // CharacterData(원본 설계 데이터)를 기반으로 생성되는
    // 인게임 런타임 캐릭터 상태.
    // 새로운 스탯이 CharacterData에 추가될 때마다
    // 이 클래스에도 대응하는 Current 필드를 추가한다.
    */

    public class CharacterRuntimeData
    {
        public CharacterData Source { get; private set; }

        public int CurrentHealth { get; private set; }
        public int CurrentATKPower { get; private set; }

        // 앞으로 스탯 추가 시 여기에 계속 추가
        public bool IsDead => CurrentHealth <= 0;
        public float HealthRatio =>
            Source.MaxHealth > 0 ? (float)CurrentHealth / Source.MaxHealth : 0f;

        public CharacterRuntimeData(CharacterData source)
        {
            Source = source;
            CurrentHealth = source.MaxHealth;
            CurrentATKPower = source.AttackPower;
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0) return;
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        }

        public void Heal(int amount)
        {
            if (amount <= 0) return;
            CurrentHealth = Mathf.Min(Source.MaxHealth, CurrentHealth + amount);
        }

        public void ResetState()
        {
            CurrentHealth = Source.MaxHealth;
            CurrentATKPower = Source.AttackPower;
        }
    }
}
