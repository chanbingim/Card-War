using System;
using UnityEngine;

using UnityEngine.UI;
public class DateComponent : MonoBehaviour
{
    [SerializeField] private Text _Text = null;

    public void SettingData(string StartDate, string enddDate)
    {
        if (_Text == null)
            return;

        DateTimeOffset sDate = DateTimeOffset.Parse(StartDate);
        DateTimeOffset eDate = DateTimeOffset.Parse(enddDate);

        _Text.text = $"{sDate.Year}/{sDate.Month}/{sDate.Day} Á¡°ËÈÄ~ " +
                     $"{eDate.Year}/{eDate.Month}/{eDate.Day} {eDate.Hour} : {eDate.Minute}";
    }
}
