using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[DisallowMultipleComponent]
public class CameraManager : MonoBehaviour
{
    [Header("Target / Orbit")]
    public Transform target;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    [Header("Speeds")]
    public float rotationSpeed = 0.2f; // 픽셀당 각도
    public float zoomSpeed = 0.02f;    // 입력 스케일

    [Header("Zoom Limits")]
    public float minDistance = 5f;
    public float maxDistance = 20f;

    private float yaw;
    private float pitch;
    private float distance;

#if UNITY_EDITOR
    private Vector2 lastMousePos;
    private bool leftDragging;
    private bool rightDragging;
#endif

    void OnEnable()
    {
        EnhancedTouchSupport.Enable(); // 모바일 터치 활성화
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        if (target == null)
        {
            var go = new GameObject("CameraTarget");
            go.transform.position = Vector3.zero;
            target = go.transform;
        }

        Vector3 toCam = transform.position - target.position;
        distance = Mathf.Clamp(toCam.magnitude, minDistance, maxDistance);

        Vector3 dir = toCam.normalized;
        yaw   = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        pitch = Mathf.Asin(dir.y) * Mathf.Rad2Deg;

        ApplyCameraTransform();
    }

    void Update()
    {
#if UNITY_EDITOR
        UpdateEditorInput();
#else
        UpdateTouchInput();
#endif
        ApplyCameraTransform();
    }

    // ==========================
    // 모바일 터치
    // ==========================
    void UpdateTouchInput()
    {
        var touches = Touch.activeTouches;

        if (touches.Count == 1)
        {
            // 한손 회전
            RotateFromDelta(touches[0].delta);
        }
        else if (touches.Count >= 2)
        {
            // 두손 핀치 줌
            var t0 = touches[0];
            var t1 = touches[1];

            Vector2 p0 = t0.screenPosition;
            Vector2 p1 = t1.screenPosition;
            Vector2 p0Prev = p0 - t0.delta;
            Vector2 p1Prev = p1 - t1.delta;

            float prevDist = Vector2.Distance(p0Prev, p1Prev);
            float curDist  = Vector2.Distance(p0, p1);
            float diff = curDist - prevDist;

            Zoom(diff * zoomSpeed);
        }
    }

#if UNITY_EDITOR
    // ==========================
    // 에디터용 마우스 시뮬레이션
    // ==========================
    void UpdateEditorInput()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 pos = mouse.position.ReadValue();

        // --- 왼쪽 버튼: 회전 ---
        if (mouse.leftButton.wasPressedThisFrame)
        {
            leftDragging = true;
            lastMousePos = pos;
        }
        if (mouse.leftButton.isPressed && leftDragging)
        {
            Vector2 delta = pos - lastMousePos;
            RotateFromDelta(delta);
            lastMousePos = pos;
        }
        if (mouse.leftButton.wasReleasedThisFrame)
            leftDragging = false;

        // --- 오른쪽 버튼: 핀치 줌 시뮬레이션 ---
        if (mouse.rightButton.wasPressedThisFrame)
        {
            rightDragging = true;
            lastMousePos = pos;
        }
        if (mouse.rightButton.isPressed && rightDragging)
        {
            float dy = pos.y - lastMousePos.y;
            Zoom(dy * zoomSpeed * 10f);
            lastMousePos = pos;
        }
        if (mouse.rightButton.wasReleasedThisFrame)
            rightDragging = false;

        // --- 마우스 휠 줌 ---
        float wheelY = mouse.scroll.ReadValue().y;
        if (Mathf.Abs(wheelY) > 0.01f)
        {
            Zoom(wheelY * zoomSpeed * 20f);
        }
    }
#endif

    // ==========================
    // 카메라 제어
    // ==========================
    void RotateFromDelta(Vector2 delta)
    {
        yaw   += delta.x * rotationSpeed;
        pitch -= delta.y * rotationSpeed;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void Zoom(float amount)
    {
        distance = Mathf.Clamp(distance - amount, minDistance, maxDistance);
    }

    void ApplyCameraTransform()
    {
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 dir = rot * Vector3.forward;
        transform.position = target.position - dir * distance;
        transform.rotation = rot;
    }
}
