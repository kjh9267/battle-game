using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[DisallowMultipleComponent]
public class CameraManager : MonoBehaviour
{
    [Header("Target / Orbit")]
    public Transform target;
    public Transform zoomTarget;

    [Header("Speeds")]
    public float rotationSpeed = 0.1f;
    public float zoomSpeed = 1f;

    [Header("Zoom Limits")]
    public float minOrthoSize = 5f;
    public float maxOrthoSize = 19f;

    [Header("View Angle")]
    public float fixedPitch = 30f;

    private float yaw;
    private Vector3 startZoomTargetPos;

#if UNITY_EDITOR
    private Vector2 lastMousePos;
    private bool leftDragging;
#endif
    
    public static CameraManager Instance { get; private set; }
    private Camera cam;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        cam = GetComponent<Camera>();
        if (cam == null) cam = gameObject.AddComponent<Camera>();
        cam.orthographic = true;
    }

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    void Start()
    {
        if (target == null)
        {
            GameObject go = new GameObject("CameraTarget");
            go.transform.position = Vector3.zero;
            target = go.transform;
        }

        if (zoomTarget == null)
        {
            GameObject go = new GameObject("ZoomTarget");
            go.transform.position = target.position;
            zoomTarget = go.transform;
        }

        startZoomTargetPos = zoomTarget.position;

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minOrthoSize, maxOrthoSize);

        ApplyCameraTransform();
    }

    void Update()
    {
#if UNITY_EDITOR
        UpdateEditorInput();
#else
        UpdateTouchInput();
#endif
        // orthographicSize가 최대일 때 zoomTarget을 시작 위치로 이동
        if (Mathf.Approximately(cam.orthographicSize, maxOrthoSize))
        {
            zoomTarget.position = startZoomTargetPos;
        }
        
        ApplyCameraTransform();
    }

    void UpdateTouchInput()
    {
        var touches = Touch.activeTouches;

        if (touches.Count == 1)
        {
            RotateFromDelta(touches[0].delta);
        }
        else if (touches.Count >= 2)
        {
            Vector2 p0 = touches[0].screenPosition;
            Vector2 p1 = touches[1].screenPosition;
            Vector2 p0Prev = p0 - touches[0].delta;
            Vector2 p1Prev = p1 - touches[1].delta;

            float diff = Vector2.Distance(p0, p1) - Vector2.Distance(p0Prev, p1Prev);
            Vector2 pinchCenter = (p0 + p1) * 0.5f;

            ZoomAtScreenPosition(-diff * zoomSpeed * 0.1f, pinchCenter);
        }
    }

#if UNITY_EDITOR
    void UpdateEditorInput()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;
        Vector2 pos = mouse.position.ReadValue();

        if (mouse.leftButton.wasPressedThisFrame)
        {
            leftDragging = true;
            lastMousePos = pos;
        }
        if (mouse.leftButton.isPressed && leftDragging)
        {
            RotateFromDelta(pos - lastMousePos);
            lastMousePos = pos;
        }
        if (mouse.leftButton.wasReleasedThisFrame) leftDragging = false;

        if (mouse.rightButton.isPressed)
        {
            float dy = pos.y - lastMousePos.y;
            ZoomAtScreenPosition(-dy * zoomSpeed, pos);
            lastMousePos = pos;
        }
    }
#endif

    void RotateFromDelta(Vector2 delta)
    {
        yaw += delta.x * rotationSpeed;
    }

    void ZoomAtScreenPosition(float deltaSize, Vector2 screenPos)
    {
        float oldSize = cam.orthographicSize;

        Vector3 worldBefore = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));

        cam.orthographicSize = Mathf.Clamp(oldSize + deltaSize, minOrthoSize, maxOrthoSize);

        Vector3 worldAfter = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));

        Vector3 diff = worldBefore - worldAfter;
        zoomTarget.position += diff;
    }

    void ApplyCameraTransform()
    {
        Quaternion rot = Quaternion.Euler(fixedPitch, yaw, 0f);
        float height = cam.orthographicSize / Mathf.Tan(fixedPitch * Mathf.Deg2Rad);
        float backOffsetMultiplier = 5f;

        transform.position = zoomTarget.position - rot * Vector3.forward * height * backOffsetMultiplier;
        transform.rotation = rot;
    }
}