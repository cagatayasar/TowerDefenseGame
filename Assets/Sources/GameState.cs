using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public List<SerializedMonster> serializedMonsters = new List<SerializedMonster>();
    public List<SerializedTower> serializedTowers = new List<SerializedTower>();
    public List<Monster.Type> spawnerMonsterPool;
    public int waveIndex;
    public float timePassed;
    public int gold;
    public int mana;
    public int killCount;
    public int towerCost;
    public int castleHealth;

    public GameState(List<Monster> monsters, List<Tower> towers, List<Monster.Type> remainingMonsterPool, int waveIndex,
        float totalTimePassed, int gold, int mana, int killCount, int towerCost, int castleHealth)
    {
        foreach (var m in monsters) {
            serializedMonsters.Add(m.GetSerialized());
        }
        foreach (var t in towers) {
            serializedTowers.Add(t.GetSerialized());
        }
        this.spawnerMonsterPool = remainingMonsterPool;
        this.waveIndex = waveIndex;
        this.timePassed = totalTimePassed;
        this.gold = gold;
        this.mana = mana;
        this.killCount = killCount;
        this.towerCost = towerCost;
        this.castleHealth = castleHealth;
    }
}