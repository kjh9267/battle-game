using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    public Warrior target;                 // 체력 참조할 유닛
    public GameObject sliderPrefab;        // Slider 프리팹
    public Vector3 worldOffset = new Vector3(0, 2f, 0); // 머리 위 위치

    private Camera cam;
    private Slider slider;
    private RectTransform sliderTransform;

    void Start()
    {
        cam = Camera.main;

        // Overlay 모드용 Canvas 찾기
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas 없음!");
            return;
        }

        // Slider 생성
        GameObject sliderObj = Instantiate(sliderPrefab, canvas.transform);
        slider = sliderObj.GetComponent<Slider>();
        sliderTransform = slider.GetComponent<RectTransform>();

        if (target != null)
        {
            slider.maxValue = target.maxHealth;
            slider.value = target.currentHealth;
        }
    }

    void LateUpdate()
    {
        if (target == null || slider == null) return;

        // 1. 체력 갱신
        slider.value = target.currentHealth;

        // 2. 월드 → 스크린 좌표 변환
        Vector3 worldPos = target.transform.position + worldOffset;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        // 3. 화면 뒤쪽일 경우 숨김
        if (screenPos.z < 0)
        {
            slider.gameObject.SetActive(false);
            return;
        }
        else
        {
            slider.gameObject.SetActive(true);
        }

        // 4. Overlay 모드에서는 그냥 스크린 좌표 그대로 적용
        sliderTransform.position = screenPos;
    }

    private void OnDestroy()
    {
        if (slider != null)
        {
            Destroy(slider.gameObject);
        }
    }
}