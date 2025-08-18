using System;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Team Tags")]
    public string teamATag = "TeamA";
    public string teamBTag = "TeamB";

    private bool gameEnded = false;

    public static GameManager Instance { get; private set; }
    
    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        StartCoroutine(CheckTeamsRoutine());
    }

    private IEnumerator CheckTeamsRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);

        while (!gameEnded)
        {
            int teamACount = GameObject.FindGameObjectsWithTag(teamATag).Length;
            int teamBCount = GameObject.FindGameObjectsWithTag(teamBTag).Length;

            if (teamACount == 0 && teamBCount == 0)
            {
                GameDraw();
                gameEnded = true;
            }
            else if (teamACount == 0)
            {
                GameLose(teamATag);
                gameEnded = true;
            }
            else if (teamBCount == 0)
            {
                GameWin(teamATag);
                gameEnded = true;
            }

            yield return wait;
        }
    }

    private void GameWin(string winningTeam)
    {
        Debug.Log(winningTeam + " Wins!");
        ShowInterstitialThenAction(() =>
        {
            // 승리 후 UI 처리 또는 씬 전환
            Debug.Log("승리 후 처리 완료");
        });
    }

    private void GameLose(string losingTeam)
    {
        Debug.Log(losingTeam + " Loses!");
        ShowInterstitialThenAction(() =>
        {
            // 패배 후 UI 처리 또는 씬 전환
            Debug.Log("패배 후 처리 완료");
        });
    }

    private void GameDraw()
    {
        Debug.Log("Draw!");
        ShowInterstitialThenAction(() =>
        {
            // 무승부 후 UI 처리 또는 씬 전환
            Debug.Log("무승부 후 처리 완료");
        });
    }

// 전면 광고 호출 후 콜백 실행
    private void ShowInterstitialThenAction(Action onComplete)
    {
        if (InterstitialAdManager.Instance != null && InterstitialAdManager.Instance.IsAdReady())
        {
            InterstitialAdManager.Instance.ShowInterstitialAd(onComplete);
        }
        else
        {
            Debug.Log("전면 광고 준비 안됨 → 바로 처리");
            onComplete?.Invoke();
        }
    }
}