using UnityEngine;
using UnityEngine.UI;

public class BagSystem : MonoBehaviour
{
    public static BagSystem Instance;

    [Header("背包面板本体")]
    public GameObject bagPanel;

    [System.Serializable]
    public struct ItemSlot
    {
        public Image grayIcon;
        public Image colorIcon;
        public GameObject slotObj;
    }

    [Header("5个道具格子，顺序：1白瓷水盂 2青釉瓶 3褐彩药瓶 4玉壶春 5冰裂小碗")]
    public ItemSlot[] itemSlots = new ItemSlot[5];

    public bool bagIsOpen = false;

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

    void Start()
    {
        bagPanel.SetActive(false);
        bagIsOpen = false;
        HideAllSlots();
    }

    public void ToggleBag()
    {
        AudioManager.Instance?.PlayClickSFX();
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

    public void CloseBag()
    {
        AudioManager.Instance?.PlayClickSFX();
        bagIsOpen = false;
        bagPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void HideAllSlots()
    {
        foreach (var slot in itemSlots)
        {
            if(slot.slotObj != null)
                slot.slotObj.SetActive(false);
        }
    }

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

    public void RefreshBag()
    {
        if (!GlobalVar.QuestStart)
        {
            HideAllSlots();
            return;
        }

        ShowAllGrayIcon();
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
