using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Monster : MonoBehaviour
{
    // To scriptable object?
    public bool isDead;
    public int health = 100;
    public float speed = 1f;
    public TMP_Text healthText;
    [HideInInspector] public int pathPointIndex;

    Transform lastPathPoint;
    Transform nextPathPoint;
    float distance;
    [HideInInspector] public float travelledDistance = 0f;

    public float totalPathProgress => (pathPointIndex + travelledDistance / distance);

    public SerializedMonster GetSerialized()
    {
        return new SerializedMonster(health, pathPointIndex, travelledDistance);
    }

    public void Initialize(int health = 100, int pathPointIndex = 0, float travelledDistance = 0f)
    {
        this.health = health;
        this.pathPointIndex = pathPointIndex;
        this.travelledDistance = travelledDistance;

        lastPathPoint = Map.instance.pathPoints[pathPointIndex];
        if (pathPointIndex >= Map.instance.pathPoints.Length - 1) {
            nextPathPoint = null;
        } else {
            nextPathPoint = Map.instance.pathPoints[pathPointIndex+1];
            distance = (nextPathPoint.position - lastPathPoint.position).magnitude;
            transform.position = Vector3.Lerp(lastPathPoint.position, nextPathPoint.position, travelledDistance / distance);
        }

        healthText.text = health.ToString();
    }

    public void ReceiveDamage(int damage)
    {
        health -= damage;
        healthText.text = health.ToString();
        if (health <= 0) {
            Game.instance.RemoveMonster(this);
            isDead = true;
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (nextPathPoint == null) return;

        travelledDistance += speed * Time.deltaTime;

        if (travelledDistance < distance) {
            transform.position = Vector3.Lerp(lastPathPoint.position, nextPathPoint.position, travelledDistance / distance);
        } else {
            lastPathPoint = nextPathPoint;
            travelledDistance -= distance;
            pathPointIndex++;
            if (pathPointIndex >= Map.instance.pathPoints.Length - 1) {
                nextPathPoint = null;
            } else {
                nextPathPoint = Map.instance.pathPoints[pathPointIndex+1];
                distance = (nextPathPoint.position - lastPathPoint.position).magnitude;
                transform.position = Vector3.Lerp(lastPathPoint.position, nextPathPoint.position, travelledDistance / distance);
            }
        }
    }
}
