using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{
    private Transform target;

    public void Init(Transform target)
    {
        this.target = target;
        gameObject.SetActive(true);
        StopAllCoroutines(); // 혹시 이전 코루틴 돌고있으면 중단
        StartCoroutine(FlyToTarget());
    }

    private IEnumerator FlyToTarget()
    {
        if (target == null)
        {
            gameObject.SetActive(false);
            yield break;
        }

        Vector3 startPos = transform.position;
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 targetPos = target.position + Vector3.up * 1.0f;

        while (elapsed < duration)
        {
            if (target == null) break; 

            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false); // 다시 꺼서 재활용
    }
}