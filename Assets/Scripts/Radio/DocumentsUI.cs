using UnityEngine;
using TMPro;

public class DocumentUI : MonoBehaviour
{
    public TextMeshProUGUI contentText;

    public void SetContent(string newText)
    {
        if (contentText != null)
        {
            contentText.text = newText;
        }
    }
}