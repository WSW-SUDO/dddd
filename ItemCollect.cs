using UnityEngine;
using UnityEngine.UI;

public class ItemCollect : MonoBehaviour
{
    [Header("收集成功提示文字")]
    public string finishTip;
    [Header("提示UI文本框")]
    public Text UIText;
    [Header("文物名称（背包显示）")]
    public string itemName;
    [Header("文物索引（0-4）：0=白瓷水盂 1=青釉瓶 2=褐彩药瓶 3=玉壶春 4=冰裂小碗")]
    public int itemIndex;

    private bool playerInRange;
    private bool alreadyPickup = false;

    private void Start()
    {
        // 开局隐藏提示文字
        if (UIText != null)
            UIText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyPickup) return;
        if (other.CompareTag("Player") && GlobalVar.QuestStart)
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !alreadyPickup)
        {
            alreadyPickup = true;

            // 弹出收集成功文字
            if (UIText != null)
            {
                UIText.text = finishTip;
                UIText.gameObject.SetActive(true);
            }

            // 存入背包列表
            GlobalVar.AddItem(itemName, itemIndex);

            // 道具立刻隐藏，2秒后文字消失
            Invoke(nameof(HideItem), 0.1f);
            Invoke(nameof(HideTip), 2f);
        }
    }

    void HideItem()
    {
        gameObject.SetActive(false);
    }

    void HideTip()
    {
        if (UIText != null)
            UIText.gameObject.SetActive(false);
    }
}
