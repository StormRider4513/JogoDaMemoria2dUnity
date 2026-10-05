using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.InputSystem;

public class SplashScreen : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panelSplash;
    [SerializeField] private GameObject panelMenu;

    [Header("Texts")]
    [SerializeField] private TMP_Text pressText;
    [SerializeField] private TMP_Text titleA;
    [SerializeField] private TMP_Text titleRest;

    [Header("Refs")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioManager audioManager;

    private bool canStart = false;

    void Start()
    {
        canStart = false;

        // Estado inicial
        titleA.alpha = 0f;
        titleRest.alpha = 0f;
        pressText.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        // 1️⃣ A azul aparece lentamente
        seq.Append(titleA.DOFade(1f, 1.8f).SetEase(Ease.OutSine));

        // 2️⃣ Pausa dramática
        seq.AppendInterval(0.6f);

        // 3️⃣ CHARADA aparece lentamente
        seq.Append(titleRest.DOFade(1f, 2.0f).SetEase(Ease.OutSine));

        // 4️⃣ Texto "clique para começar"
        seq.AppendInterval(0.3f);
        seq.AppendCallback(StartPressBlink);

        // Libera input
        seq.AppendCallback(() => Invoke(nameof(EnableStart), 0.1f));
    }

    void Update()
    {
        if (!canStart) return;

        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            audioManager.PlayClick();
            StartGame();
        }
    }

    private void EnableStart()
    {
        canStart = true;
    }

    private void StartGame()
    {
        canStart = false;

        pressText.DOKill();
        titleA.DOKill();
        titleRest.DOKill();

        CanvasGroup cg = panelSplash.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panelSplash.AddComponent<CanvasGroup>();

        cg.DOFade(0f, 0.4f).OnComplete(() =>
        {
            gameManager.EnterMainMenu();
        });
    }

    private void StartPressBlink()
    {
        pressText
            .DOFade(1f, 0.7f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }
}
