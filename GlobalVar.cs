using System.Collections.Generic;
using UnityEngine;

public class GlobalVar : MonoBehaviour
{
    public static GlobalVar Instance;
    public static bool QuestStart = false;
    public static bool QuestCompleted = false;
    public static List<string> collectedItems = new List<string>();
    public static List<int> collectedIndices = new List<int>();
    public const int TotalItems = 5;

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
        LoadGameData();
        PlayStartBGM();
    }

    private void LoadGameData()
    {
        if (DataSave.Instance != null)
        {
            QuestStart = DataSave.Instance.LoadQuestStart();
            QuestCompleted = DataSave.Instance.LoadQuestCompleted();
            
            string savedItems = DataSave.Instance.LoadCollectedItems();
            if (!string.IsNullOrEmpty(savedItems))
            {
                string[] items = savedItems.Split(",");
                foreach (string item in items)
                {
                    if (!string.IsNullOrEmpty(item) && !collectedItems.Contains(item))
                    {
                        collectedItems.Add(item);
                    }
                }
            }
        }
    }

    private void PlayStartBGM()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM();
        }
    }

    public static void AddItem(string itemName, int itemIndex)
    {
        if (!collectedItems.Contains(itemName))
        {
            collectedItems.Add(itemName);
            collectedIndices.Add(itemIndex);
            
            if (DataSave.Instance != null)
            {
                DataSave.Instance.SaveCollectedItems(string.Join(",", collectedItems));
            }
        }
    }

    public static bool IsQuestCompleted()
    {
        return collectedItems.Count >= TotalItems;
    }

    public static void CompleteQuest()
    {
        QuestCompleted = true;
        if (DataSave.Instance != null)
        {
            DataSave.Instance.SaveQuestCompleted(true);
        }
    }

    public static void SaveQuestStart(bool value)
    {
        QuestStart = value;
        if (DataSave.Instance != null)
        {
            DataSave.Instance.SaveQuestStart(value);
        }
    }
}
