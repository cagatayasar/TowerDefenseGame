using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefenseGame/Game Attributes", fileName = "Game Attributes")]
public class GameAttributes : ScriptableObject
{
    public int startingGold;
    public int startingMana;
    public int startingTowerCost;
    public int towerCostIncreaseAmount;
    public int towerInitialDamageMin;
    public int towerInitialDamageMax;
    public int towerMergedDamageMax;
    public int castleHealth;
    public float waitLengthStart;
    public float waitLengthBetweenWaves;
}
