using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    private NavMeshSurface surface;

    void Awake()
    {
        surface = GetComponent<NavMeshSurface>();
    }

    public void RebuildNavMesh()
    {
        surface.BuildNavMesh();
    }
}
