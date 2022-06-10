using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedMonster
{
    public int health;
    public int pathPointIndex;
    public float travelledDistance;

    public SerializedMonster(int health, int pathPointIndex, float travelledDistance)
    {
        this.health = health;
        this.pathPointIndex = pathPointIndex;
        this.travelledDistance = travelledDistance;
    }
}
