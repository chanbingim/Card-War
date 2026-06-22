using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TurnCardGame.Data;
using TurnCardGame.Game;
using UnityEngine;

namespace TurnCardGame.Tests.EditMode
{
    public sealed class GameSessionTests
    {
        [Test]
        public void SelectStageDrawsTwoCards()
        {
            GameSession session = CreateSession();

            session.StartGame();
            session.SelectStage(session.Stages[0]);

            Assert.AreEqual(GamePhase.PlayerTurn, session.Phase);
            Assert.AreEqual(2, session.Hand.Count);
        }

        [Test]
        public void DrawForTurnTrimsHandToFiveCards()
        {
            GameSession session = CreateSession();

            session.StartGame();
            session.SelectStage(session.Stages[0]);
            session.DrawForTurn();
            session.DrawForTurn();

            Assert.LessOrEqual(session.Hand.Count, 5);
        }

        [Test]
        public void ResolveCombatClearsStageWhenMonsterDies()
        {
            GameSession session = CreateSession();

            session.StartGame();
            session.SelectStage(session.Stages[0]);
            session.UseCard(0, session.FirstLiveMonster());
            session.UseCard(0, session.FirstLiveMonster());
            session.ResolveCombat();

            Assert.AreEqual(GamePhase.StageCleared, session.Phase);
        }

        private static GameSession CreateSession()
        {
            CardData card = CreateCard("test_strike", "Test Strike", CardEffectType.Damage, 10);
            MonsterData monster = CreateMonster("test_monster", "Test Monster", 12, 1);
            StageData stage = ScriptableObject.CreateInstance<StageData>();
            SetField(stage, "stageId", "test_stage");
            SetField(stage, "title", "Test Stage");
            SetField(stage, "monsters", new List<MonsterData> { monster });
            SetField(stage, "startingDeck", new List<CardData> { card, card, card, card, card, card });
            return new GameSession(new[] { stage });
        }

        private static CardData CreateCard(string id, string title, CardEffectType effectType, int power)
        {
            CardData card = ScriptableObject.CreateInstance<CardData>();
            SetField(card, "cardId", id);
            SetField(card, "title", title);
            SetField(card, "effectType", effectType);
            SetField(card, "power", power);
            return card;
        }

        private static MonsterData CreateMonster(string id, string title, int health, int attack)
        {
            MonsterData monster = ScriptableObject.CreateInstance<MonsterData>();
            SetField(monster, "monsterId", id);
            SetField(monster, "title", title);
            SetField(monster, "maxHealth", health);
            SetField(monster, "attackPower", attack);
            return monster;
        }

        private static void SetField<TTarget, TValue>(TTarget target, string fieldName, TValue value)
        {
            FieldInfo field = typeof(TTarget).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(target, value);
        }
    }
}
