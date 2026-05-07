using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 8f;

    [Header("Wander")]
    public float wanderRadius = 8f;
    public float changeDirectionInterval = 3f;

    private CharacterController controller;
    private StatusEffectController statusEffectController;

    private Vector3 moveDirection;
    private Vector3 startPosition;

    private float timer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        statusEffectController = GetComponent<StatusEffectController>();

        startPosition = transform.position;
        PickNewDirection();
    }

    private void Update()
    {
        if (statusEffectController != null && !statusEffectController.CanMove())
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= changeDirectionInterval)
        {
            timer = 0f;
            PickNewDirection();
        }

        float speedMultiplier = 1f;

        if (statusEffectController != null)
        {
            speedMultiplier = statusEffectController.GetSpeedMultiplier();
        }

        controller.Move(moveDirection * moveSpeed * speedMultiplier * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void PickNewDirection()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;

        Vector3 targetPosition = startPosition + new Vector3(
            randomCircle.x,
            0f,
            randomCircle.y
        );

        moveDirection = (targetPosition - transform.position).normalized;
    }
}