using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public List<SerializedMonster> serializedMonsters = new List<SerializedMonster>();
    public List<SerializedTower> serializedTowers = new List<SerializedTower>();
    public float timePassed;
    public int gold;
    public int mana;
    public int monsterKillCount;
    public int towerCost;
    public int castleHealth;

    public GameState(List<Monster> monsters, List<Tower> towers, float totalTimePassed, int gold, int mana, int monsterKillCount, int towerCost, int castleHealth)
    {
        foreach (var m in monsters) {
            serializedMonsters.Add(m.GetSerialized());
        }
        foreach (var t in towers) {
            serializedTowers.Add(t.GetSerialized());
        }
        this.timePassed = totalTimePassed;
        this.gold = gold;
        this.mana = mana;
        this.monsterKillCount = monsterKillCount;
        this.towerCost = towerCost;
        this.castleHealth = castleHealth;
    }
}