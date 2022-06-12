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
    public Transform projectilesParent;
    public Castle castle;
    public Tower towerPrefab;
    public Monster monsterPrefab;
    public GameAttributes gameAttributes;
    public List<Wave> waves;

    [HideInInspector] public bool isPaused;
    [HideInInspector] public int towerCost;
    [HideInInspector] public int killCount;
    [HideInInspector] public int gold;
    [HideInInspector] public int mana;
    [HideInInspector] public List<Tower> towers = new List<Tower>();
    [HideInInspector] public float totalTimePassed;

    Tower draggedTower;

    public void OnMonsterRemoved(int goldDrop)
    {
        killCount++;
        OverlayUI.instance.OnKillCountChanged();

        gold += goldDrop;
        OverlayUI.instance.OnGoldChanged();
    }

    public void EndGame()
    {
        MonsterSpawner.instance.StopAllCoroutines();
        isPaused = true;
        OverlayUI.instance.OnGameEnd();
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
        towerCost += gameAttributes.towerCostIncreaseAmount;
        OverlayUI.instance.OnGoldChanged();
        OverlayUI.instance.OnTowerCostChanged();
        
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
            OverlayUI.instance.OnManaChanged();
            stationaryTower.gunRenderer.enabled = false;
            stationaryTower.gunUpgradedRenderer.enabled = true;
        }

        Map.instance.towerPointAvailability[draggedTower.towerPointIndex] = true;
        towers.Remove(draggedTower);
        Destroy(draggedTower.gameObject);

        OverlayUI.instance.OnTowerCountChanged();
    }

    void InitializeWithState(GameState gameState)
    {
        if (gameState == null) {
            MonsterSpawner.instance.StartWaveSpawner();
            return;
        }

        foreach (var sm in gameState.serializedMonsters) {
            MonsterSpawner.instance.Spawn(sm.monsterType, Vector3.zero, sm);
        }
        MonsterSpawner.instance.StartWaveSpawnerWithState(gameState.waveIndex, gameState.spawnerMonsterPool);
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
        killCount = gameState.killCount;
        towerCost = gameState.towerCost;
        castle.health = gameState.castleHealth;
    }

    void SaveGame()
    {
        var gameState = new GameState(MonsterSpawner.instance.monsters, towers, MonsterSpawner.instance.monsterPool,
            MonsterSpawner.instance.waveIndex, totalTimePassed, gold, mana, killCount, towerCost, castle.health);
        var bf = new BinaryFormatter();
        var path = Application.persistentDataPath + saveFileName;
        var stream = new FileStream(path, FileMode.Create);

        bf.Serialize(stream, gameState);
        stream.Close();
        OverlayUI.instance.OnSaveStateChanged(true);
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
        OverlayUI.instance.OnSaveStateChanged(false);
    }

    void Update()
    {
        if (isPaused) return;

        totalTimePassed += Time.deltaTime;

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

#if UNITY_EDITOR
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
#endif
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

        OverlayUI.instance.placeTowerButton.onClick.AddListener(PlaceTower);
        OverlayUI.instance.saveButton.onClick.AddListener(SaveGame);
        OverlayUI.instance.loadSaveButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        OverlayUI.instance.deleteSaveButton.onClick.AddListener(DeleteSave);
        OverlayUI.instance.retryButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        OverlayUI.instance.OnSaveStateChanged(File.Exists(Application.persistentDataPath + saveFileName));

        loadAndInitialize();

        OverlayUI.instance.OnGoldChanged();
        OverlayUI.instance.OnKillCountChanged();
        OverlayUI.instance.OnTowerCostChanged();
    }
}
