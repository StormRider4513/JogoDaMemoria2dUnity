using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ConfettiUIController : MonoBehaviour
{
    [SerializeField] private ConfettiItem confettiPrefab;
    [SerializeField] private int amount = 30;
    [SerializeField] private Text youwin;

    public void Play()
    {
        RectTransform canvasRect = transform as RectTransform;
        RectTransform textRect = youwin.GetComponent<RectTransform>();
        RectTransform parentRect = transform as RectTransform;

        Vector2 startPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            RectTransformUtility.WorldToScreenPoint(null, textRect.position),
            null,
            out startPos
        );

        for (int i = 0; i < amount; i++)
        {
            ConfettiItem item = Instantiate(confettiPrefab, transform);
            item.Play(startPos);
        }
    }
}
