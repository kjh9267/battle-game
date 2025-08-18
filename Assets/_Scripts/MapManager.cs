using UnityEngine;
using UnityEngine.AI;

public class MapManager : MonoBehaviour
{
    public GameObject mapPrefab;
    public NavMeshData navMeshDataAsset;

    private GameObject currentMapInstance;
    private NavMeshDataInstance navMeshDataInstance;

    public void LoadMap()
    {
        // 기존 맵과 NavMesh 제거
        UnloadMap();

        // 맵 생성
        currentMapInstance = Instantiate(mapPrefab);

        // NavMeshDataInstance 생성
        navMeshDataInstance = NavMesh.AddNavMeshData(navMeshDataAsset);
    }

    public void UnloadMap()
    {
        if (currentMapInstance != null) Destroy(currentMapInstance);
        if (navMeshDataInstance.valid) navMeshDataInstance.Remove();
    }
}