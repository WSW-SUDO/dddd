using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonMove : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 5f;
    public float jumpPower = 5f;

    [Header("旋转参数")]
    public float rotateSpeed = 2f;
    public float verticalSpeed = 1.5f;
    public float modelTurnSpeed = 8f;
    private float verticalAngle = 0;

    [Header("组件引用")]
    public Transform playerModel;
    public Transform cameraFollowPoint;
    public Camera mainCamera;

    [Header("地面检测")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("背包系统引用")]
    public BagSystem bagSystem;

    private Rigidbody rb;
    private bool jumpRequested = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogWarning("ThirdPersonMove: groundCheck is not assigned!");
            return false;
        }
        
        if (rb.velocity.y > 0.1f)
        {
            return false;
        }
        
        Collider[] hitColliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);
        foreach (Collider hit in hitColliders)
        {
            if (!hit.isTrigger && hit.transform != transform && !hit.transform.IsChildOf(transform))
            {
                return true;
            }
        }
        return false;
    }

    private void FixedUpdate()
    {
        // 背包打开时，禁止移动、跳跃操作
        if (bagSystem != null && bagSystem.bagIsOpen)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;
        moveDirection.Normalize();

        Vector3 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);

        if (moveDirection.magnitude > 0.01f && playerModel != null)
        {
            Quaternion targetModelRot = Quaternion.LookRotation(moveDirection);
            playerModel.rotation = Quaternion.Lerp(playerModel.rotation, targetModelRot, Time.fixedDeltaTime * modelTurnSpeed);
        }

        if (jumpRequested)
        {
            if (IsGrounded())
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
            }
            jumpRequested = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }

        // 背包开启，不执行鼠标视角旋转
        if (bagSystem != null && bagSystem.bagIsOpen)
            return;

        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
        transform.Rotate(Vector3.up, mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;
        verticalAngle -= mouseY;
        verticalAngle = Mathf.Clamp(verticalAngle, -45f, 30f);

        if (cameraFollowPoint != null && mainCamera != null)
        {
            mainCamera.transform.position = cameraFollowPoint.position;
            Vector3 lookPos = transform.position + new Vector3(0, 1.3f, 0);
            mainCamera.transform.LookAt(lookPos);
            mainCamera.transform.localEulerAngles = new Vector3(verticalAngle, 0, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
