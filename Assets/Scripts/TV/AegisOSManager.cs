using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class AegisOSManager : MonoBehaviour
{
    [Header("App Windows")]
    public GameObject airTrafficWindow;
    public GameObject inboxWindow;

    [Header("Main Scene Name")]
    public string mainDeskSceneName = "SampleScene"; 

    void Start()
    {
        if (airTrafficWindow) airTrafficWindow.SetActive(false);
        if (inboxWindow) inboxWindow.SetActive(false);
    }

    public void OpenAirTrafficApp()
    {
        if (airTrafficWindow)
        {
            airTrafficWindow.SetActive(true);
            airTrafficWindow.transform.SetAsLastSibling(); 
        }
    }

    public void OpenInboxApp()
    {
        if (inboxWindow)
        {
            inboxWindow.SetActive(true);
            inboxWindow.transform.SetAsLastSibling(); 
        }
    }

    public void CloseWindow(GameObject windowToClose)
    {
        if (windowToClose)
        {
            windowToClose.SetActive(false);
        }
    }
}