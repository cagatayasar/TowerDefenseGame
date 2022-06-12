using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Pixelplacement;
using TweenBase = Pixelplacement.TweenSystem.TweenBase;

public class Castle : MonoBehaviour
{
    public SpriteRenderer baseRenderer;
    public TMP_Text healthText;
    public int health;

    TweenBase activeTween;

    void OnTriggerEnter2D(Collider2D other)
    {
        var monster = other.GetComponent<Monster>();
        var receivedDamage = monster.damageToCastle;
        monster.ReceiveDamage(monster.maxHealth);

        activeTween?.Stop();
        activeTween = Tween.Color(baseRenderer, Color.red, 0.3f, 0f, Tween.EaseOut, completeCallback: () => {
            activeTween = Tween.Color(baseRenderer, Color.white, 0.3f, 0f, Tween.EaseIn);
        });

        health = Mathf.Max(0, health - receivedDamage);
        healthText.text = health.ToString();

        if (health <= 0) {
            Game.instance.EndGame(false);
        }
    }

    void Awake()
    {
        healthText.text = health.ToString();
    }
}
