using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    public Health targetHealth;

    public Image fillImage;

    public TMP_Text healthText;

    private void Start()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += UpdateHealthBar;

            UpdateHealthBar(
                targetHealth.currentHealth,
                targetHealth.maxHealth
            );
        }
    }

    private void OnDestroy()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(
        int currentHealth,
        int maxHealth
    )
    {
        float fillAmount =
            (float)currentHealth / maxHealth;

        if (fillImage != null)
        {
            fillImage.fillAmount = fillAmount;
        }

        if (healthText != null)
        {
            healthText.text =
                $"{currentHealth}/{maxHealth}";
        }
    }
}