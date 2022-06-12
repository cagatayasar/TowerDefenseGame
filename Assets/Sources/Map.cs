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
    public List<Transform> towerPoints;
    public List<bool> towerPointAvailability;

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

        towerPoints = new List<Transform>(towerTilemap.transform.childCount);
        towerPointAvailability = new List<bool>(towerTilemap.transform.childCount);
        for (int i = 0; i < towerTilemap.transform.childCount; i++) {
            towerPoints.Add(towerTilemap.transform.GetChild(i));
            towerPointAvailability.Add(true);
        }
    }
}
