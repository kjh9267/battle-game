using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Team Tags")]
    public string teamATag = "TeamA";
    public string teamBTag = "TeamB";

    private bool gameEnded = false;
    private bool gamePlaying = false;

    public static GameManager Instance { get; private set; }

    public bool IsGamePlaying => gamePlaying;

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
        StartGame(); // 게임 시작 시
    }

    public void StartGame()
    {
        gamePlaying = true;
        gameEnded = false;
    }

    public void EndGame()
    {
        gamePlaying = false;
        gameEnded = true;
    }

    private IEnumerator CheckTeamsRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);

        while (!gameEnded)
        {
            int teamACount = GameObject.FindGameObjectsWithTag(teamATag).Length;
            int teamBCount = GameObject.FindGameObjectsWithTag(teamBTag).Length;

            if (teamACount == 0 || teamBCount == 0)
            {
                EndGame();
                MapManager.Instance.UnloadMap();
                ShowInterstitialThenAction(() =>
                {
                    MapManager.Instance.LoadMap("mapName");
                });
            }

            yield return wait;
        }
    }

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