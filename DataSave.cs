using UnityEngine;

public class DataSave : MonoBehaviour
{
    public static DataSave Instance;

    [Header("默认音量")]
    public float defaultBGMVolume = 0.5f;
    public float defaultSFXVolume = 0.8f;

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

    public float LoadBGMVolume()
    {
        if (PlayerPrefs.HasKey("BGMVolume"))
            return PlayerPrefs.GetFloat("BGMVolume");
        return defaultBGMVolume;
    }

    public void SaveBGMVolume(float value)
    {
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();
    }

    public float LoadSFXVolume()
    {
        if (PlayerPrefs.HasKey("SFXVolume"))
            return PlayerPrefs.GetFloat("SFXVolume");
        return defaultSFXVolume;
    }

    public void SaveSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public bool LoadStoryShown()
    {
        if (PlayerPrefs.HasKey("StoryShown"))
            return PlayerPrefs.GetInt("StoryShown") == 1;
        return false;
    }

    public void SaveStoryShown(bool value)
    {
        PlayerPrefs.SetInt("StoryShown", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool LoadQuestStart()
    {
        if (PlayerPrefs.HasKey("QuestStart"))
            return PlayerPrefs.GetInt("QuestStart") == 1;
        return false;
    }

    public void SaveQuestStart(bool value)
    {
        PlayerPrefs.SetInt("QuestStart", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool LoadQuestCompleted()
    {
        if (PlayerPrefs.HasKey("QuestCompleted"))
            return PlayerPrefs.GetInt("QuestCompleted") == 1;
        return false;
    }

    public void SaveQuestCompleted(bool value)
    {
        PlayerPrefs.SetInt("QuestCompleted", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public string LoadCollectedItems()
    {
        if (PlayerPrefs.HasKey("CollectedItems"))
            return PlayerPrefs.GetString("CollectedItems");
        return "";
    }

    public void SaveCollectedItems(string items)
    {
        PlayerPrefs.SetString("CollectedItems", items);
        PlayerPrefs.Save();
    }
}
