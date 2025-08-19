using UnityEngine;

public class WarriorPlacementManager : MonoBehaviour
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

        // 카메라에서 현재 마우스/터치 위치로 레이 쏘기
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // 타일 위에 드래그 되도록 처리
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayer))
        {
            // 병사가 마우스를 따라가도록 위치 갱신
            transform.position = hit.point + Vector3.up * 0.1f; // 살짝 띄워 보이게
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // 같은 팀 병사 확인
        GameObject[] teamUnits = GameObject.FindGameObjectsWithTag("TeamA"); // 필요 시 태그 변경
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

        // 아무도 없으면 드래그한 위치 그대로 두기
    }
}