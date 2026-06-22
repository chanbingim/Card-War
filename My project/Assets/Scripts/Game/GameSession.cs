using System;
using System.Collections.Generic;
using System.Linq;
using TurnCardGame.Data;
using UnityEngine;

namespace TurnCardGame.Game
{
    public sealed class GameSession
    {
        private const int DrawPerTurn = 2;
        private const int MaxHandSize = 5;
        private readonly List<StageData> stages;
        private readonly List<CardData> deck = new List<CardData>();
        private readonly List<CardData> hand = new List<CardData>();
        private readonly List<CardActionRecord> pendingActions = new List<CardActionRecord>();
        private readonly List<MonsterState> monsters = new List<MonsterState>();
        private int deckCursor;
        private int playerGuard;

        public GameSession(IEnumerable<StageData> stages)
        {
            this.stages = stages?.Where(stage => stage != null).ToList() ?? new List<StageData>();
            Phase = GamePhase.Start;
            PlayerHealth = 30;
        }

        public GamePhase Phase { get; private set; }
        public int PlayerHealth { get; private set; }
        public StageData CurrentStage { get; private set; }
        public IReadOnlyList<StageData> Stages => stages;
        public IReadOnlyList<CardData> Hand => hand;
        public IReadOnlyList<CardActionRecord> PendingActions => pendingActions;
        public IReadOnlyList<MonsterState> Monsters => monsters;
        public bool HasLiveMonsters => monsters.Any(monster => !monster.IsDefeated);

        public void StartGame()
        {
            Phase = GamePhase.StageSelect;
            CurrentStage = null;
            hand.Clear();
            pendingActions.Clear();
            monsters.Clear();
            deck.Clear();
            deckCursor = 0;
            PlayerHealth = 30;
            playerGuard = 0;
        }

        public void SelectStage(StageData stage)
        {
            if (stage == null)
            {
                throw new ArgumentNullException(nameof(stage));
            }

            CurrentStage = stage;
            Phase = GamePhase.PlayerTurn;
            hand.Clear();
            pendingActions.Clear();
            monsters.Clear();
            deck.Clear();
            deck.AddRange(stage.StartingDeck.Where(card => card != null));
            deckCursor = 0;
            playerGuard = 0;

            foreach (MonsterData monster in stage.Monsters.Where(monster => monster != null))
            {
                monsters.Add(new MonsterState(monster));
            }

            DrawForTurn();
        }

        public void DrawForTurn()
        {
            if (deck.Count == 0)
            {
                TrimHandToLimit();
                return;
            }

            for (int i = 0; i < DrawPerTurn; i++)
            {
                hand.Add(deck[deckCursor % deck.Count]);
                deckCursor++;
            }

            TrimHandToLimit();
        }

        public bool UseCard(int handIndex, MonsterState target)
        {
            if (Phase != GamePhase.PlayerTurn || handIndex < 0 || handIndex >= hand.Count)
            {
                return false;
            }

            CardData card = hand[handIndex];
            hand.RemoveAt(handIndex);
            string targetId = target?.Data.MonsterId ?? string.Empty;
            pendingActions.Add(new CardActionRecord(card, targetId, card.Power));
            return true;
        }

        public IReadOnlyList<string> ResolveCombat()
        {
            Phase = GamePhase.ResolvingCombat;
            var log = new List<string>();

            foreach (CardActionRecord action in pendingActions)
            {
                ResolvePlayerAction(action, log);
            }

            pendingActions.Clear();

            if (!HasLiveMonsters)
            {
                Phase = GamePhase.StageCleared;
                log.Add($"{CurrentStage.Title} cleared.");
                return log;
            }

            foreach (MonsterState monster in monsters.Where(monster => !monster.IsDefeated))
            {
                int damage = Mathf.Max(0, monster.Data.AttackPower - playerGuard);
                PlayerHealth = Mathf.Max(0, PlayerHealth - damage);
                log.Add($"{monster.Data.Title} attacks for {damage}.");
                playerGuard = Mathf.Max(0, playerGuard - monster.Data.AttackPower);
            }

            DrawForTurn();
            Phase = GamePhase.PlayerTurn;
            return log;
        }

        public MonsterState FirstLiveMonster()
        {
            return monsters.FirstOrDefault(monster => !monster.IsDefeated);
        }

        private void ResolvePlayerAction(CardActionRecord action, List<string> log)
        {
            switch (action.Card.EffectType)
            {
                case CardEffectType.Damage:
                    MonsterState target = monsters.FirstOrDefault(monster => !monster.IsDefeated && monster.Data.MonsterId == action.TargetId) ?? FirstLiveMonster();
                    if (target == null)
                    {
                        return;
                    }

                    target.TakeDamage(action.Value);
                    log.Add($"{action.Card.Title} deals {action.Value} to {target.Data.Title}.");
                    break;
                case CardEffectType.Guard:
                    playerGuard += action.Value;
                    log.Add($"{action.Card.Title} adds {action.Value} guard.");
                    break;
                case CardEffectType.Heal:
                    PlayerHealth = Mathf.Min(30, PlayerHealth + action.Value);
                    log.Add($"{action.Card.Title} heals {action.Value}.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TrimHandToLimit()
        {
            while (hand.Count > MaxHandSize)
            {
                hand.RemoveAt(hand.Count - 1);
            }
        }
    }

    public sealed class MonsterState
    {
        public MonsterState(MonsterData data)
        {
            Data = data;
            Health = data.MaxHealth;
        }

        public MonsterData Data { get; }
        public int Health { get; private set; }
        public bool IsDefeated => Health <= 0;

        public void TakeDamage(int amount)
        {
            Health = Mathf.Max(0, Health - Mathf.Max(0, amount));
        }
    }
}
