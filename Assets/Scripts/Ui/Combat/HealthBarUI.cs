using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectPinch.Stats.Core;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStats targetStats;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    private void Start()
    {
        if (targetStats == null)
            targetStats = FindFirstObjectByType<CharacterStats>();

        if (targetStats != null)
        {
            targetStats.OnHealthChanged += UpdateHealthBar;

            UpdateHealthBar(
                targetStats.CurrentHealth,
                targetStats.MaxHealth
            );
        }
    }

    private void OnDestroy()
    {
        if (targetStats != null)
        {
            targetStats.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthSlider == null || maxHealth <= 0)
            return;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }
}