using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 12f;

    [Header("Camera")]
    public Transform cameraTransform;

    private CharacterController controller;
    private StatusEffectController statusEffectController;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        statusEffectController = GetComponent<StatusEffectController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (statusEffectController != null && !statusEffectController.CanMove())
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection.magnitude < 0.1f)
            return;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection =
            cameraForward * vertical +
            cameraRight * horizontal;

        moveDirection.Normalize();

        float speedMultiplier = statusEffectController != null
            ? statusEffectController.GetSpeedMultiplier()
            : 1f;

        controller.Move(moveDirection * moveSpeed * speedMultiplier * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}