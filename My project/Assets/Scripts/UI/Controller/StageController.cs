using System;
using UnityEngine;
using System.Collections.Generic;

public class StageController : MonoBehaviour
{
    [SerializeField] private List<StageClearComponent> StageUIs;

    private void OnEnable()
    {
        SettingData();
    }

    private void SettingData()
    {
        try
        {
            var ClientMgr = GameClientManager.instance;
            if (ClientMgr == null)
                throw new ArgumentException("Not Create Client Manager");

            var Stages = ClientMgr.GetPlayerStages();
            if (Stages == null)
                throw new ArgumentException("Not Find Stage Data");

            for (int i = 0; i < StageUIs.Count; i++)
            {
                if(Stages.Count >= i)
                    StageUIs[i].OpenStage($"{1} - {i + 1}");
            }
        }
        catch (Exception Msg)
        {
            Debug.Log(Msg);
        }
    }
}
