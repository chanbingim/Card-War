using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI", menuName = "Scriptable Objects/AI Data")]
public class StageAISO : ScriptableObject
{
    public string              _Name;
    public  List<int>          _Skills;
    public  List<DeckEntry>    _Decks;
    public  List<int>          _Monsters;
}
