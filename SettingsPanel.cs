using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    public static SettingsPanel Instance;
    public GameObject settingsPanel;
    public bool panelIsOpen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (settingsPanel == null)
        {
            settingsPanel = GameObject.Find("SettingsPanel");
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            panelIsOpen = false;
        }
    }

    public void TogglePanel()
    {
        AudioManager.Instance?.PlayClickSFX();
        
        if (settingsPanel == null)
        {
            settingsPanel = GameObject.Find("SettingsPanel");
            if (settingsPanel == null) return;
        }

        panelIsOpen = !panelIsOpen;
        settingsPanel.SetActive(panelIsOpen);

        if (panelIsOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OpenPanel() { if (!panelIsOpen) TogglePanel(); }
    public void ClosePanel() { if (panelIsOpen) TogglePanel(); }

    public void OnBGMButtonClick()
    {
        AudioManager.Instance?.ToggleBGM();
    }

    public void OnSFXButtonClick()
    {
        AudioManager.Instance?.ToggleSFX();
    }
}
