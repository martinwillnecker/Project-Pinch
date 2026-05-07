using UnityEngine;

public class MMOCameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Zoom")]
    public float distance = 15f;
    public float minDistance = 8f;
    public float maxDistance = 25f;
    public float zoomSpeed = 8f;

    [Header("Rotation")]
    public float rotationSpeed = 180f;

    [Header("Pitch")]
    public float pitch = 45f;
    public float minPitch = 25f;
    public float maxPitch = 75f;

    private float yaw = 0f;

    private void Start()
    {
        if (target != null)
        {
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;
        }
    }

    private void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);

        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    private void HandleRotation()
    {
    if (!Input.GetMouseButton(1)) return;

    float mouseX = Input.GetAxis("Mouse X");
    float mouseY = Input.GetAxis("Mouse Y");

    yaw += mouseX * rotationSpeed;
    pitch -= mouseY * rotationSpeed;

    pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) < 0.01f) return;

        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }
}