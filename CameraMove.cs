using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float mouseSensitivity = 120f;
    private float verticalAngle = 0f;

    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        verticalAngle -= mouseY;
        // 限制视角，防止镜头颠倒
        verticalAngle = Mathf.Clamp(verticalAngle, -80f, 80f);
        transform.localEulerAngles = new Vector3(verticalAngle, 0f, 0f);
    }
}
