using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    public GameObject _1; // 평지
    public GameObject _2; // 계단
    public GameObject _3; // 기타(벽, 장애물 등)

    public int width = 10;
    public int height = 10;

    // 여러 개의 미리 저장된 맵 데이터
    private int[][,] maps = new int[][,]
    {
        // 맵 1
        new int[,]
        {
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,3,1,1,1,1},
            {1,1,1,1,1,2,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
        },
        // 맵 2
        new int[,]
        {
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1},
        }
    };

    private int[,] mapData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 원하는 맵 인덱스를 선택해서 로드
        LoadMap(0); // 0번 맵 로드
        GenerateMap();
    }

    public void LoadMap(int index)
    {
        if (index < 0 || index >= maps.Length)
        {
            Debug.LogError("맵 인덱스가 범위를 벗어남");
            return;
        }

        mapData = maps[index];
    }

    void GenerateMap()
    {
        if (mapData == null)
        {
            Debug.LogError("맵 데이터가 로드되지 않음");
            return;
        }

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(x, 0, z);
                GameObject prefabToSpawn = null;

                int tileType = mapData[z, x];

                if (tileType == 1)
                    prefabToSpawn = _1;
                else if (tileType == 2)
                    prefabToSpawn = _2;
                else
                    prefabToSpawn = _3;

                if (prefabToSpawn != null)
                {
                    Quaternion rot = Quaternion.identity;

                    if (tileType == 3)
                        rot = Quaternion.Euler(0, 180f, 0);
                    else if (tileType == 4)
                        rot = Quaternion.Euler(0, 270f, 0);
                    else if (tileType == 6)
                        rot = Quaternion.Euler(0, 90f, 0);

                    Instantiate(prefabToSpawn, pos, rot, transform);
                }
            }
        }
    }
}
