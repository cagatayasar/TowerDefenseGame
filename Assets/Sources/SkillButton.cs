using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum SkillType {
        Damage,
        Freeze,
        PlaceTower,
    }

    public static List<SkillButton> descriptionShownFor;

    public Skill skill;
    public Button button;
    public Image image;
    public TMP_Text manaText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionShownFor.Add(this);
        OverlayUI.instance.ShowSkillDescription(descriptionShownFor[0].skill, transform.position.x);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionShownFor.Remove(this);
        if (descriptionShownFor.Count == 0) {
            OverlayUI.instance.HideSkillDescription();
        }
    }

    public void OnManaChanged()
    {
        button.interactable = Game.instance.mana >= skill.manaCost;
    }

    void Awake()
    {
        descriptionShownFor ??= new List<SkillButton>();
        manaText.text = skill.manaCost.ToString();
        image.sprite = skill.sprite;

        button.onClick.AddListener(() => Game.instance.ExecuteSkill(skill));
    }
}
