using UnityEngine;
using UnityEngine.EventSystems; // Обязательно для работы с мышкой в UI

public class DraggablePaper : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Ищем главный Canvas, чтобы правильно рассчитывать масштаб мыши
        canvas = GetComponentInParent<Canvas>();
    }

    // Срабатывает в момент КЛИКА по бумажке
    public void OnPointerDown(PointerEventData eventData)
    {
        // Выносим эту бумажку поверх всех остальных!
        rectTransform.SetAsLastSibling();
    }

    // Срабатывает, когда мы ТЯНЕМ мышку с зажатой кнопкой
    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // Двигаем бумажку за мышкой с учетом масштаба интерфейса (Canvas)
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}