using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OpeningStory : MonoBehaviour
{
    public static OpeningStory Instance;
    public GameObject storyPanel;
    public Text storyText;
    public float typeSpeed = 0.05f;
    private string fullText = "怎么听见有人在哭泣……";
    private bool isTyping = false;
    private bool storyShown = false;

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
        if (storyPanel != null) storyPanel.SetActive(false);
        if (DataSave.Instance != null) storyShown = DataSave.Instance.LoadStoryShown();
        if (!storyShown) StartCoroutine(ShowOpeningStory());
    }

    private void Update()
    {
        if (storyPanel != null && storyPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (isTyping) CompleteTyping();
            else CloseStory();
        }
    }

    private IEnumerator ShowOpeningStory()
    {
        if (storyPanel == null || storyText == null) yield break;
        AudioManager.Instance?.PlayClickSFX();
        storyPanel.SetActive(true);
        storyText.text = "";
        isTyping = true;
        foreach (char c in fullText)
        {
            storyText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }

    private void CompleteTyping() { isTyping = false; storyText.text = fullText; }

    public void CloseStory()
    {
        AudioManager.Instance?.PlayClickSFX();
        if (storyPanel != null) storyPanel.SetActive(false);
        if (!storyShown)
        {
            storyShown = true;
            if (DataSave.Instance != null) DataSave.Instance.SaveStoryShown(true);
        }
    }

    public void ResetStory()
    {
        storyShown = false;
        if (DataSave.Instance != null) DataSave.Instance.SaveStoryShown(false);
    }
}
