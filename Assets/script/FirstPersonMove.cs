using UnityEngine;

public class SwitchView : MonoBehaviour
{
    [Header("双相机赋值")]
    public Camera thirdCam;
    public Camera firstCam;
    [Header("拖拽BagPanel")]
    public BagSystem bagSystem;

    private bool isThirdPerson = true;

    void Start()
    {
        // 初始第三人称激活，锁定鼠标
        thirdCam.enabled = true;
        firstCam.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 背包打开时，禁止Tab切换视角，避免鼠标错乱
        if (bagSystem != null && bagSystem.bagIsOpen)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isThirdPerson = !isThirdPerson;
            thirdCam.enabled = isThirdPerson;
            firstCam.enabled = !isThirdPerson;
            // 切换视角后保持鼠标锁定
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
