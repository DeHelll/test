using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI; // Обязательно для CanvasGroup

public class ZoomReturnManager : MonoBehaviour
{
    // Статическая переменная, которая помнит, откуда мы возвращаемся
    public static string pendingReturnTargetName = "";

    [Header("Настройки отдаления")]
    public RectTransform rootContainer; // Твой ScreenContent
    public float zoomDuration = 0.5f;
    public float startingZoomMultiplier = 2.5f;

    void Start()
    {
        // Если переменная не пустая, значит мы загрузили сцену, возвращаясь с радара/терминала
        if (!string.IsNullOrEmpty(pendingReturnTargetName))
        {
            StartCoroutine(PrepareAndZoomOut());
        }
    }

    private IEnumerator PrepareAndZoomOut()
    {
        // 1. Делаем контейнер временно прозрачным на 1 кадр, чтобы скрыть "прыжок"
        CanvasGroup canvasGroup = rootContainer.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = rootContainer.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        // 2. ИСПРАВЛЕНИЕ: Ждем ровно один кадр и принудительно обновляем UI. 
        // Без этого Unity думает, что все объекты находятся в точке (0,0).
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();

        // 3. Ищем наш объект по сохраненному имени
        GameObject targetObj = GameObject.Find(pendingReturnTargetName);

        if (targetObj != null)
        {
            Debug.Log($"<color=green>[ZoomReturn]</color> Найден объект для возврата: {targetObj.name}");
            // Передаем обычный Transform (как в скрипте входа), чтобы работало с любыми объектами
            yield return StartCoroutine(ZoomOutAnimation(targetObj.transform, canvasGroup));
        }
        else
        {
            Debug.LogError($"<color=red>[ZoomReturn]</color> Ошибка! Не найден объект с именем: {pendingReturnTargetName}. Возврат из центра.");
            canvasGroup.alpha = 1f; // Возвращаем видимость, если сломалось
        }

        // Очищаем переменную
        pendingReturnTargetName = "";
    }

    private IEnumerator ZoomOutAnimation(Transform zoomTarget, CanvasGroup canvasGroup)
    {
        // Запоминаем нормальное состояние стола (целиком)
        Vector3 normalScale = rootContainer.localScale;
        Vector2 normalPos = rootContainer.anchoredPosition;

        // Вычисляем координаты сильно приближенного состояния
        Vector3 zoomedScale = normalScale * startingZoomMultiplier;

        Vector3 localTargetPos3D = rootContainer.InverseTransformPoint(zoomTarget.position);
        Vector2 localTargetPos = new Vector2(localTargetPos3D.x, localTargetPos3D.y);
        Vector2 zoomedPos = normalPos - (localTargetPos * (zoomedScale.x - normalScale.x));

        // МГНОВЕННО применяем приближенное состояние
        rootContainer.localScale = zoomedScale;
        rootContainer.anchoredPosition = zoomedPos;

        // Задаем мгновенное значение света
        Light2D[] lights = rootContainer.GetComponentsInChildren<Light2D>();
        float[] normalOuter = new float[lights.Length];
        float[] normalInner = new float[lights.Length];
        for (int i = 0; i < lights.Length; i++)
        {
            normalOuter[i] = lights[i].pointLightOuterRadius;
            normalInner[i] = lights[i].pointLightInnerRadius;

            lights[i].pointLightOuterRadius = normalOuter[i] * startingZoomMultiplier;
            lights[i].pointLightInnerRadius = normalInner[i] * startingZoomMultiplier;
        }

        // Включаем видимость перед началом плавной анимации
        canvasGroup.alpha = 1f;

        // ПЛАВНО отдаляемся обратно к нормальному состоянию
        float elapsedTime = 0f;
        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float smooth = elapsedTime / zoomDuration;
            smooth = smooth * smooth * (3f - 2f * smooth); // Плавность (ease-in-out)

            rootContainer.localScale = Vector3.Lerp(zoomedScale, normalScale, smooth);
            rootContainer.anchoredPosition = Vector2.Lerp(zoomedPos, normalPos, smooth);

            float currentScaleRatio = rootContainer.localScale.x / normalScale.x;
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].pointLightOuterRadius = normalOuter[i] * currentScaleRatio;
                lights[i].pointLightInnerRadius = normalInner[i] * currentScaleRatio;
            }

            yield return null;
        }

        // Гарантируем, что в конце стол стоит идеально ровно
        rootContainer.localScale = normalScale;
        rootContainer.anchoredPosition = normalPos;
    }
}