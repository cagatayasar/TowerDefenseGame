using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedTower
{
    public int damage;
    public float attackTimer;
    public int towerPointIndex;

    public SerializedTower(int damage, float attackTimer, int towerPointIndex)
    {
        this.damage = damage;
        this.attackTimer = attackTimer;
        this.towerPointIndex = towerPointIndex;
    }
}
