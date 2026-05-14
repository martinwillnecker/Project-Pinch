using TMPro;
using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI damageText;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 40f;
    [SerializeField] private float lifetime = 1f;

    [Header("Scale")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float critScale = 1.35f;

    private float timer;
    private Color startColor;

    private void Awake()
    {
        if (damageText == null)
            damageText = GetComponentInChildren<TextMeshProUGUI>();

        if (damageText == null)
        {
            Debug.LogWarning("FloatingDamageText: no encontré TextMeshProUGUI.");
            return;
        }

        startColor = damageText.color;
    }

    private void OnEnable()
    {
        timer = 0f;
    }

    private void Update()
    {
        if (damageText == null)
            return;

        timer += Time.deltaTime;

        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);

        damageText.color =
            new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (timer >= lifetime)
            Destroy(gameObject);
    }

    public void Setup(int damage, bool isCrit, Color textColor)
    {
        if (damageText == null)
            damageText = GetComponentInChildren<TextMeshProUGUI>();

        if (damageText == null)
        {
            Debug.LogWarning("FloatingDamageText: damageText es NULL en Setup.");
            return;
        }

        damageText.text = isCrit ? $"{damage}!" : damage.ToString();
        damageText.color = textColor;
        startColor = textColor;

        transform.localScale =
            Vector3.one * (isCrit ? critScale : normalScale);
    }
}