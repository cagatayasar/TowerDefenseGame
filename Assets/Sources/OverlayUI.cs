using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityAction = UnityEngine.Events.UnityAction;
using TMPro;
using Pixelplacement;

public class OverlayUI : MonoBehaviour
{
    public static OverlayUI instance;

    public List<SkillButton> skillButtons;
    public Button placeTowerButton;
    public Button saveButton;
    public Button loadSaveButton;
    public Button deleteSaveButton;

    [Space(10)]
    public TMP_Text waveAnnouncementText;
    public TMP_Text waveText;
    public TMP_Text killCountText;
    public TMP_Text goldText;
    public TMP_Text manaText;
    public TMP_Text towerCostText;

    [Space(10)]
    public TMP_Text resultsMainText;
    public TMP_Text resultsTimeText;
    public TMP_Text resultsWaveText;
    public TMP_Text resultsKillCountText;
    public Button resultsRetryButton;
    public TMP_Text resultsRetryText;

    [Space(10)]
    public TMP_Text skillDescriptionText;
    public CanvasGroup skillDescriptionCanvasGroup;
    public CanvasGroup bgShadowCanvasGroup;
    public RectTransform endGameWindow;

    public void ShowSkillDescription(Skill skill, float xPosition)
    {
        skillDescriptionText.text = skill.description;
        skillDescriptionCanvasGroup.alpha = 1f;
        var pos = skillDescriptionCanvasGroup.transform.position;
        pos.x = xPosition;
        skillDescriptionCanvasGroup.transform.position = pos;
    }

    public void HideSkillDescription()
    {
        skillDescriptionCanvasGroup.alpha = 0f;
    }

    public void OnGameEnd(bool isPlayerWinner)
    {
        resultsMainText.text = isPlayerWinner ? "YOU WIN!" : "GAME OVER";
        resultsRetryText.text = isPlayerWinner ? "Replay?" : "Retry";
        var minutes = (int)(Game.instance.totalTimePassed / 60f);
        var seconds = (int)(Game.instance.totalTimePassed % 60f);
        resultsTimeText.text = (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
        resultsWaveText.text = "Wave: " + MonsterSpawner.instance.waveIndex;
        resultsKillCountText.text = killCountText.text;
        bgShadowCanvasGroup.blocksRaycasts = true;
        Tween.CanvasGroupAlpha(bgShadowCanvasGroup, 0.4f, 0.7f, 0f, Tween.EaseOut);
        Tween.AnchoredPosition(endGameWindow, Vector2.zero, 0.7f, 0.2f, Tween.EaseOut);
    }

    public void OnGoldChanged()
    {
        goldText.text = Game.instance.gold.ToString();
        placeTowerButton.interactable = Game.instance.gold >= Game.instance.towerCost &&
            Map.instance.towerPoints.Count > Game.instance.towers.Count;
    }

    public void OnKillCountChanged()
    {
        killCountText.text = Game.instance.killCount.ToString();
    }

    public void OnManaChanged()
    {
        manaText.text = Game.instance.mana.ToString();
        placeTowerButton.interactable = Game.instance.gold >= Game.instance.towerCost &&
            Map.instance.towerPoints.Count > Game.instance.towers.Count;
        foreach (var skillButton in skillButtons) {
            skillButton.OnManaChanged();
        }
    }

    public void OnWaveChanged()
    {
        waveText.text = "Wave: " + MonsterSpawner.instance.waveIndex;
    }

    public void AnnounceWave(int incomingWaveNumber)
    {
        waveAnnouncementText.text = "Wave: " +  incomingWaveNumber;
        var transparentColor = new Color(1f, 1f, 1f, 0f);
        Tween.Color(waveAnnouncementText, transparentColor, Color.white, 0.5f, 0f, Tween.EaseOut, completeCallback: () => {
            Tween.Color(waveAnnouncementText, Color.white, transparentColor, 1f, 1.5f, Tween.EaseIn);
        });
    }

    public void OnTowerCountChanged()
    {
        placeTowerButton.interactable = Game.instance.gold >= Game.instance.towerCost &&
            Map.instance.towerPoints.Count > Game.instance.towers.Count;
    }

    public void OnTowerCostChanged()
    {
        towerCostText.text = Game.instance.towerCost.ToString();
        placeTowerButton.interactable = Game.instance.gold >= Game.instance.towerCost &&
            Map.instance.towerPoints.Count > Game.instance.towers.Count;
    }

    public void OnSaveStateChanged(bool saveFileExists)
    {
        deleteSaveButton.interactable = saveFileExists;
    }

    void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("This is a singleton, and there are multiple instances of it.");
            Destroy(gameObject);
            return;
        }
    }
}
