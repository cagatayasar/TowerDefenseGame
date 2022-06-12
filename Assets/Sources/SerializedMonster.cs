using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedMonster
{
    public Monster.Type monsterType;
    public int health;
    public int pathPointIndex;
    public float travelledDistance;

    public SerializedMonster(Monster.Type monsterType, int health, int pathPointIndex, float travelledDistance)
    {
        this.monsterType = monsterType;
        this.health = health;
        this.pathPointIndex = pathPointIndex;
        this.travelledDistance = travelledDistance;
    }
}
