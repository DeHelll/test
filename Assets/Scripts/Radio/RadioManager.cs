using UnityEngine;
using UnityEngine.SceneManagement;

public class RadioManager : MonoBehaviour
{
    public static RadioManager Instance;
    public GameObject blinkingLight;
    public float blinkSpeed = 2f;

    public static string activeCallsign = "";
    public static bool isNewCall = false;     

    private float blinkTimer = 0f;

    void Awake()
    {
        Instance = this;
        if (blinkingLight != null) blinkingLight.SetActive(activeCallsign != "");
    }

    void Update()
    {
        if (activeCallsign != "" && blinkingLight != null)
        {
            if (isNewCall)
            {
                blinkTimer += Time.deltaTime * blinkSpeed;
                blinkingLight.SetActive(Mathf.Sin(blinkTimer) > 0);
            }
            else
            {
                blinkingLight.SetActive(true); 
            }
        }
    }

    public void OnRadioClicked()
    {
        if (activeCallsign != "")
        {
            isNewCall = false; 
            SceneManager.LoadScene("CommsScene");
        }
    }
}