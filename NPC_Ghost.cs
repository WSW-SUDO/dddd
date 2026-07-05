using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NPC_Ghost : MonoBehaviour
{
    public static NPC_Ghost Instance;
    public static bool isDialogActive = false;

    [Header("对话UI")]
    public GameObject dialogPanel;
    public Text dialogText;

    [Header("打字机效果")]
    public float typeSpeed = 0.03f;

    [Header("任务完成奖励")]
    public GameObject rewardPanel;
    public Text rewardText;

    [Header("对话冷却")]
    public float dialogCooldown = 1f;
    private bool canTriggerDialog = true;

    private bool playerInRange = false;
    private bool isTyping = false;
    private int currentDialogIndex = 0;

    private List<string> dialogPhase1 = new List<string>()
    {
        "呜呜……",
        "有人吗……",
        "我的宝贝们不见了……",
        "你愿意帮我找回它们吗？",
        "一共五件北宋古物，散落在这庭院里……"
    };

    private List<string> dialogPhase2 = new List<string>()
    {
        "还没找齐吗？",
        "还差 "
    };

    private List<string> dialogPhase3 = new List<string>()
    {
        "太好了！谢谢你！",
        "你真是个好人……",
        "这些古物终于回到我身边了……",
        "作为感谢，请收下这个吧！"
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        isDialogActive = false;
        playerInRange = false;
        canTriggerDialog = true;
    }

    private void Update()
    {
        if (isDialogActive && Input.GetMouseButtonDown(0))
        {
            NextDialog();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
            if (canTriggerDialog && !isDialogActive)
            {
                StartDialog();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
            if (isDialogActive)
            {
                CloseDialog();
            }
        }
    }

    public void StartDialog()
    {
        if (isDialogActive) return;
        
        AudioManager.Instance?.PlayClickSFX();
        isDialogActive = true;
        currentDialogIndex = 0;
        
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true);
        }
        
        ShowCurrentDialog();
    }

    private void ShowCurrentDialog()
    {
        string text = GetCurrentDialogText();
        StartCoroutine(TypeText(text));
    }

    private string GetCurrentDialogText()
    {
        if (GlobalVar.QuestCompleted)
        {
            if (currentDialogIndex < dialogPhase3.Count) return dialogPhase3[currentDialogIndex];
            return "";
        }
        if (!GlobalVar.QuestStart)
        {
            if (currentDialogIndex < dialogPhase1.Count) return dialogPhase1[currentDialogIndex];
            return "";
        }
        if (currentDialogIndex == 0) return dialogPhase2[0];
        return dialogPhase2[1] + (GlobalVar.TotalItems - GlobalVar.collectedItems.Count) + " 件";
    }

    private IEnumerator TypeText(string text)
    {
        if (dialogText == null) yield break;
        isTyping = true;
        dialogText.text = "";
        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }

    public void NextDialog()
    {
        if (!isDialogActive) return;
        if (isTyping)
        {
            dialogText.text = GetCurrentDialogText();
            isTyping = false;
            return;
        }
        AudioManager.Instance?.PlayClickSFX();

        if (GlobalVar.QuestCompleted)
        {
            currentDialogIndex++;
            if (currentDialogIndex >= dialogPhase3.Count) CloseDialog();
            else ShowCurrentDialog();
            return;
        }

        if (!GlobalVar.QuestStart)
        {
            currentDialogIndex++;
            if (currentDialogIndex >= dialogPhase1.Count) CompletePhase1();
            else ShowCurrentDialog();
            return;
        }

        currentDialogIndex++;
        if (currentDialogIndex >= dialogPhase2.Count) CloseDialog();
        else ShowCurrentDialog();
    }

    private void CompletePhase1()
    {
        GlobalVar.SaveQuestStart(true);
        dialogText.text = "任务已接取！去寻找五件散落的北宋古物吧！";
        if (BagSystem.Instance != null) BagSystem.Instance.RefreshBag();
        StartCoroutine(CloseAfterDelay(2f));
    }

    private IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseDialog();
    }

    public void CloseDialog()
    {
        if (!isDialogActive) return;
        AudioManager.Instance?.PlayClickSFX();
        isDialogActive = false;
        isTyping = false;
        
        if (dialogPanel != null) dialogPanel.SetActive(false);

        if (!GlobalVar.QuestCompleted && GlobalVar.IsQuestCompleted())
        {
            ShowReward();
            GlobalVar.CompleteQuest();
        }

        canTriggerDialog = false;
        StartCoroutine(ResetTriggerCooldown());
    }

    private IEnumerator ResetTriggerCooldown()
    {
        yield return new WaitForSeconds(dialogCooldown);
        canTriggerDialog = true;
    }

    private void ShowReward()
    {
        if (rewardPanel != null && rewardText != null)
        {
            rewardText.text = "恭喜！你已集齐所有文物！\\n获得神秘道具：【古画残卷】";
            rewardPanel.SetActive(true);
            AudioManager.Instance?.PlayClickSFX();
        }
    }

    public void CloseReward()
    {
        AudioManager.Instance?.PlayClickSFX();
        if (rewardPanel != null) rewardPanel.SetActive(false);
    }
}
