using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ConfettiItem : MonoBehaviour
{
    private RectTransform rect;
    private Image img;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        img = GetComponent<Image>();
    }

    public void Play(Vector2 startPos)
    {
        rect.anchoredPosition = startPos;
        rect.localRotation = Quaternion.identity;
        rect.localScale = Vector3.one;
        img.color = GetRandomColor();

        float xOffset = Random.Range(-800f, 800f);
        float fallDistance = Random.Range(400f, 600f);
        float duration = Random.Range(1.4f, 1.8f);
        float rotation = Random.Range(-360f, 360f);

        Sequence seq = DOTween.Sequence();

        seq.Join(rect.DOAnchorPosY(startPos.y + 150f, 0.3f).SetEase(Ease.OutQuad));
        seq.Append(rect.DOAnchorPos(new Vector2(startPos.x + xOffset, startPos.y - fallDistance), duration)
            .SetEase(Ease.InQuad));

        seq.Join(rect.DORotate(new Vector3(0, 0, rotation), duration, RotateMode.FastBeyond360));
        seq.Join(img.DOFade(0f, duration));

        seq.OnComplete(() => Destroy(gameObject));
    }

    private Color GetRandomColor()
    {
        Color[] colors =
        {
            new Color(1f, 0.85f, 0.2f),
            new Color(0.3f, 0.7f, 1f),
            new Color(1f, 0.4f, 0.4f),
            new Color(0.5f, 1f, 0.6f),
            new Color(0.9f, 0.6f, 1f)
        };

        return colors[Random.Range(0, colors.Length)];
    }
}
