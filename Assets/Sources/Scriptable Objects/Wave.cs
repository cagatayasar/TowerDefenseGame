using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefenseGame/Wave", fileName = "Wave")]
public class Wave : ScriptableObject
{
    [System.Serializable]
    public struct MonsterGroup {
        public Monster.Type monsterType;
        public int count;
    }

    public List<MonsterGroup> monsterGroups;
    public float spawnTimePeriodStart;
    public float spawnTimePeriodEnd;
    public float waitBefore;
    public float waitAfter;
}
