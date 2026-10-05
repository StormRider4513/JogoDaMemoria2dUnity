using UnityEngine;
using DG.Tweening;
using TMPro;

public class WinTextEffect : MonoBehaviour
{
    [SerializeField] private float popDuration = 0.4f;
    [SerializeField] private float pulseScale = 1.08f;
    [SerializeField] private float pulseDuration = 0.8f;

    private Tween pulseTween;

    void OnEnable()
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;

        // Pop inicial
        transform
            .DOScale(1f, popDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(StartPulse);
    }

    void OnDisable()
    {
        pulseTween?.Kill();
    }

    private void StartPulse()
    {
        pulseTween = transform
            .DOScale(pulseScale, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
