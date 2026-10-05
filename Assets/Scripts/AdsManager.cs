using UnityEngine;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    private BannerView bannerView;
    private InterstitialAd interstitial;

    // SEUS IDs:
    private string appId = "ca-app-pub-9151134733971489~2923232722";
    private string bannerId = "ca-app-pub-9151134733971489/5437715423";
    private string interstitialId = "ca-app-pub-9151134733971489/5730904052";

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
        // Inicializa SDK
        MobileAds.Initialize((InitializationStatus status) =>
        {
            Debug.Log("ADMOB v10 Inicializado!");
            LoadBanner();
            LoadInterstitial();
        });
    }

    // ============================
    // BANNER
    // ============================
    private void LoadBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Top);

        var adRequest = new AdRequest();
        bannerView.LoadAd(adRequest);
    }

    public void ShowBanner()
    {
        bannerView?.Show();
    }

    public void HideBanner()
    {
        bannerView?.Hide();
    }

    // ============================
    // INTERSTITIAL
    // ============================
    private void LoadInterstitial()
    {
        if (interstitial != null)
        {
            interstitial.Destroy();
        }

        InterstitialAd.Load(interstitialId, new AdRequest(),
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Erro ao carregar interstitial: " + error);
                    return;
                }

                interstitial = ad;

                interstitial.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("Interstitial fechado → recarregando");
                    LoadInterstitial();
                };
            });
    }

    public void ShowInterstitial()
    {
        if (interstitial != null)
        {
            interstitial.Show();
        }
        else
        {
            Debug.Log("Interstitial não está pronto, recarregando...");
            LoadInterstitial();
        }
    }
}
