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

public class Game : MonoBehaviour
{
    public const string saveFileName = "/ManualSave.dat";

    public static Game instance;

    public Transform towersParent;
    public Transform monstersParent;
    public Transform projectilesParent;
    public Button placeTowerButton;
    public Button saveButton;
    public Button loadSaveButton;
    public Button deleteSaveButton;
    public Button retryButton;
    public TMP_Text timeText;
    public TMP_Text monsterKillCountText;
    public TMP_Text goldText;
    public TMP_Text manaText;
    public TMP_Text towerCostText;
    public TMP_Text resultsTimeText;
    public TMP_Text resultsWaveText;
    public TMP_Text resultsMonsterKillCountText;
    public CanvasGroup bgShadowCanvasGroup;
    public RectTransform endGameWindow;
    public Castle castle;
    public Tower towerPrefab;
    public Monster monsterPrefab;
    public GameAttributes gameAttributes;

    [HideInInspector] public bool isPaused;

    List<Monster> monsters = new List<Monster>();
    List<Tower> towers = new List<Tower>();
    Tower draggedTower;
    float monsterSpawnTimer = 0f;
    float totalTimePassed = 0f;
    int towerCost;
    int monsterKillCount;
    int gold;
    int mana;

    public void RemoveMonster(Monster monster)
    {
        monsterKillCount++;
        monsterKillCountText.text = monsterKillCount.ToString();

        gold += gameAttributes.monsterGoldDrop;
        goldText.text = gold.ToString();
        placeTowerButton.interactable = gold >= towerCost && Map.instance.towerPoints.Count > towers.Count;

        monsters.Remove(monster);
    }

    public void EndGame()
    {
        isPaused = true;
        resultsTimeText.text = timeText.text;
        resultsWaveText.text = "Wave : 0/7";
        resultsMonsterKillCountText.text = monsterKillCountText.text;

        bgShadowCanvasGroup.blocksRaycasts = true;
        Tween.CanvasGroupAlpha(bgShadowCanvasGroup, 0.4f, 0.7f, 0f, Tween.EaseOut);
        Tween.AnchoredPosition(endGameWindow, Vector2.zero, 0.7f, 0.2f, Tween.EaseOut);
    }

    void PlaceTower()
    {
        var count = Map.instance.towerPoints.Count;
        if (count == 0)
            return;

        var availablePointIndexes = Map.instance.towerPointAvailability
            .Select((boolValue, i) => (boolValue, i)).Where(x => x.boolValue).Select(x => x.i).ToList();
        if (availablePointIndexes.Count == 0)
            return;

        gold -= towerCost;
        goldText.text = gold.ToString();
        towerCost += gameAttributes.towerCostIncreaseAmount;
        placeTowerButton.interactable = gold >= towerCost && Map.instance.towerPoints.Count > towers.Count;
        towerCostText.text = towerCost.ToString();
        
        var index = availablePointIndexes[Random.Range(0, availablePointIndexes.Count)];
        var point = Map.instance.towerPoints[index];
        var tower = Instantiate(towerPrefab, point.transform.position, Quaternion.identity, towersParent);
        tower.Initialize(index, Random.Range(gameAttributes.towerInitialDamageMin, gameAttributes.towerInitialDamageMax+1));
        towers.Add(tower);
        Map.instance.towerPointAvailability[index] = false;
    }

    void MergeTowers(Tower stationaryTower, Tower draggedTower)
    {
        if (stationaryTower.damage == gameAttributes.towerMergedDamageMax ||
            draggedTower.damage    == gameAttributes.towerMergedDamageMax)
            return;

        stationaryTower.damage = Mathf.Min(gameAttributes.towerMergedDamageMax, stationaryTower.damage + draggedTower.damage);
        stationaryTower.damageText.text = stationaryTower.damage.ToString();
        if (stationaryTower.damage == gameAttributes.towerMergedDamageMax) {
            mana += 1;
            manaText.text = mana.ToString();
            stationaryTower.gunRenderer.enabled = false;
            stationaryTower.gunUpgradedRenderer.enabled = true;
        }

        Map.instance.towerPointAvailability[draggedTower.towerPointIndex] = true;
        towers.Remove(draggedTower);
        Destroy(draggedTower.gameObject);

        placeTowerButton.interactable = gold >= towerCost && Map.instance.towerPoints.Count > towers.Count;
    }

    void InitializeWithState(GameState gameState)
    {
        if (gameState == null)
            return;

        foreach (var sm in gameState.serializedMonsters) {
            var monster = Instantiate(monsterPrefab, Vector3.zero, Quaternion.identity, monstersParent);
            monster.Initialize(sm.health, sm.pathPointIndex, sm.travelledDistance);
            monsters.Add(monster);
        }
        foreach (var st in gameState.serializedTowers) {
            var point = Map.instance.towerPoints[st.towerPointIndex];
            var tower = Instantiate(towerPrefab, point.transform.position, Quaternion.identity, towersParent);
            tower.Initialize(st.towerPointIndex, st.damage, st.attackTimer);
            towers.Add(tower);
            Map.instance.towerPointAvailability[st.towerPointIndex] = false;
        }
        totalTimePassed = gameState.timePassed;
        gold = gameState.gold;
        mana = gameState.mana;
        monsterKillCount = gameState.monsterKillCount;
        towerCost = gameState.towerCost;
        castle.health = gameState.castleHealth;
    }

    void SaveGame()
    {
        var gameState = new GameState(monsters, towers, totalTimePassed, gold, mana, monsterKillCount, towerCost, castle.health);
        var bf = new BinaryFormatter();
        var path = Application.persistentDataPath + saveFileName;
        var stream = new FileStream(path, FileMode.Create);

        bf.Serialize(stream, gameState);
        stream.Close();
        deleteSaveButton.interactable = true;
    }

    GameState LoadSave()
    {
        var path = Application.persistentDataPath + saveFileName;
        if (File.Exists(path)) {
            var bf = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Open);

            var loadedGameState = bf.Deserialize(stream) as GameState;
            stream.Close();
            return loadedGameState;
        }

        Debug.Log("Save file not found in " + path);
        return null;
    }

    void DeleteSave()
    {
        var path = Application.persistentDataPath + saveFileName;
        if (File.Exists(path)) {
            File.Delete(path);
        }
        deleteSaveButton.interactable = false;
    }

    void Update()
    {
        if (isPaused) return;

        monsterSpawnTimer += Time.deltaTime;
        totalTimePassed += Time.deltaTime;
        if (monsterSpawnTimer > 2 / (1f + totalTimePassed * 0.03f)) {
            monsterSpawnTimer = 0f;
            var monster = Instantiate(monsterPrefab, Map.instance.pathPoints[0].position, Quaternion.identity, monstersParent);
            monster.Initialize();
            monsters.Add(monster);
        }

        if (draggedTower == null) {
            if (Input.GetMouseButtonDown(0)) {
                var collider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition), LayerMask.GetMask("TowerDrag"));
                if (collider != null) {
                    var tower = collider.GetComponentInParent<Tower>();
                    if (tower.damage != gameAttributes.towerMergedDamageMax) {
                        draggedTower = tower;
                    }
                    draggedTower?.OnDragStart();
                }
            }
        }
        // "else" is not used here, since on very low frames GetMouseButtonUp/Down can be called on the same frame (IIRC).
        if (draggedTower != null) {
            if (Input.GetMouseButton(0)) {
                draggedTower.OnDrag();
            }
            if (Input.GetMouseButtonUp(0)) {
                draggedTower.OnDragEnd();
                var colliders = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), LayerMask.GetMask("TowerDrag"));
                foreach (var c in colliders) {
                    if (draggedTower.dragCollider != c) {
                        MergeTowers(c.GetComponentInParent<Tower>(), draggedTower);
                        break;
                    }
                }
                draggedTower = null;
            }
        }

        var minutes = (int)(totalTimePassed / 60f);
        var seconds = (int)(totalTimePassed % 60f);
        timeText.text = (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.UpArrow)) {
            if (Time.timeScale * 2.0f < 100.0f) {
                Time.timeScale *= 2.0f;
            } else {
                Time.timeScale = 99.0f;
            }
        }
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.DownArrow)) {
            if (Time.timeScale / 2.0f > 0.01f)
                Time.timeScale /= 2.0f;
        }
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.LeftArrow)) {
            Time.timeScale = 0.01f;
        }
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.RightArrow)) {
            Time.timeScale = 99.0f;
        }
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.R)) {
            Time.timeScale = 1.0f;
        }
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

    void Start()
    {
        gold = gameAttributes.startingGold;
        mana = gameAttributes.startingMana;
        towerCost = gameAttributes.startingTowerCost;

        UnityAction loadAndInitialize = () => InitializeWithState(LoadSave());

        placeTowerButton.onClick.AddListener(PlaceTower);
        saveButton.onClick.AddListener(SaveGame);
        loadSaveButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        deleteSaveButton.onClick.AddListener(DeleteSave);
        retryButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        deleteSaveButton.interactable = File.Exists(Application.persistentDataPath + saveFileName);

        loadAndInitialize();

        placeTowerButton.interactable = gold >= towerCost && Map.instance.towerPoints.Count > towers.Count;
        monsterKillCountText.text = monsterKillCount.ToString();
        goldText.text = gold.ToString();
        manaText.text = mana.ToString();
        towerCostText.text = towerCost.ToString();
    }
}
