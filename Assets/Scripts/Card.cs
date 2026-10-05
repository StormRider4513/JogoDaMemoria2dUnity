using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public int id;
    public GameObject front;
    public GameObject back;

    private GameManager gameManager;
    private bool isFront = false;
    private bool isMatched = false;
    private bool isAnimating = false;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        ResetCard();
    }

    public void OnClick()
    {
        if (isAnimating || isMatched)
            return;

        if (!isFront && gameManager.CanClickCard())
        {
            FlipToFront();
            gameManager.CardRevealed(this);
        }
    }

    // ===================== FLIP PARA FRENTE =====================
    public void FlipToFront()
    {
        isAnimating = true;
        transform.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DORotate(new Vector3(0, 90, 0), 0.15f));
        seq.AppendCallback(() =>
        {
            back.SetActive(false);
            front.SetActive(true);
            isFront = true;
        });
        seq.Append(transform.DORotate(Vector3.zero, 0.15f));

        seq.OnComplete(() => isAnimating = false);
    }

    // ===================== FLIP PARA TRÁS =====================
    public void FlipToBack()
    {
        if (isAnimating || !isFront || isMatched)
            return;

        isAnimating = true;
        transform.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DORotate(new Vector3(0, 90, 0), 0.15f));
        seq.AppendCallback(() =>
        {
            front.SetActive(false);
            back.SetActive(true);
            isFront = false;
        });
        seq.Append(transform.DORotate(Vector3.zero, 0.15f));

        seq.OnComplete(() => isAnimating = false);
    }

    // ===================== ACERTO =====================
    public void SetMatched()
    {
        isMatched = true;

        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.interactable = false;

        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = false;
            cg.DOFade(0f, 0.35f);
        }
    }

    // ===================== ERRO =====================
    public void Shake()
    {
        transform.DOKill();
        transform.DOShakePosition(0.25f, new Vector3(15f, 0f, 0f), 10, 0);
    }

    // ===================== RESET =====================
    public void ResetCard()
    {
        isFront = false;
        isMatched = false;
        isAnimating = false;

        transform.rotation = Quaternion.identity;

        front.SetActive(false);
        back.SetActive(true);

        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.blocksRaycasts = true;
        }

        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.interactable = true;
    }
}
