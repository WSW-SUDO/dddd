using UnityEngine;
using UnityEngine.UI;

public class AreaTip : MonoBehaviour
{
    [Header("地点名称")]
    public string locationName;
    [Header("区域提示语")]
    public string enterTip;

    public Text UIText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GlobalVar.QuestStart)
        {
            UIText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIText.gameObject.SetActive(false);
        }
    }
}
