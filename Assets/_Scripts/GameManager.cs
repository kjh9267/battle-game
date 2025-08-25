using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Team Tags")]
    public string teamATag = "TeamA";
    public string teamBTag = "TeamB";

    [Header("UI")]
    public Button startButton; // Start 버튼

    private bool gamePlaying = false; // 게임 진행 중 여부
    private int gameCount = 0;        // 현재 게임 카운트

    public static GameManager Instance { get; private set; }

    public bool IsGamePlaying => gamePlaying;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        MapManager.Instance.LoadMap("mapName");

        // 버튼 클릭 이벤트 등록
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartGameButtonClicked);
        }
        else
        {
            Debug.LogWarning("StartButton이 할당되지 않았습니다.");
        }
    }

    public void OnStartGameButtonClicked()
    {
        Debug.Log("Start 버튼 눌림 → 게임 시작");

        // 버튼 비활성화
        if (startButton != null)
            startButton.gameObject.SetActive(false);

        StartGame();
    }

    public void StartGame()
    {
        gamePlaying = true;
        StartCoroutine(CheckTeamsRoutine());
        Debug.Log("게임 진행 상태: true");
    }

    public void EndGame()
    {
        gamePlaying = false;
        gameCount++;
        Debug.Log($"게임 종료, 현재 게임카운트: {gameCount}");

        // 2판마다 광고
        if (gameCount % 2 == 0)
        {
            ShowInterstitialThenAction(() =>
            {
                gameCount = 0; // 광고 후 게임카운트 초기화
                MapManager.Instance.LoadMap("mapName");
                startButton.gameObject.SetActive(true);
            });
        }
        else
        {
            // 광고 없이 바로 다음 게임
            MapManager.Instance.LoadMap("mapName");
            startButton.gameObject.SetActive(true);
        }

        StopCoroutine(CheckTeamsRoutine());
    }

    private IEnumerator CheckTeamsRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);

        while (true)
        {
            if (gamePlaying)
            {
                int teamACount = GameObject.FindGameObjectsWithTag(teamATag).Length;
                int teamBCount = GameObject.FindGameObjectsWithTag(teamBTag).Length;

                if (teamACount == 0 || teamBCount == 0)
                {
                    EndGame();
                }
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
