using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private Camera mainCamera;

    [Header("Prefab")]
    [SerializeField] private FloatingDamageText damageTextPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 2f, 0f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public void SpawnDamageText(Vector3 worldPosition, int damage, bool isCrit, Color textColor)
    {
        if (damageTextPrefab == null)
        {
            Debug.LogWarning("Damage Text Prefab is missing in FloatingTextSpawner.");
            return;
        }

        if (targetCanvas == null)
        {
            Debug.LogWarning("Target Canvas is missing in FloatingTextSpawner.");
            return;
        }

        if (mainCamera == null)
        {
            Debug.LogWarning("Main Camera is missing in FloatingTextSpawner.");
            return;
        }

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition + worldOffset);

        FloatingDamageText textInstance = Instantiate(
            damageTextPrefab,
            targetCanvas.transform
        );

        RectTransform rectTransform = textInstance.GetComponent<RectTransform>();
        rectTransform.position = screenPosition;

        textInstance.Setup(damage, isCrit, textColor);

        Debug.Log($"Damage text spawned: {damage}");
    }
}