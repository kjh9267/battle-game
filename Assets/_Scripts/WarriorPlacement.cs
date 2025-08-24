using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class WarriorPlacement : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private int activeFingerId = -1;
    private Vector3 originalPosition;

    [Header("Grid Settings")] 
    public LayerMask tileLayer; // 드래그 가능한 타일 레이어
    public LayerMask unitLayer; // 병사 Layer

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("[WarriorPlacementManager] MainCamera를 찾지 못했습니다. 카메라에 MainCamera 태그를 지정하세요.");
    }

    void OnEnable()
    {
#if UNITY_IOS || UNITY_ANDROID
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerMove += OnFingerMove;
        Touch.onFingerUp   += OnFingerUp;
#endif
    }

    void OnDisable()
    {
#if UNITY_IOS || UNITY_ANDROID
        Touch.onFingerDown -= OnFingerDown;
        Touch.onFingerMove -= OnFingerMove;
        Touch.onFingerUp   -= OnFingerUp;
        EnhancedTouchSupport.Disable();
#endif
    }

#if UNITY_IOS || UNITY_ANDROID
    // ---------------- 모바일 터치 입력 ----------------
    private void OnFingerDown(Finger finger)
    {
        if (isDragging || mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(finger.screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            if (hit.collider && (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform)))
            {
                isDragging = true;
                activeFingerId = finger.index;
                originalPosition = transform.position;
            }
        }
    }

    private void OnFingerMove(Finger finger)
    {
        if (!isDragging || finger.index != activeFingerId || mainCamera == null) return;
        MoveWithPointer(finger.screenPosition);
    }

    private void OnFingerUp(Finger finger)
    {
        if (!isDragging || finger.index != activeFingerId || mainCamera == null) return;

        isDragging = false;
        HandleRelease(mainCamera.ScreenPointToRay(finger.screenPosition));
        activeFingerId = -1;
    }
#endif

#if UNITY_EDITOR
    // ---------------- 에디터용 마우스 입력 (InputSystem) ----------------
    void Update()
    {
        if (mainCamera == null) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 200f))
            {
                if (hit.collider && (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform)))
                {
                    isDragging = true;
                    originalPosition = transform.position;
                }
            }
        }
        else if (mouse.leftButton.isPressed && isDragging)
        {
            MoveWithPointer(mouse.position.ReadValue());
        }
        else if (mouse.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
            HandleRelease(mainCamera.ScreenPointToRay(mouse.position.ReadValue()));
        }
    }
#endif

    // ---------------- 공통: 드래그 중 위치 이동 ----------------
    private void MoveWithPointer(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

        // 1) 타일 맞으면 타일 위에 스냅
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, tileLayer))
        {
            transform.position = hit.point + Vector3.up * 0.1f;
        }
        else
        {
            // 2) 아니면 XZ 평면(높이 0)에 맞춰 이동 (즉, 자유 드래그 느낌)
            Plane ground = new Plane(Vector3.up, Vector3.zero);
            if (ground.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                transform.position = hitPoint + Vector3.up * 0.1f;
            }
        }
    }

    // ---------------- 공통: 드래그 놓을 때 ----------------
    private void HandleRelease(Ray ray)
    {
        // 0) 드랍 위치 계산
        Vector3 dropPosition;
        if (Physics.Raycast(ray, out RaycastHit groundHit, 200f, tileLayer))
        {
            dropPosition = groundHit.point;
        }
        else
        {
            Plane ground = new Plane(Vector3.up, Vector3.zero);
            if (ground.Raycast(ray, out float enter))
                dropPosition = ray.GetPoint(enter);
            else
                dropPosition = originalPosition;
        }

        // 1) 주변 병사 탐색
        float checkRadius = 0.5f; // 병사 크기에 맞게 조절
        Collider[] hits = Physics.OverlapSphere(dropPosition, checkRadius, unitLayer);

        GameObject nearestUnit = null;
        float nearestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            GameObject otherUnit = hit.gameObject;
            if (otherUnit == gameObject) continue;

            float dist = Vector3.Distance(dropPosition, otherUnit.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestUnit = otherUnit;
            }
        }

        // 2) 가장 가까운 병사와 교환
        if (nearestUnit != null)
        {
            Vector3 otherPosition = nearestUnit.transform.position;
            nearestUnit.transform.position = originalPosition;
            transform.position = otherPosition + Vector3.up * 0.1f;
            return;
        }

        // 3) 병사가 없으면 타일 위로 스냅
        if (Physics.Raycast(ray, out RaycastHit tileHit, 200f, tileLayer))
        {
            transform.position = tileHit.point + Vector3.up * 0.1f;
        }
        else
        {
            // 4) 타일도 없으면 원래 자리로 복귀
            transform.position = originalPosition;
        }
    }
}