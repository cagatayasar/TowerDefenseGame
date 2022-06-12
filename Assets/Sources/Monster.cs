using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pixelplacement;

public class Monster : MonoBehaviour
{
    public int maxHealth;
    public float speed;

    public SpriteRenderer bodyRenderer;
    public CanvasGroup healthBarCanvasGroup;
    public RectTransform healthBarRedMask;

    [HideInInspector] public float travelledDistance = 0f;
    [HideInInspector] public int pathPointIndex;
    [HideInInspector] public bool isDead;

    Transform lastPathPoint;
    Transform nextPathPoint;
    float distance;
    float healthBarRedMaskWidth;
    int health;

    public float totalPathProgress => (pathPointIndex + travelledDistance / distance);

    public SerializedMonster GetSerialized()
    {
        return new SerializedMonster(health, pathPointIndex, travelledDistance);
    }

    public void Initialize(int maxHealth = 100, int pathPointIndex = 0, float travelledDistance = 0f)
    {
        this.maxHealth = maxHealth;
        this.pathPointIndex = pathPointIndex;
        this.travelledDistance = travelledDistance;

        lastPathPoint = Map.instance.pathPoints[pathPointIndex];
        UpdatePath();

        health = maxHealth;
        healthBarRedMask.sizeDelta = new Vector2(health * healthBarRedMaskWidth / maxHealth, healthBarRedMask.sizeDelta.y); 
    }

    public void ReceiveDamage(int damage)
    {
        if (health == maxHealth) {
            Tween.CanvasGroupAlpha(healthBarCanvasGroup, 1f, 0.2f, 0f);
        }
        health -= damage;
        healthBarRedMask.sizeDelta = new Vector2(health * healthBarRedMaskWidth / maxHealth, healthBarRedMask.sizeDelta.y); 

        if (health <= 0) {
            Game.instance.RemoveMonster(this);
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

        travelledDistance += speed * Time.deltaTime;
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
