using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefenseGame/Skill", fileName = "Skill")]
public class Skill : ScriptableObject
{
    public enum Type {
        Damage,
        Freeze,
        PlaceTower,
    }

    public Skill.Type skillType;
    public int manaCost;
    public string description;
}
