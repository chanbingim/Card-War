using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    // ---- 상태 ----
    private readonly List<ITurnParticipant> _participants;

    public int CurrentTurnIndex { get; private set; } = 0;   // 현재 턴인 참가자의 인덱스
    public int CurrentPhase { get; private set; } = 1;        // 현재 진행 중인 Phase (1부터 시작)
    public bool IsRunning { get; private set; } = false;

    public ITurnParticipant Current => _participants[CurrentTurnIndex];
    public int ParticipantCount => _participants.Count;

    public TurnManager(List<ITurnParticipant> participants)
    {
        if (participants == null || participants.Count == 0)
            throw new ArgumentException("참가자가 최소 1명 이상 필요합니다.");

        _participants = participants;
        foreach (ITurnParticipant participant in participants)
        {
            var Base = participant as TurnParticipantBase;
            Base.RequestTurnEnd += RequestEndTurn;
        }
    }

    public void Begin()
    {
        CurrentTurnIndex = 0;
        CurrentPhase = 1;
        IsRunning = true;

        StartTurn();
    }

    private void StartTurn()
    {
        EventBus.Publish<TurnStartEvent>(new TurnStartEvent(Current.Name));
        Current.TurnBegin();
    }

    public void Update()
    {
        Current.TurnRunning();
    }

    /// 현재 참가자의 턴을 종료하고 다음 참가자로 넘김.
    /// 마지막 참가자였다면 Phase 완료 처리 후 다음 Phase 시작.
    public bool RequestEndTurn(string participantId)
    {
        if (!IsRunning)
            return false;

        if (Current.Name != participantId)
        {
            Console.WriteLine($"거부: {participantId}는 지금 턴이 아님 (현재 턴: {Current.Name})");
            return false;
        }

        if (!Current.IsActive)
        {
            Console.WriteLine($"거부: {participantId}는 비활성 상태");
            return false;
        }

        ExecuteEndTurn();
        return true;
    }

    private void ExecuteEndTurn()
    {
        bool isLast = CurrentTurnIndex == _participants.Count - 1;

        if (isLast)
        {
            //EventBus.Publish<TurnStartEvent>(new TurnStartEvent(Current.Name));
            CurrentTurnIndex = 0;
            CurrentPhase++;
        }
        else
        {
            CurrentTurnIndex++;
        }

        StartTurn();
    }

    public void Stop()
    {
        IsRunning = false;
    }
}
