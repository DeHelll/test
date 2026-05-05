using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class EmailData
{
    public string sender;
    public string date;
    public string subject;
    [TextArea(5, 15)] 
    public string body;
}

public class AegisMailApp : MonoBehaviour
{
    public static List<EmailData> pendingEmails = new List<EmailData>();

    [Header("Настройки левой панели")]
    public Transform emailListContent;
    public GameObject emailButtonPrefab;

    [Header("Настройки правой панели (Чтение)")]
    public GameObject emptyStateVisual;
    public GameObject readingContentVisual;

    public TextMeshProUGUI readingSenderText;
    public TextMeshProUGUI readingSubjectText;
    public TextMeshProUGUI readingBodyText;

    [Header("База данных писем")]
    public List<EmailData> inbox = new List<EmailData>();

    void OnEnable()
    {
        if (pendingEmails.Count > 0)
        {
            inbox.InsertRange(0, pendingEmails); 
            pendingEmails.Clear(); 
        }

        RefreshInbox();
        ShowEmptyState();
    }

    public void RefreshInbox()
    {
        foreach (Transform child in emailListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (EmailData email in inbox)
        {
            GameObject btnObj = Instantiate(emailButtonPrefab, emailListContent);
            EmailButtonHelper helper = btnObj.GetComponent<EmailButtonHelper>();

            if (helper != null)
            {
                helper.senderText.text = email.sender;
                helper.subjectText.text = email.subject;
                helper.dateText.text = email.date;
                helper.button.onClick.AddListener(() => OpenEmail(email));
            }
        }
    }

    private void OpenEmail(EmailData email)
    {
        emptyStateVisual.SetActive(false);
        readingContentVisual.SetActive(true);

        readingSenderText.text = "FROM: " + email.sender;
        readingSubjectText.text = "SUBJECT: " + email.subject;
        readingBodyText.text = email.body;
    }

    public void ShowEmptyState()
    {
        emptyStateVisual.SetActive(true);
        readingContentVisual.SetActive(false);
    }

    public void ReceiveNewEmail(EmailData newEmail)
    {
        inbox.Insert(0, newEmail);

        if (gameObject.activeInHierarchy)
        {
            RefreshInbox();
        }
    }
}