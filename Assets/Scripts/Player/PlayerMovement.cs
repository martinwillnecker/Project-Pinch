using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float gravity = -20f;

    [Header("Rotation")]
    public bool rotateTowardsMovement = true;
    public float rotationSpeed = 20f;
    public Transform facingArrow;

    [Header("Camera")]
    public Transform cameraTransform;

    private CharacterController controller;
    private StatusEffectController statusEffectController;
    private Vector3 verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        statusEffectController = GetComponent<StatusEffectController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        Move();
        ApplyGravity();
    }

    private void Move()
    {
        if (statusEffectController != null && !statusEffectController.CanMove())
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);

        if (inputDirection.sqrMagnitude > 1f)
            inputDirection.Normalize();

        if (cameraTransform == null)
            return;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection =
            cameraForward * inputDirection.z +
            cameraRight * inputDirection.x;

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        float speedMultiplier = statusEffectController != null
            ? statusEffectController.GetSpeedMultiplier()
            : 1f;

        controller.Move(moveDirection * moveSpeed * speedMultiplier * Time.deltaTime);

        if (rotateTowardsMovement && moveDirection.sqrMagnitude > 0.001f)
            RotateTowards(moveDirection);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = -2f;

        verticalVelocity.y += gravity * Time.deltaTime;

        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        if (facingArrow != null)
        {
            facingArrow.rotation = Quaternion.Slerp(
                facingArrow.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}