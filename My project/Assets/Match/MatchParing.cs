using System.Collections.Generic;
using UnityEngine;

public class MatchParing : MonoBehaviour
{
    // 각 플레이어가 이전에 누구와 붙었는지 기록
    private Dictionary<int, HashSet<int>> _matchHistory = new();

    public List<(int playerA, int playerB)> CreatePairs(List<int> alivePlayers)
    {
        var pairs = new List<(int, int)>();
        var unpaired = new List<int>(alivePlayers);
        Shuffle(unpaired); // 매 라운드 랜덤성 부여

        while (unpaired.Count > 1)
        {
            int first = unpaired[0];
            unpaired.RemoveAt(0);

            // a와 아직 안 붙어본 상대를 우선적으로 탐색
            int secondIndex = unpaired.FindIndex(secondidx => !_matchHistory[first].Contains(secondidx));
            if (secondIndex == -1) secondIndex = 0; // 다 붙어봤으면 그냥 아무나

            int second = unpaired[secondIndex];
            unpaired.RemoveAt(secondIndex);

            pairs.Add((first, second));
            _matchHistory[second].Add(first);
            _matchHistory[first].Add(second);
        }

        // 홀수라서 남는 한 명 처리
        if (unpaired.Count == 1)
        {
            int ghost = unpaired[0];
            pairs.Add((ghost, -1)); // -1을 "고스트 상대"로 표시
        }

        return pairs;
    }

    private void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1); // 여기는 굳이 암호학적 RNG 안 써도 됨
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
