using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    public static NavMeshManager Instance { get; private set; }

    private NavMeshSurface surface;
    
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
        surface = GetComponent<NavMeshSurface>();
    }

    public void RebuildNavMesh()
    {
        surface.BuildNavMesh();
    }
}
