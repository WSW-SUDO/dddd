using UnityEngine;
using UnityEngine.UI;

public class BagSystem : MonoBehaviour
{
    [Header("背包面板本体")]
    public GameObject bagPanel;

    [System.Serializable]
    public struct ItemSlot
    {
        [Tooltip("未收集：黑底灰色图标")]
        public Image grayIcon;
        [Tooltip("收集后：黑底彩色图标")]
        public Image colorIcon;
        // 插槽父物体（整个道具格子）
        public GameObject slotObj;
    }

    [Header("5个道具格子，顺序：1白瓷水盂 2青釉瓶 3褐彩药瓶 4玉壶春 5冰裂小碗")]
    public ItemSlot[] itemSlots = new ItemSlot[5];

    public bool bagIsOpen = false;

    void Start()
    {
        bagPanel.SetActive(false);
        bagIsOpen = false;
        // 开局：隐藏全部5个道具格子，背包空白
        HideAllSlots();
    }

    // B键开关背包
    public void ToggleBag()
    {
        bagIsOpen = !bagIsOpen;
        bagPanel.SetActive(bagIsOpen);
        if (bagIsOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            RefreshBag();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // 关闭背包按钮绑定方法
    public void CloseBag()
    {
        bagIsOpen = false;
        bagPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 全部格子直接隐藏（开局空白用）
    void HideAllSlots()
    {
        foreach (var slot in itemSlots)
        {
            if(slot.slotObj != null)
                slot.slotObj.SetActive(false);
        }
    }

    // 全部格子显示灰色图标、隐藏彩色
    void ShowAllGrayIcon()
    {
        foreach (var slot in itemSlots)
        {
            if(slot.slotObj != null)
                slot.slotObj.SetActive(true);
            
            if(slot.grayIcon != null)
                slot.grayIcon.gameObject.SetActive(true);
            
            if(slot.colorIcon != null)
                slot.colorIcon.gameObject.SetActive(false);
        }
    }

    // 刷新背包核心逻辑
    public void RefreshBag()
    {
        // 没接任务：保持所有格子隐藏，背包空白
        if (!GlobalVar.QuestStart)
        {
            HideAllSlots();
            return;
        }

        // 已领取任务：先全部显示灰色图标
        ShowAllGrayIcon();
        // 根据收集到的索引激活对应位置的彩色图标
        foreach (int index in GlobalVar.collectedIndices)
        {
            if (index >= 0 && index < itemSlots.Length)
            {
                if (itemSlots[index].grayIcon != null)
                    itemSlots[index].grayIcon.gameObject.SetActive(false);
                if (itemSlots[index].colorIcon != null)
                    itemSlots[index].colorIcon.gameObject.SetActive(true);
            }
        }
    }
}
