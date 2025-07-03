using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Transform cameraTransform;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;
    private bool movementEnabled = true;

    private MinimapRenderer minimap;
    private Vector2Int lastCell = new Vector2Int(-1, -1);

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        minimap = FindObjectOfType<MinimapRenderer>();
    }

    void Update()
    {
        if (!movementEnabled)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        MovePlayer();
        RotateCamera();
        CheckJump();
        UpdateMinimapPosition();
    }

    private void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

        Vector3 moveDir = (right * moveX + forward * moveZ).normalized * moveSpeed;

        rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * 2f;
        transform.Rotate(Vector3.up * mouseX);
    }

    private void CheckJump()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    private void UpdateMinimapPosition()
    {
        if (minimap == null) return;

        int x = Mathf.RoundToInt(transform.position.x / 2f);
        int z = Mathf.RoundToInt(transform.position.z / 2f);
        Vector2Int currentCell = new Vector2Int(x, z);

        if (currentCell != lastCell)
        {
            minimap.MarkVisited(x, z);
            minimap.SetPlayerPosition(x, z);
            lastCell = currentCell;
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
    }
}
