using UnityEngine;

public class UnitPlacement : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;

    [Header("Grid Settings")]
    public LayerMask tileLayer;   // 드래그 가능한 타일 레이어
    public LayerMask unitLayer;   // 병사 Layer

    private Vector3 originalPosition;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        Plane plane = new Plane(Vector3.up, 0f);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            offset = transform.position - hitPoint;
            originalPosition = transform.position;
            isDragging = true;
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Plane plane = new Plane(Vector3.up, 0f);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter) + offset;

            if (Physics.Raycast(hitPoint + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f, tileLayer))
            {
                transform.position = hit.point;
            }
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // 같은 팀 병사 확인
        GameObject[] teamUnits = GameObject.FindGameObjectsWithTag("TeamA"); // 필요에 따라 태그 변경
        foreach (var unit in teamUnits)
        {
            if (unit != gameObject && Vector3.Distance(unit.transform.position, transform.position) < 0.1f)
            {
                // 다른 병사와 위치 교환
                Vector3 otherPosition = unit.transform.position;
                unit.transform.position = originalPosition;
                transform.position = otherPosition;
                return;
            }
        }

        // 아무도 없으면 드래그한 위치 그대로
    }
}