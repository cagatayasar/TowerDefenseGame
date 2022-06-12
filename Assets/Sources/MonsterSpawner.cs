using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner instance;

    public Transform monstersParent;
    public List<Wave> waves;
    public List<Monster> monsterPrefabs;
    [HideInInspector] public List<Monster> monsters = new List<Monster>();
    [HideInInspector] public List<Monster.Type> monsterPool;
    [HideInInspector] public int waveIndex;

    public void Spawn(Monster.Type monsterType, Vector3 position, SerializedMonster sm = null)
    {
        var prefab = monsterPrefabs.First(m => m.monsterType == monsterType);
        var monster = Instantiate(prefab, Vector3.zero, Quaternion.identity, monstersParent);
        if (sm == null) {
            monster.Initialize();
        } else {
            monster.Initialize(sm);
        }
        monsters.Add(monster);
    }

    public void Remove(Monster monster)
    {
        monsters.Remove(monster);
        Game.instance.OnMonsterRemoved(monster.goldDrop);
    }

    public void StartWaveSpawner()
    {
        waveIndex = 0;
        OverlayUI.instance.OnWaveChanged();
        StartCoroutine(SpawnWaves_Coroutine());
    }

    public void StartWaveSpawnerWithState(int waveIndex, List<Monster.Type> monsterPool)
    {
        this.waveIndex = waveIndex;
        this.monsterPool = monsterPool;
        OverlayUI.instance.OnWaveChanged();
        StartCoroutine(SpawnWavesContinue_Coroutine());
    }

    IEnumerator SpawnWavesContinue_Coroutine()
    {
        yield return SpawnMonsterPool_Coroutine(waves[waveIndex], monsterPool);
        yield return SpawnWaves_Coroutine();
    }

    IEnumerator SpawnWaves_Coroutine()
    {
        if (waves.Count == 0) yield break;

        var remainingWaves = waves.Skip(waveIndex);
        foreach (var wave in remainingWaves) {
            yield return new WaitForSeconds(wave.waitBefore);
            waveIndex++;
            OverlayUI.instance.OnWaveChanged();
            OverlayUI.instance.AnnounceWave(waveIndex);

            monsterPool = new List<Monster.Type>();
            foreach (var mg in wave.monsterGroups) {
                for (int i = 0; i < mg.count; i++) {
                    monsterPool.Add(mg.monsterType);
                }
            }

            yield return SpawnMonsterPool_Coroutine(wave, monsterPool);

            yield return new WaitForSeconds(wave.waitAfter);
            OverlayUI.instance.OnWaveChanged();
        }

        while (monsters.Count > 0) {
            yield return null;
        }
        Game.instance.EndGame(true);
    }

    IEnumerator SpawnMonsterPool_Coroutine(Wave wave, List<Monster.Type> monsterPool)
    {
        var totalSpawns = monsterPool.Count;
        for (int i = 0; i < totalSpawns; i++) {
            var selectedMonster = monsterPool[Random.Range(0, monsterPool.Count)];
            monsterPool.Remove(selectedMonster);
            Spawn(selectedMonster, Map.instance.pathPoints[0].position);
            yield return new WaitForSeconds(Mathf.Lerp(wave.spawnTimePeriodStart, wave.spawnTimePeriodEnd, (float) i / totalSpawns));
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
}
