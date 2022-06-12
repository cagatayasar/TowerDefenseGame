using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pixelplacement;

public class Monster : MonoBehaviour
{
    public enum Type {
        Basic,
        Big,
        Fast,
        Boss
    }

    public static float freezeTimeMultiplier = 1f;

    public Monster.Type monsterType;
    public int maxHealth;
    public int goldDrop;
    public float speed;

    public SpriteRenderer bodyRenderer;
    public CanvasGroup healthBarCanvasGroup;
    public RectTransform healthBarRedMask;

    [HideInInspector] public float travelledDistance = 0f;
    [HideInInspector] public int pathPointIndex = 0;
    [HideInInspector] public bool isDead;

    Transform lastPathPoint;
    Transform nextPathPoint;
    float distance;
    float healthBarRedMaskWidth;
    int health;

    public float totalPathProgress => (pathPointIndex + travelledDistance / distance);

    public SerializedMonster GetSerialized()
    {
        return new SerializedMonster(monsterType, health, pathPointIndex, travelledDistance);
    }

    public void Initialize(SerializedMonster serializedMonster = null)
    {
        if (serializedMonster == null) {
            health = maxHealth;
        } else {
            health = serializedMonster.health;
            pathPointIndex = serializedMonster.pathPointIndex;
            travelledDistance = serializedMonster.travelledDistance;
        }

        lastPathPoint = Map.instance.pathPoints[pathPointIndex];
        UpdatePath();

        healthBarRedMask.sizeDelta = new Vector2(health * healthBarRedMaskWidth / maxHealth, healthBarRedMask.sizeDelta.y); 
    }

    public void ReceiveDamage(int damage)
    {
        if (healthBarCanvasGroup.alpha == 0f) {
            Tween.CanvasGroupAlpha(healthBarCanvasGroup, 1f, 0.2f, 0f);
        }
        health -= damage;
        healthBarRedMask.sizeDelta = new Vector2(health * healthBarRedMaskWidth / maxHealth, healthBarRedMask.sizeDelta.y); 

        if (health <= 0) {
            MonsterSpawner.instance.Remove(this);
            isDead = true;
            Destroy(gameObject);
        }
    }

    void UpdatePath()
    {
        if (pathPointIndex >= Map.instance.pathPoints.Length - 1) {
            nextPathPoint = null;
        } else {
            nextPathPoint = Map.instance.pathPoints[pathPointIndex+1];
            var displacement = nextPathPoint.position - lastPathPoint.position;
            distance = displacement.magnitude;
            transform.position = Vector3.Lerp(lastPathPoint.position, nextPathPoint.position, travelledDistance / distance);

            var newZAngle = 90f + Vector3.SignedAngle(Vector3.up, displacement.normalized, Vector3.forward);
            Tween.Rotation(bodyRenderer.transform, new Vector3(0f, 0f, newZAngle), 0.2f, 0, Tween.EaseOut);
        }
    }

    void Update()
    {
        if (nextPathPoint == null) return;
        if (Game.instance.isPaused) return;

        travelledDistance += speed * Time.deltaTime * freezeTimeMultiplier;
        if (travelledDistance < distance) {
            transform.position = Vector3.Lerp(lastPathPoint.position, nextPathPoint.position, travelledDistance / distance);
        } else {
            lastPathPoint = nextPathPoint;
            travelledDistance -= distance;
            pathPointIndex++;
            UpdatePath();
        }
    }

    void Awake()
    {
        healthBarRedMaskWidth = healthBarRedMask.sizeDelta.x;
    }
}
