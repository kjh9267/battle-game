using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    public GameObject _1;
    public GameObject _2;
    public GameObject _3;
    public GameObject _4;
    public GameObject _5;
    public GameObject _6;

    public int width = 10;
    public int height = 10;
    public float tileSize = 3f;

    int[,] mapData = new int[10, 10];

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
        GenerateNaturalMap();
        GenerateMap();
    }

    void GenerateNaturalMap()
    {
        mapData = new int[height, width];

        // 1. 기본: 가장자리 줄은 평지(1), 내부는 0
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (z < 2 || z >= height - 2 || x < 2 || x >= width - 2)
                    mapData[z, x] = 1; // 가장자리 전부 1
                else
                    mapData[z, x] = 0; // 내부 빈칸
            }
        }

        // 2. 계단 먼저 배치
        for (int z = 1; z < height - 1; z++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                // 이미 계단 위 바닥(6)이거나 인접에 계단이 있으면 패스
                if (mapData[z, x] == 6 || HasAdjacentStair(x, z))
                    continue;

                // 50% 확률로 계단 배치
                if (Random.Range(0, 10) < 5)
                {
                    if (HasAdjacentStair(z, x))
                    {
                        continue;
                    }
                    int stairType = Random.Range(2, 6); // 2~5

                    // 경계 조건 (row/col 제한)
                    if ((stairType == 2 && z == height - 2) || // 2: 위로 나가면 안됨
                        (stairType == 4 && z == 1) ||         // 4: 아래로 나가면 안됨
                        (stairType == 3 && x == width - 2) || // 3: 오른쪽 나가면 안됨
                        (stairType == 5 && x == 1))           // 5: 왼쪽 나가면 안됨
                        continue;

                    // 계단 위 바닥 위치 계산
                    int upX = x, upZ = z;
                    switch (stairType)
                    {
                        case 2: upZ = z + 1; break; // 위
                        case 3: upX = x + 1; break; // 오른쪽
                        case 4: upZ = z - 1; break; // 아래
                        case 5: upX = x - 1; break; // 왼쪽
                    }

                    // 계단 위 바닥 기준으로 충돌 체크
                    if (HasAdjacentStair(upZ, upX))
                    {
                        continue;
                    }
                    
                    // 계단 아래 바닥 위치 계산
                    int downX = x, downZ = z;
                    switch (stairType)
                    {
                        case 2: downZ = z - 1; break; // 위
                        case 3: downX = x - 1; break; // 오른쪽
                        case 4: downZ = z + 1; break; // 아래
                        case 5: downX = x + 1; break; // 왼쪽
                    }

                    if (mapData[downZ, downX] != 1)
                    {
                        continue;
                    }
                    
                    // 계단 배치
                    mapData[z, x] = stairType;
                    mapData[upZ, upX] = 6;
                }
            }
        }

        // 3. 남은 빈칸(0)을 평지(1) 또는 일부 6으로 채움
        for (int z = 1; z < height - 1; z++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if (mapData[z, x] == 0)
                {
                    if (
                        HasAdjacentUpStair(x, z) 
                        && !HasAdjacentStair(z, x) 
                        && Random.Range(0, 2) == 0
                        )
                        mapData[z, x] = 6;
                    else
                        mapData[z, x] = 1;
                }
            }
        }

        for (int z = 1; z < height - 1; z++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if (mapData[z, x] == 1)
                {
                    if (!CanReachEdge(x, z))
                    {
                        mapData[z, x] = 6;
                    }
                }
            }
        }
    }

    private bool HasAdjacentStair(int z, int x)
    {
        if (mapData[z + 1, x] == 2) return true; // 위에 2
        if (mapData[z, x + 1] == 3) return true; // 오른쪽에 3
        if (mapData[z - 1, x] == 4) return true; // 아래에 4
        if (mapData[z, x - 1] == 5) return true; // 왼쪽에 5
        
        return false;
    }

    bool HasAdjacentUpStair(int x, int z)
    {
        int[,] directions = { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };
        for (int i = 0; i < 4; i++)
        {
            int nx = x + directions[i, 0];
            int nz = z + directions[i, 1];
            if (nx >= 0 && nx < width && nz >= 0 && nz < height)
            {
                if (mapData[nz, nx] == 6)
                    return true;
            }
        }

        return false;
    }

    bool CanReachEdge(int startX, int startZ)
    {
        // 시작 타일이 1이 아니면 불가능
        if (mapData[startZ, startX] != 1)
            return false;

        // BFS 준비
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        bool[,] visited = new bool[height, width];

        queue.Enqueue(new Vector2Int(startX, startZ));
        visited[startZ, startX] = true;

        // 방향(상, 하, 좌, 우)
        int[] dx = { 0, 0, -1, 1 };
        int[] dz = { -1, 1, 0, 0 };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int x = current.x;
            int z = current.y;

            // 가장자리 도달 체크
            if (x == 1 || z == 1 || x == width - 1 || z == height - 1)
                return true;

            // 상하좌우 탐색
            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int nz = z + dz[i];

                // 범위 확인
                if (nx < 0 || nx >= width || nz < 0 || nz >= height)
                    continue;

                // 방문 여부, 이동 가능 여부 (1만 가능)
                if (!visited[nz, nx] && mapData[nz, nx] == 1)
                {
                    visited[nz, nx] = true;
                    queue.Enqueue(new Vector2Int(nx, nz));
                }
            }
        }

        // 큐가 비었는데도 도달 못하면 false
        return false;
    }

    void GenerateMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(-12f + x * tileSize, 0, z * tileSize);
                GameObject prefabToSpawn = null;

                int tileType = mapData[z, x];

                if (tileType == 1)
                    prefabToSpawn = _1;
                else if (tileType == 2)
                    prefabToSpawn = _2;
                else if (tileType == 3)
                    prefabToSpawn = _3;
                else if (tileType == 4)
                    prefabToSpawn = _4;
                else if (tileType == 5)
                    prefabToSpawn = _5;
                else
                    prefabToSpawn = _6;

                if (prefabToSpawn != null)
                {
                    Quaternion rot = Quaternion.identity;

                    if (tileType == 2)
                        rot = Quaternion.Euler(0, 180f, 0);
                    else if (tileType == 3)
                        rot = Quaternion.Euler(0, 270f, 0);
                    else if (tileType == 5)
                        rot = Quaternion.Euler(0, 90f, 0);

                    Instantiate(prefabToSpawn, pos, rot, transform);
                }
            }
        }
    }
}