using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static Game instance;

    public Transform towersParent;
    public Transform monstersParent;
    public Transform projectilesParent;
    public Button placeTowerButton;
    public Tower towerPrefab;
    public Monster monsterPrefab;

    float monsterSpawnTimer = 0f;
    int gold = 0;

    void PlaceTower()
    {
        var count = Map.instance.availableTowerPoints.Count;
        if (count == 0)
            return;

        var point = Map.instance.availableTowerPoints[Random.Range(0, count)];
        var tower = Instantiate(towerPrefab, point.transform.position, Quaternion.identity, towersParent);

        Map.instance.availableTowerPoints.Remove(point);
    }

    void Update()
    {
        monsterSpawnTimer += Time.deltaTime;
        if (monsterSpawnTimer > 1f / (1f + Time.time * 0.1f)) {
            monsterSpawnTimer = 0f;
            Instantiate(monsterPrefab, Map.instance.pathPoints[0].position, Quaternion.identity, monstersParent);
        }

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.UpArrow)) {
            if (Time.timeScale * 2.0f < 100.0f)
                Time.timeScale *= 2.0f;
            else
                Time.timeScale = 99.0f;
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
        placeTowerButton.onClick.AddListener(PlaceTower);
    }
}
