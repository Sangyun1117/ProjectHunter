using UnityEngine;
using UnityEngine.EventSystems;

public class HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform rectTransform;
    Vector3 originalScale;
    public float hoverScale = 1.3f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스 호버 시 크기 증가
        rectTransform.localScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 떠날 때 원래 크기로 복원
        rectTransform.localScale = originalScale;
    }
}