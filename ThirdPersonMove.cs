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
    public Animator playerAnimator;

    [Header("地面检测")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("背包系统引用")]
    public BagSystem bagSystem;

    [Header("UI边缘检测")]
    public float edgeThreshold = 50f;
    private bool wasEdgeHovering = false;

    [Header("相机参数")]
    public float defaultCameraDistance = 2.5f;
    public float minCameraDistance = 0.5f;
    public float defaultFOV = 75f;
    public float narrowFOV = 60f;
    public LayerMask obstacleLayer;

    private Rigidbody rb;
    private bool isJumping = false;
    private bool wasGrounded = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (mainCamera != null)
        {
            mainCamera.fieldOfView = defaultFOV;
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return false;
        if (rb.velocity.y > 0.1f) return false;
        Collider[] hitColliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);
        foreach (Collider hit in hitColliders)
        {
            if (!hit.isTrigger && hit.transform != transform && !hit.transform.IsChildOf(transform))
                return true;
        }
        return false;
    }

    private bool IsMovementBlocked()
    {
        if (bagSystem != null && bagSystem.bagIsOpen) return true;
        if (SettingsPanel.Instance != null && SettingsPanel.Instance.panelIsOpen) return true;
        if (OpeningStory.Instance != null && OpeningStory.Instance.storyPanel != null && OpeningStory.Instance.storyPanel.activeSelf) return true;
        if (NPC_Ghost.isDialogActive) return true;
        return false;
    }

    private bool IsCameraBlocked()
    {
        if (bagSystem != null && bagSystem.bagIsOpen) return true;
        if (SettingsPanel.Instance != null && SettingsPanel.Instance.panelIsOpen) return true;
        return false;
    }

    private bool IsMouseNearEdge()
    {
        return Input.mousePosition.x < edgeThreshold ||
               Input.mousePosition.x > Screen.width - edgeThreshold ||
               Input.mousePosition.y < edgeThreshold ||
               Input.mousePosition.y > Screen.height - edgeThreshold;
    }

    private void Update()
    {
        bool isEdgeHovering = IsMouseNearEdge();

        if (isEdgeHovering && !wasEdgeHovering)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (!isEdgeHovering && wasEdgeHovering && !IsMovementBlocked() && !IsCameraBlocked())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        wasEdgeHovering = isEdgeHovering;

        if (!IsMovementBlocked())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!isJumping && IsGrounded())
                {
                    rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
                    isJumping = true;
                }
            }
        }

        if (!IsCameraBlocked() && !isEdgeHovering)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
            transform.Rotate(Vector3.up, mouseX);

            float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;
            verticalAngle -= mouseY;
            verticalAngle = Mathf.Clamp(verticalAngle, -45f, 30f);
        }

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        if (cameraFollowPoint == null || mainCamera == null) return;

        Vector3 targetPosition = cameraFollowPoint.position;
        Vector3 lookDirection = Quaternion.Euler(verticalAngle, transform.eulerAngles.y, 0) * Vector3.back;
        Vector3 cameraOffset = lookDirection * defaultCameraDistance;
        Vector3 desiredPosition = targetPosition + cameraOffset;

        float adjustedDistance = CalculateAdjustedDistance(targetPosition, desiredPosition);
        cameraOffset = lookDirection * adjustedDistance;
        desiredPosition = targetPosition + cameraOffset;

        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, desiredPosition, Time.deltaTime * 5f);
        mainCamera.transform.LookAt(targetPosition);

        UpdateCameraFOV(adjustedDistance);
    }

    private float CalculateAdjustedDistance(Vector3 from, Vector3 to)
    {
        Vector3 direction = (to - from).normalized;
        float originalDistance = Vector3.Distance(from, to);

        if (obstacleLayer != 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(from, direction, out hit, originalDistance, obstacleLayer))
            {
                float hitDistance = Vector3.Distance(from, hit.point);
                return Mathf.Max(minCameraDistance, hitDistance - 0.3f);
            }
        }

        return originalDistance;
    }

    private void UpdateCameraFOV(float currentDistance)
    {
        if (mainCamera == null) return;

        float ratio = currentDistance / defaultCameraDistance;
        float targetFOV = Mathf.Lerp(narrowFOV, defaultFOV, ratio);
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * 3f);
    }

    private void FixedUpdate()
    {
        if (IsMovementBlocked())
        {
            if (playerAnimator != null) playerAnimator.SetFloat("MoveSpeed", 0);
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection;
        if (mainCamera != null)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();
            Vector3 cameraRight = mainCamera.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            moveDirection = cameraForward * vertical + cameraRight * horizontal;
        }
        else
        {
            moveDirection = transform.forward * vertical + transform.right * horizontal;
        }
        moveDirection.Normalize();

        Vector3 targetVel = moveDirection * moveSpeed;
        targetVel.y = rb.velocity.y;
        rb.velocity = targetVel;

        if (moveDirection.magnitude > 0.01f && playerModel != null)
        {
            Quaternion targetModelRot = Quaternion.LookRotation(moveDirection);
            playerModel.rotation = Quaternion.Lerp(playerModel.rotation, targetModelRot, Time.fixedDeltaTime * modelTurnSpeed);
        }

        if (playerAnimator != null) playerAnimator.SetFloat("MoveSpeed", moveDirection.magnitude);

        bool isGrounded = IsGrounded();
        if (isGrounded && !wasGrounded) isJumping = false;
        wasGrounded = isGrounded;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (cameraFollowPoint != null && mainCamera != null)
        {
            Gizmos.color = Color.blue;
            Vector3 desiredPosition = cameraFollowPoint.position +
                Quaternion.Euler(verticalAngle, transform.eulerAngles.y, 0) * Vector3.back * defaultCameraDistance;
            Gizmos.DrawLine(cameraFollowPoint.position, desiredPosition);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(desiredPosition, 0.1f);
        }
    }
}