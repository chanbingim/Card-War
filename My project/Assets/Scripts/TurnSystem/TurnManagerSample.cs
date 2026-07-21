using System.Collections.Generic;
using UnityEngine;

public class TurnManagerSample : MonoBehaviour
{
    TurnManager turnManager = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<ITurnParticipant> participants = new List<ITurnParticipant>();
        participants.Add(PlayerDataManager.instance.LocalPlayer);

        for (int i = 1; i < 2; i++)
        {
            var player = new PlayerData();
            player.SetName($"Player {i}");

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
