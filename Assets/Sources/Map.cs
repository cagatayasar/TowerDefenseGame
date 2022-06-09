using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class Map : MonoBehaviour
{
    public static Map instance;

    public Tilemap pathTilemap;
    public Tilemap towerTilemap;
    public Transform[] pathPoints;
    public List<Transform> availableTowerPoints;

    void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        pathPoints = new Transform[pathTilemap.transform.childCount];
        for (int i = 0; i < pathTilemap.transform.childCount; i++) {
            pathPoints[i] = pathTilemap.transform.GetChild(i);
        }
#endif

        Gizmos.color = Color.yellow;
        for (int i = 0; i < pathPoints.Length-1; i++) {
            Gizmos.DrawLine(pathPoints[i].position, pathPoints[i+1].position);
        }
        foreach (var point in pathPoints) {
            Gizmos.DrawCube(point.position, 0.1f * Vector3.one);
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

        pathPoints = new Transform[pathTilemap.transform.childCount];
        for (int i = 0; i < pathTilemap.transform.childCount; i++) {
            pathPoints[i] = pathTilemap.transform.GetChild(i);
        }

        availableTowerPoints = new List<Transform>(towerTilemap.transform.childCount);
        for (int i = 0; i < towerTilemap.transform.childCount; i++) {
            availableTowerPoints.Add(towerTilemap.transform.GetChild(i));
        }

        Debug.Log("tilemap.size: " + pathTilemap.size);
        var tile = pathTilemap.GetTile(new Vector3Int(0, 0, 0));
        // Debug.Log(tilemap.);
    }
}
