using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapManager : MonoBehaviour
{
    [Serializable]
    public class MapData
    {
        public string name;
        public GameObject prefab;
        public NavMeshData navMeshData;
    }

    public List<MapData> maps = new List<MapData>();

    private GameObject currentMapInstance;
    private NavMeshDataInstance navMeshDataInstance;

    public static MapManager Instance { get; private set; }

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void LoadMap(string mapName)
    {
        UnloadMap();

        // 원하는 맵 검색
        MapData map = maps.Find(m => m.name == mapName);
        if (map == null) {
            Debug.LogError($"Map '{mapName}' not found!");
            return;
        }

        // 맵 생성
        currentMapInstance = Instantiate(map.prefab);

        // NavMeshData 적용
        if (map.navMeshData != null)
            navMeshDataInstance = NavMesh.AddNavMeshData(map.navMeshData);
    }

    public void UnloadMap()
    {
        if (currentMapInstance != null) Destroy(currentMapInstance);
        if (navMeshDataInstance.valid) navMeshDataInstance.Remove();
    }
}