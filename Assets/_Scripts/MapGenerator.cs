using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject stairPrefab;

    public int width = 10;
    public int height = 10;
    public float tileSize = 1f;

    int[,] mapData = new int[,]
    {
        {0,0,0,0,0,0,0,0,0,0},
        {0,1,1,1,0,0,2,2,0,0},
        {0,1,0,1,0,0,2,0,0,0},
        {0,1,0,1,0,0,0,0,0,0},
        {0,0,0,1,0,0,0,1,1,0},
        {0,0,0,1,0,0,0,1,0,0},
        {0,0,0,1,1,1,1,1,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0}
    };

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        for(int x=0; x < width; x++)
        {
            for(int z=0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * tileSize, 0, z * tileSize);
                GameObject prefabToSpawn = null;

                int tileType = mapData[z, x];

                if (tileType == 0)
                    prefabToSpawn = floorPrefab;
                else if (tileType == 1)
                    prefabToSpawn = wallPrefab;
                else if (tileType == 2)
                    prefabToSpawn = stairPrefab;

                if(prefabToSpawn != null)
                    Instantiate(prefabToSpawn, pos, Quaternion.identity, transform);
            }
        }
    }
}