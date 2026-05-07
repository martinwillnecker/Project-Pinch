using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBarUI : MonoBehaviour
{
    [Header("UI References")]
    public Image fillImage;
    public TMP_Text healthText;

    private Health currentHealth;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        HideBar();
    }

    private void Update()
    {
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        if (TargetManager.Instance == null)
        {
            HideBar();
            return;
        }

        GameObject target = TargetManager.Instance.CurrentTarget;

        if (target == null)
        {
            HideBar();
            return;
        }

        Health health = target.GetComponent<Health>();

        if (health == null)
        {
            HideBar();
            return;
        }

        if (currentHealth != health)
        {
            if (currentHealth != null)
            {
                currentHealth.OnHealthChanged -= UpdateHealthBar;
            }

            currentHealth = health;
            currentHealth.OnHealthChanged += UpdateHealthBar;

            UpdateHealthBar(
                currentHealth.currentHealth,
                currentHealth.maxHealth
            );
        }

        ShowBar();
    }

    private void UpdateHealthBar(
        int currentHealthValue,
        int maxHealthValue
    )
    {
        float fillAmount = (float)currentHealthValue / maxHealthValue;

        if (fillImage != null)
        {
            fillImage.fillAmount = fillAmount;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealthValue}/{maxHealthValue}";
        }
    }

    private void ShowBar()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void HideBar()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (currentHealth != null)
        {
            currentHealth.OnHealthChanged -= UpdateHealthBar;
            currentHealth = null;
        }
    }

    private void OnDestroy()
    {
        if (currentHealth != null)
        {
            currentHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }
}