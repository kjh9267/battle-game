using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;

public class InterstitialAdManager : MonoBehaviour
{
    public static InterstitialAdManager Instance { get; private set; }
    private string adUnitId;
    private InterstitialAd interstitialAd;
    private bool isAdLoading = false;
    private bool isAdShowing = false;

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
        adUnitId = "ca-app-pub-3940256099942544/1033173712";

        // 비개인화 + 아동용 광고 설정
        RequestConfiguration requestConfiguration = new RequestConfiguration
        {
            TagForChildDirectedTreatment = TagForChildDirectedTreatment.True,
            MaxAdContentRating = MaxAdContentRating.G,
        };
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // MobileAds 초기화
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads 초기화 완료 (Interstitial)");
            LoadInterstitialAd();
        });
    }

    public void LoadInterstitialAd()
    {
        if (isAdLoading)
        {
            Debug.Log("이미 광고를 로드 중입니다.");
            return;
        }

        isAdLoading = true;
        AdRequest adRequest = BuildAdRequest();

        InterstitialAd.Load(adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            isAdLoading = false;

            if (error != null || ad == null)
            {
                Debug.LogError("전면 광고 로드 실패: " + error);
                StartCoroutine(RetryLoadAdCoroutine());
                return;
            }

            interstitialAd = ad;
            Debug.Log("전면 광고 로드 완료");

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("광고 닫힘 - 다시 로드 시도");
                isAdShowing = false;
                LoadInterstitialAd();
            };

            interstitialAd.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.LogError("광고 전체 화면 실패: " + adError);
                isAdShowing = false;
                LoadInterstitialAd();
            };
        });
    }

    private IEnumerator RetryLoadAdCoroutine()
    {
        float retryDelay = 5f;
        Debug.Log($"전면 광고 로드 재시도 대기 중... {retryDelay}초 후");
        yield return new WaitForSeconds(retryDelay);
        Debug.Log("전면 광고 로드 재시도 시작");
        LoadInterstitialAd();
    }

    public void ShowInterstitialAd(Action onClosed)
    {
        if (interstitialAd == null || !interstitialAd.CanShowAd())
        {
            Debug.LogWarning("전면 광고를 재생할 수 없습니다.");
            onClosed?.Invoke();
            return;
        }

        if (isAdShowing)
        {
            Debug.LogWarning("이미 광고가 재생 중입니다.");
            return;
        }

        isAdShowing = true;

        interstitialAd.Show();

        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("전면 광고 종료");
            isAdShowing = false;
            onClosed?.Invoke();
        };

        interstitialAd.OnAdFullScreenContentFailed += (AdError adError) =>
        {
            Debug.LogError("전면 광고 전체 화면 실패: " + adError);
            isAdShowing = false;
            onClosed?.Invoke();
        };
    }

    public bool IsAdReady()
    {
        return interstitialAd != null && interstitialAd.CanShowAd();
    }

    private AdRequest BuildAdRequest()
    {
        var adRequest = new AdRequest();
        adRequest.Extras["npa"] = "1";
        return adRequest;
    }
}
