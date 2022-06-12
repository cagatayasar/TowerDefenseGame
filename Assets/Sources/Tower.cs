using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Pixelplacement;

public class Tower : MonoBehaviour
{
    const float dragMaterialAlpha = 0.37f;

    static Material defaultMaterial = null;
    static Material dragMaterial = null;

    public TMP_Text damageText;
    public Projectile projectilePrefab;
    public Collider2D dragCollider;
    public Transform gunRotator;
    public SpriteRenderer baseRenderer;
    public SpriteRenderer gunRenderer;
    public SpriteRenderer gunUpgradedRenderer;

    [HideInInspector] public int damage;
    [HideInInspector] public int towerPointIndex;
    List<Monster> monstersInRange = new List<Monster>();
    Monster targetMonster;
    float attackTimer;
    float attackTimePeriod = 1f;
    bool isDragged;

    static Monster TargetMonster(List<Monster> monstersInRange)
    {
        if (monstersInRange.Count == 0)
            return null;
        return monstersInRange.Aggregate((m1, m2) => m1.totalPathProgress > m2.totalPathProgress ? m1 : m2);
    }

    public SerializedTower GetSerialized()
    {
        return new SerializedTower(damage, attackTimer, towerPointIndex);
    }

    public void Initialize(int towerPointIndex, int damage, float attackTimer = 0f)
    {
        this.towerPointIndex = towerPointIndex;
        this.damage = damage;
        this.attackTimer = attackTimer;
        damageText.text = damage.ToString();

        if (damage == Game.instance.gameAttributes.towerMergedDamageMax) {
            gunRenderer.enabled = false;
            gunUpgradedRenderer.enabled = true;
        }

        Tween.LocalScale(transform, Vector3.zero * 0.6f, Vector3.one, 0.4f, 0f, Tween.EaseIn);
    }

    public void OnDragStart()
    {
        isDragged = true;
        baseRenderer.material = dragMaterial;
        gunRenderer.material = dragMaterial;
        baseRenderer.sortingOrder += 10;
        gunRenderer.sortingOrder += 10;
    }

    public void OnDrag()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        transform.position = mousePosition;
    }

    public void OnDragEnd()
    {
        isDragged = false;
        baseRenderer.material = defaultMaterial;
        gunRenderer.material = defaultMaterial;
        transform.position = Map.instance.towerPoints[towerPointIndex].position;
        baseRenderer.sortingOrder -= 10;
        gunRenderer.sortingOrder -= 10;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        monstersInRange.Add(other.GetComponent<Monster>());
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
        if (Game.instance.isPaused) return;

        attackTimer += Time.deltaTime;
        if (isDragged) return;

        targetMonster ??= TargetMonster(monstersInRange);
        if (targetMonster != null) {
            var displacement = (targetMonster.transform.position - gunRotator.position).normalized;
            gunRotator.rotation = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(Vector3.up, displacement, Vector3.forward));
        }
        if (attackTimer > attackTimePeriod) {
            if (targetMonster != null) {
                var projectile = Instantiate(projectilePrefab, transform.position, gunRotator.rotation, Game.instance.projectilesParent);
                projectile.Initialize(targetMonster, damage);
                attackTimer = 0f;
            }
        }
    }

    void Awake()
    {
        damage = Random.Range(10, 51);
        damageText.text = damage.ToString();

        if (defaultMaterial == null) {
            defaultMaterial = baseRenderer.material;
            dragMaterial = new Material(baseRenderer.material);
            dragMaterial.SetColor("_Color", new Color(1f, 1f, 1f, dragMaterialAlpha));
        }
    }
}
