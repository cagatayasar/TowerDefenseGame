using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Tower : MonoBehaviour
{
    public TMP_Text damageText;
    public Projectile projectilePrefab;

    List<Monster> monstersInRange = new List<Monster>();
    Monster targetMonster;
    float attackTimer;
    float attackTimePeriod = 1f;
    int damage;

    static Monster TargetMonster(List<Monster> monstersInRange)
    {
        if (monstersInRange.Count == 0)
            return null;
        return monstersInRange.Aggregate((m1, m2) => m1.totalPathProgress > m2.totalPathProgress ? m1 : m2);
    }

    void OnTriggerEnter2D(Collider2D other) {
        var monster = other.GetComponent<Monster>();
        monstersInRange.Add(monster);
    }

    void OnTriggerExit2D(Collider2D other) {
        var monster = monstersInRange.Find(m => m.gameObject == other.gameObject);
        monstersInRange.Remove(monster);
        if (targetMonster == monster) {
            targetMonster = null;
        }
    }

    void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer > attackTimePeriod) {
            targetMonster ??= TargetMonster(monstersInRange);
            if (targetMonster != null) {
                Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity, Game.instance.projectilesParent);
                projectile.Initialize(targetMonster, damage);
                attackTimer = 0f;
            }
        }
    }

    void Awake()
    {
        damage = Random.Range(10, 51);
        damageText.text = damage.ToString();
    }
}
