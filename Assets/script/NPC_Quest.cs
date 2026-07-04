using UnityEngine;
using UnityEngine.UI;

public class NPC_Quest : MonoBehaviour
{
    public Text TipsText;
    public BagSystem bagSystem;
    private bool playerInRange;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !GlobalVar.QuestStart)
        {
            playerInRange = true;
            TipsText.gameObject.SetActive(true);
            TipsText.text = "按下E接取寻宝任务";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            TipsText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !GlobalVar.QuestStart)
        {
            GlobalVar.QuestStart = true;
            TipsText.text = "你答应了小幽灵的请求，前往庭院各处，找回五件散落的北宋古物。";
            
            if (bagSystem != null && bagSystem.bagIsOpen)
            {
                bagSystem.RefreshBag();
            }
        }
    }
}
