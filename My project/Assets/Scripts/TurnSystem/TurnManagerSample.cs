using System.Collections.Generic;
using UnityEngine;

public class TurnManagerSample : MonoBehaviour
{
    TurnManager turnManager = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        List<ITurnParticipant> participants = new List<ITurnParticipant>();
        for (int i = 0; i < 2; i++)
        {
            var player = new PlayerData();
            player.SetName($"Player {i} ");

            participants.Add(player);
        }

        turnManager = new TurnManager(participants);
        turnManager.Begin();

    }

    // Update is called once per frame
    void Update()
    {
        turnManager.Update();
    }
}
