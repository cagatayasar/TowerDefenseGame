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

    public SkillType skillType;
    public Button button;
    public TMP_Text manaText;
    public int manaCost;
    public string description;

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionShownFor.Add(this);
        OverlayUI.instance.ShowSkillDescription(descriptionShownFor[0]);
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
        button.interactable = Game.instance.mana >= manaCost;
    }

    void Awake()
    {
        descriptionShownFor ??= new List<SkillButton>();
        manaText.text = manaCost.ToString();

        button.onClick.AddListener(() => Game.instance.ExecuteSkill(this));
    }
}
