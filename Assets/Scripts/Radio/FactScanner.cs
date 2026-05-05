using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FactScanner : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI textMesh;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Ищем, кликнули ли мы по тегу <link>
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMesh, eventData.position, eventData.pressEventCamera);

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = textMesh.textInfo.linkInfo[linkIndex];
            string factID = linkInfo.GetLinkID(); // Получаем ID улики (например, "manifest_cargo")
            string factText = linkInfo.GetLinkText(); // Получаем само слово (например, "FUEL")

            // Отправляем клик в Менеджер
            CommsManager.Instance.SelectFact(factID, factText, this, linkIndex);
        }
    }

    // Метод для перекраски слова (подсветка)
    public void HighlightLink(int linkIndex, Color32 color)
    {
        TMP_TextInfo textInfo = textMesh.textInfo;
        TMP_LinkInfo linkInfo = textInfo.linkInfo[linkIndex];

        for (int i = 0; i < linkInfo.linkTextLength; i++)
        {
            int charIndex = linkInfo.linkTextfirstCharacterIndex + i;
            int meshIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;
            if (textInfo.characterInfo[charIndex].isVisible)
            {
                vertexColors[vertexIndex + 0] = color;
                vertexColors[vertexIndex + 1] = color;
                vertexColors[vertexIndex + 2] = color;
                vertexColors[vertexIndex + 3] = color;
            }
        }
        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}