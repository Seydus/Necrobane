using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private UIMenuManager UIMenuManager;

    private void Awake()
    {
        UIMenuManager = GetComponent<UIMenuManager>();
    }

    public void PlayBtn()
    {
        SceneManager.LoadScene(1);
    }

    public void BackToMain(GameObject gameObject)
    {
        UIMenuManager.mainObj.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SettingsBtn()
    {
        UIMenuManager.settingsObj.SetActive(true);
        UIMenuManager.mainObj.SetActive(false);
    }

    public void QuitBtn()
    {
        
    }
}
