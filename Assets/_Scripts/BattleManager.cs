using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public GameObject soldierPrefabTeamA;
    public GameObject soldierPrefabTeamB;
    public int teamACount = 5;
    public int teamBCount = 5;

    public Transform teamASpawnPoint;
    public Transform teamBSpawnPoint;

    void Start()
    {
        SpawnTeam(soldierPrefabTeamA, teamACount, teamASpawnPoint, "TeamA");
        SpawnTeam(soldierPrefabTeamB, teamBCount, teamBSpawnPoint, "TeamB");
    }

    void SpawnTeam(GameObject prefab, int count, Transform spawnPoint, string tag)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
            GameObject soldier = Instantiate(prefab, spawnPoint.position + offset, Quaternion.identity);
            soldier.tag = tag;
        }
    }
}
