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

public class Game : MonoBehaviour
{
    public static Game instance;

    public Transform towersParent;
    public Transform monstersParent;
    public Transform projectilesParent;
    public Button placeTowerButton;
    public Button saveButton;
    public Button loadSaveButton;
    public Button deleteSaveButton;
    public TMP_Text timeText;
    public TMP_Text monsterKillCountText;
    public TMP_Text goldText;
    public TMP_Text manaText;
    public TMP_Text towerCostText;
    public Tower towerPrefab;
    public Monster monsterPrefab;

    List<Monster> monsters = new List<Monster>();
    List<Tower> towers = new List<Tower>();
    float monsterSpawnTimer = 0f;
    float totalTimePassed = 0f;
    int towerCost = 10;
    int monsterKillCount = 0;
    int gold = 100;
    int mana = 0;

    public void RemoveMonster(Monster monster)
{
        monsterKillCount++;
        monsterKillCountText.text = monsterKillCount.ToString();

        gold += 10;
        goldText.text = gold.ToString();
        placeTowerButton.interactable = gold >= towerCost;

        monsters.Remove(monster);
    }

    void PlaceTower()
    {
        var count = Map.instance.towerPoints.Count;
        if (count == 0)
            return;

        gold -= towerCost;
        goldText.text = gold.ToString();
        towerCost += 10;
        placeTowerButton.interactable = gold >= towerCost;
        towerCostText.text = towerCost.ToString();

        var availablePointIndexes = Map.instance.towerPointAvailability
            .Select((boolValue, i) => (boolValue, i)).Where(x => x.boolValue).Select(x => x.i).ToList();
        if (availablePointIndexes.Count == 0)
            return;
        
        var index = availablePointIndexes[Random.Range(0, availablePointIndexes.Count)];
        var point = Map.instance.towerPoints[index];
        var tower = Instantiate(towerPrefab, point.transform.position, Quaternion.identity, towersParent);
        tower.Initialize(index, Random.Range(10, 51));
        towers.Add(tower);
        Map.instance.towerPointAvailability[index] = false;
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
        totalTimePassed = gameState.totalTimePassed;
        gold = gameState.gold;
    }

    void SaveGame()
    {
        var gameState = new GameState(monsters, towers, totalTimePassed, gold);
        var bf = new BinaryFormatter();
        var path = Application.persistentDataPath + "/ManualSave.dat";
        var stream = new FileStream(path, FileMode.Create);

        bf.Serialize(stream, gameState);
        stream.Close();
    }

    GameState LoadSave()
    {
        var path = Application.persistentDataPath + "/ManualSave.dat";
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
        var path = Application.persistentDataPath + "/ManualSave.dat";
        if (File.Exists(path)) {
            File.Delete(path);
        }
    }

    void Update()
    {
        monsterSpawnTimer += Time.deltaTime;
        totalTimePassed += Time.deltaTime;
        if (monsterSpawnTimer > 2 / (1f + totalTimePassed * 0.03f)) {
            monsterSpawnTimer = 0f;
            var monster = Instantiate(monsterPrefab, Map.instance.pathPoints[0].position, Quaternion.identity, monstersParent);
            monster.Initialize();
            monsters.Add(monster);
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
        UnityAction loadAndInitialize = () => InitializeWithState(LoadSave());

        placeTowerButton.onClick.AddListener(PlaceTower);
        saveButton.onClick.AddListener(SaveGame);
        loadSaveButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        deleteSaveButton.onClick.AddListener(DeleteSave);

        loadAndInitialize();

        monsterKillCountText.text = monsterKillCount.ToString();
        goldText.text = gold.ToString();
        manaText.text = mana.ToString();
        towerCostText.text = towerCost.ToString();
    }
}
