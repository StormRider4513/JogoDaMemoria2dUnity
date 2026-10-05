using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonAnimation : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    [Header("Scale Settings")]
    public float pressedScale = 0.9f;
    public float tweenDuration = 0.1f;

    private Vector3 originalScale;
    private Tween currentTween;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayScale(originalScale * pressedScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlayScale(originalScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayScale(originalScale);
    }

    private void PlayScale(Vector3 target)
    {
        currentTween?.Kill();
        currentTween = transform
            .DOScale(target, tweenDuration)
            .SetEase(Ease.OutQuad);
    }
}
