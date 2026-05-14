using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectPinch.Stats.Core;

public class EnemyHealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TargetSelector targetSelector;

    private CharacterStats currentTargetStats;

    private void Awake()
    {
        if (targetSelector == null)
            targetSelector = FindFirstObjectByType<TargetSelector>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        HideBar();
    }

    private void Update()
    {
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        if (targetSelector == null)
            return;

        Targetable target = targetSelector.CurrentTarget;

        if (target == null)
        {
            HideBar();
            return;
        }

        CharacterStats targetStats =
            target.GetComponentInParent<CharacterStats>();

        if (targetStats == null)
        {
            HideBar();
            return;
        }

        if (currentTargetStats != targetStats)
            SetTarget(targetStats);
    }

    private void SetTarget(CharacterStats stats)
    {
        if (currentTargetStats != null)
            currentTargetStats.OnHealthChanged -= UpdateHealthBar;

        currentTargetStats = stats;
        currentTargetStats.OnHealthChanged += UpdateHealthBar;

        UpdateHealthBar(
            currentTargetStats.CurrentHealth,
            currentTargetStats.MaxHealth
        );

        ShowBar();
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthSlider == null || maxHealth <= 0)
            return;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";
    }

    private void ShowBar()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void HideBar()
    {
        if (currentTargetStats != null)
            currentTargetStats.OnHealthChanged -= UpdateHealthBar;

        currentTargetStats = null;

        if (canvasGroup == null)
            return;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}