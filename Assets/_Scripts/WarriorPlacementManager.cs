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

        // 유닛이 있는지 확인
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit unitHit, 100f, unitLayer))
        {
            Debug.Log("다른 유닛 위에 놓음: " + unitHit.collider.name);

            // 예: 교환 로직 실행
            GameObject otherUnit = unitHit.collider.gameObject;
            if (otherUnit != gameObject)
            {
                Vector3 otherPosition = otherUnit.transform.position;
                otherUnit.transform.position = originalPosition;
                transform.position = otherPosition;
            }
            return;
        }

        // 타일 위에 놓기
        if (Physics.Raycast(ray, out RaycastHit tileHit, 100f, tileLayer))
        {
            transform.position = tileHit.point + Vector3.up * 0.1f;
        }
    }
}