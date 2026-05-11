using UnityEngine;

public class FacingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform facingArrow;
    [SerializeField] private Transform cameraTransform;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (facingArrow == null || cameraTransform == null)
            return;

        Vector3 directionToCamera =
            cameraTransform.position - facingArrow.position;

        directionToCamera.y = 0f;

        if (directionToCamera.sqrMagnitude < 0.001f)
            return;

        facingArrow.rotation =
            Quaternion.LookRotation(directionToCamera);
    }
}