using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    [Header("Transition UI")]
    public GameObject transitionScreen;
    public CanvasGroup transitionCanvasGroup;
    public TextMeshProUGUI dayText;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    [Header("Testing")]
    public bool skipTutorialAndStartDay1 = false;

    private EventSystem cachedEventSystem;
    public static bool isFirstGameLoad = true;

    // --- НОВОЕ: Память текущего дня ---
    public static int currentDay = 1;

    void Awake()
    {
        Instance = this;

#if !UNITY_EDITOR
        skipTutorialAndStartDay1 = false;
#endif

        cachedEventSystem = Object.FindFirstObjectByType<EventSystem>();

        bool willStartDay = skipTutorialAndStartDay1 || PlayerPrefs.HasKey("StartDayNumber");

        if (isFirstGameLoad || willStartDay)
        {
            LockPlayerInput(true);
            if (transitionScreen != null)
            {
                transitionScreen.SetActive(true);
                if (transitionCanvasGroup != null)
                {
                    transitionCanvasGroup.alpha = 1f;
                    transitionCanvasGroup.blocksRaycasts = true;
                }
                if (dayText != null) dayText.text = "";
            }
        }
        else
        {
            if (transitionScreen != null) transitionScreen.SetActive(false);
            if (transitionCanvasGroup != null) transitionCanvasGroup.blocksRaycasts = false;
        }
    }

    void Start()
    {
        if (skipTutorialAndStartDay1)
        {
            isFirstGameLoad = false;
            StartCoroutine(WaitAndStartDay(1, true));
            return;
        }

        if (PlayerPrefs.HasKey("StartDayNumber"))
        {
            isFirstGameLoad = false;
            currentDay = PlayerPrefs.GetInt("StartDayNumber"); 
            PlayerPrefs.DeleteKey("StartDayNumber");

            StartCoroutine(WaitAndStartDay(currentDay, true));
        }
        else if (isFirstGameLoad)
        {
            isFirstGameLoad = false;
            StartCoroutine(QuickFadeInTutorial());
        }
    }

    private void LockPlayerInput(bool isLocked)
    {
        if (cachedEventSystem != null) cachedEventSystem.enabled = !isLocked;
    }

    private IEnumerator QuickFadeInTutorial()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        yield return StartCoroutine(Fade(1f, 0f, 0.5f));

        if (transitionCanvasGroup != null) transitionCanvasGroup.blocksRaycasts = false;
        if (transitionScreen != null) transitionScreen.SetActive(false);
        LockPlayerInput(false);
    }

    private IEnumerator WaitAndStartDay(int dayNumber, bool isScreenAlreadyBlack = false)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        StartCoroutine(DayTransitionSequence(dayNumber, isScreenAlreadyBlack));
    }

    public void StartDay(int dayNumber)
    {
        currentDay = dayNumber;
        StartCoroutine(DayTransitionSequence(dayNumber, false));
    }

    public void EndCurrentShift()
    {
        StartCoroutine(EndShiftRoutine());
    }

    private IEnumerator EndShiftRoutine()
    {
        LockPlayerInput(true);
        transitionScreen.SetActive(true);
        if (transitionCanvasGroup != null) transitionCanvasGroup.blocksRaycasts = true;
        yield return StartCoroutine(Fade(0f, 1f, 1.5f));

        string endText = $"<size=150%>SHIFT {currentDay} COMPLETED</size>\r\n\r\n\r\n<color=#888888><size=70%>PROCESSING DATA...</size></color>";
        yield return StartCoroutine(TypeText(endText));

        yield return new WaitForSecondsRealtime(3f);

        currentDay++;

        StartCoroutine(DayTransitionSequence(currentDay, true));
    }

    private IEnumerator DayTransitionSequence(int dayNumber, bool isScreenAlreadyBlack)
    {
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.StopTutorial();
        }

        LockPlayerInput(true);

        transitionScreen.SetActive(true);
        if (transitionCanvasGroup != null) transitionCanvasGroup.blocksRaycasts = true;
        dayText.text = "";

        if (!isScreenAlreadyBlack) yield return StartCoroutine(Fade(0f, 1f, 1.0f));
        else transitionCanvasGroup.alpha = 1f;

        if (dayNumber == 1)
        {
            if (dayNumber == 1)
            {
                if (FlightDataManager.Instance != null)
                {
                    FlightDataManager.Instance.ResetForNewShift(150, 40, 220, 5);
                    FlightDataManager.Instance.maxPlanes = 3;
                }
            }
        }

        string targetText = $"<size=150%>SHIFT {dayNumber}</size>\r\n\r\n\r\n<color=#888888><size=70%>19.06.2039</size></color>";
        yield return StartCoroutine(TypeText(targetText));

        yield return new WaitForSecondsRealtime(2.5f);

        if (dayNumber == 1) SendDay1Directives();

        yield return StartCoroutine(Fade(1f, 0f, 1.5f));

        transitionScreen.SetActive(false);
        if (transitionCanvasGroup != null) transitionCanvasGroup.blocksRaycasts = false;

        LockPlayerInput(false);

        Debug.Log($"<color=green>[StoryManager]</color> Shift {dayNumber} started! Launching radar...");

        if (FlightDataManager.Instance != null)
        {
            FlightDataManager.Instance.StartDaySpawning(dayNumber);
        }
    }

    private IEnumerator TypeText(string textToType)
    {
        dayText.text = textToType;
        dayText.maxVisibleCharacters = 0;
        dayText.ForceMeshUpdate();

        int totalCharacters = dayText.textInfo.characterCount;
        for (int i = 0; i <= totalCharacters; i++)
        {
            dayText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    private void SendDay1Directives()
    {
        EmailData day1Email = new EmailData
        {
            sender = "Director Reed",
            date = "20.06.2036",
            subject = "DIRECTIVE #1 - URGENT",
            body = "Listen carefully, Dispatcher. Night storm damaged the runways. You only have THREE landing slots available today.\n\nThe base's generators are running at their limit. Your main task for today is to collect Fuel.\n\nAnd one more thing. Civilian refugees have been spotted in the sector. We have neither food nor beds for them.\n\nDIRECTIVE #1: Aircraft with civilians (Prefix TR) are STRICTLY FORBIDDEN from landing. Turn them back into the storm.\n\nEnd of communication."
        };
        AegisMailApp.pendingEmails.Add(day1Email);
    }

    private IEnumerator Fade(float startAlpha, float targetAlpha, float duration)
    {
        float time = 0;
        transitionCanvasGroup.alpha = startAlpha;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            transitionCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }
        transitionCanvasGroup.alpha = targetAlpha;
    }
}