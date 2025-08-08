using UnityEngine;

public class SoldierSpawner : MonoBehaviour
{
    public GameObject teamAPrefab;
    public GameObject teamBPrefab;
    public int teamACount = 10;
    public int teamBCount = 10;
    public Vector2 spawnAreaSize = new Vector2(20f, 20f);

    void Start()
    {
        SpawnTeam(teamAPrefab, "TeamA", teamACount);
        SpawnTeam(teamBPrefab, "TeamB", teamBCount);
    }

    void SpawnTeam(GameObject prefab, string teamTag, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
            float z = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);
            Vector3 pos = new Vector3(x, 0.5f, z); // 필요에 따라 Y 높이 조정

            GameObject soldier = Instantiate(prefab, pos, Quaternion.identity);
            soldier.tag = teamTag;
        }
    }
}