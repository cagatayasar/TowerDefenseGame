using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public List<SerializedMonster> serializedMonsters = new List<SerializedMonster>();
    public List<SerializedTower> serializedTowers = new List<SerializedTower>();
    public float totalTimePassed;
    public int gold;

    public GameState(List<Monster> monsters, List<Tower> towers, float totalTimePassed, int gold)
    {
        foreach (var m in monsters) {
            serializedMonsters.Add(m.GetSerialized());
        }
        foreach (var t in towers) {
            serializedTowers.Add(t.GetSerialized());
        }
        this.totalTimePassed = totalTimePassed;
        this.gold = gold;
    }
}