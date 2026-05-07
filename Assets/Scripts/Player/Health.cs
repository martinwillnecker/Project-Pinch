using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{gameObject.name} recibió {amount} daño. HP: {currentHealth}/{maxHealth}");

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} murió.");
        gameObject.SetActive(false);
    }
}