using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;

    Monster targetMonster;
    Vector3 targetPosition;
    int damage;

    public void Initialize(Monster targetMonster, int damage)
    {
        this.targetMonster = targetMonster;
        this.targetPosition = targetMonster.transform.position;
        this.damage = damage;
    }

    void Update()
    {
        var deltaMovementMagnitude = speed * Time.deltaTime;
        if (targetMonster != null) {
            targetPosition = targetMonster.transform.position;
        }

        var projection = (targetPosition - transform.position);
        if (deltaMovementMagnitude >= projection.magnitude) {
            if (!targetMonster.isDead) {
                targetMonster?.ReceiveDamage(damage);
            }
            Destroy(gameObject);
            return;
        }

        transform.position += deltaMovementMagnitude * projection.normalized;
    }
}
