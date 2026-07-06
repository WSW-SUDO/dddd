using UnityEngine;

public class BagHotkey : MonoBehaviour
{
    [Header("拖拽层级中的BagPanel")]
    public BagSystem bagSystem;

    void Update()
    {
        // 任意时刻按B打开背包，不受任务限制
        if (Input.GetKeyDown(KeyCode.B))
        {
            bagSystem.ToggleBag();
        }
    }
}
