using System.Collections.Generic;
using UnityEngine;

public class GlobalVar : MonoBehaviour
{
    public static GlobalVar Instance;
    // 任务开关，和幽灵NPC对话后开启
    public static bool QuestStart = false;
    // 存储收集到的5件文物名称
    public static List<string> collectedItems = new List<string>();
    // 存储收集到的5件文物索引（0-4）
    public static List<int> collectedIndices = new List<int>();

    private void Awake()
    {
        // 单例，切换场景不销毁数据
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

    // 拾取道具调用，自动去重
    public static void AddItem(string itemName, int itemIndex)
    {
        if (!collectedItems.Contains(itemName))
        {
            collectedItems.Add(itemName);
            collectedIndices.Add(itemIndex);
        }
    }
}
