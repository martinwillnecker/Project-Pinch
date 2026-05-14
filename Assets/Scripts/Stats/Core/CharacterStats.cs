using System;
using UnityEngine;
using ProjectPinch.Stats.Data;

namespace ProjectPinch.Stats.Core
{
    public class CharacterStats : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField]
        private ProjectPinch.Stats.Data.RaceData raceData;

        [SerializeField]
        private ProjectPinch.Stats.Data.ClassData classData;

        [SerializeField] private int level = 1;

        [Header("Runtime")]
        [SerializeField] private int currentHealth;
        [SerializeField] private int currentMana;

        private StatBlock finalStats;

        public StatBlock FinalStats => finalStats;

        public int CurrentHealth => currentHealth;
        public int CurrentMana => currentMana;

        public int MaxHealth =>
            finalStats != null ? finalStats.maxHealth : 0;

        public int MaxMana =>
            finalStats != null ? finalStats.maxMana : 0;

        public bool IsDead => currentHealth <= 0;

        public event Action<int, int> OnHealthChanged;
        public event Action<int, int> OnManaChanged;
        public event Action OnDeath;

        private void Awake()
        {
            RecalculateStats();

            currentHealth = finalStats.maxHealth;
            currentMana = finalStats.maxMana;

            NotifyHealthChanged();
            NotifyManaChanged();
        }

        public void RecalculateStats()
        {
            finalStats = new StatBlock();

            if (raceData != null)
                finalStats += raceData.baseStats;

            if (classData != null)
                finalStats += classData.baseStats;

            ApplyLevelScaling();

            currentHealth =
                Mathf.Clamp(
                    currentHealth,
                    0,
                    finalStats.maxHealth
                );

            currentMana =
                Mathf.Clamp(
                    currentMana,
                    0,
                    finalStats.maxMana
                );
        }

        private void ApplyLevelScaling()
        {
            int extraLevels = Mathf.Max(level - 1, 0);

            finalStats.maxHealth += extraLevels * 10;
            finalStats.maxMana += extraLevels * 5;
            finalStats.physicalAttack += extraLevels * 2;
            finalStats.magicPower += extraLevels * 2;
            finalStats.defense += extraLevels;
        }

        public void TakeDamage(int rawDamage)
        {
            if (IsDead)
                return;

            int finalDamage =
                Mathf.Max(rawDamage - finalStats.defense, 1);

            currentHealth -= finalDamage;

            currentHealth =
                Mathf.Clamp(
                    currentHealth,
                    0,
                    finalStats.maxHealth
                );

            if (FloatingTextSpawner.Instance != null)
            {
                FloatingTextSpawner.Instance.SpawnDamageText(
                    transform.position,
                    finalDamage,
                    false,
                    Color.white
                );
            }

            NotifyHealthChanged();

            if (currentHealth <= 0)
                Die();
        }

        public void TakeDamage(
            int rawDamage,
            Color damageColor,
            bool isCritical
        )
        {
            if (IsDead)
                return;

            int finalDamage =
                Mathf.Max(rawDamage - finalStats.defense, 1);

            currentHealth -= finalDamage;

            currentHealth =
                Mathf.Clamp(
                    currentHealth,
                    0,
                    finalStats.maxHealth
                );

            if (FloatingTextSpawner.Instance != null)
            {
                FloatingTextSpawner.Instance.SpawnDamageText(
                    transform.position,
                    finalDamage,
                    isCritical,
                    damageColor
                );
            }

            NotifyHealthChanged();

            if (currentHealth <= 0)
                Die();
        }

        public void Heal(int amount)
        {
            if (IsDead)
                return;

            currentHealth += amount;

            currentHealth =
                Mathf.Clamp(
                    currentHealth,
                    0,
                    finalStats.maxHealth
                );

            NotifyHealthChanged();
        }

        public bool HasEnoughMana(int amount)
        {
            return currentMana >= amount;
        }

        public bool SpendMana(int amount)
        {
            if (!HasEnoughMana(amount))
                return false;

            currentMana -= amount;

            currentMana =
                Mathf.Clamp(
                    currentMana,
                    0,
                    finalStats.maxMana
                );

            NotifyManaChanged();

            return true;
        }

        public void RestoreMana(int amount)
        {
            currentMana += amount;

            currentMana =
                Mathf.Clamp(
                    currentMana,
                    0,
                    finalStats.maxMana
                );

            NotifyManaChanged();
        }

        private void NotifyHealthChanged()
        {
            OnHealthChanged?.Invoke(
                currentHealth,
                finalStats.maxHealth
            );
        }

        private void NotifyManaChanged()
        {
            OnManaChanged?.Invoke(
                currentMana,
                finalStats.maxMana
            );
        }

        private void Die()
        {
            OnDeath?.Invoke();

            Debug.Log($"{gameObject.name} murió.");
        }

        [ContextMenu("Debug Final Stats")]
        private void DebugFinalStats()
        {
            Debug.Log(
                $"HP: {finalStats.maxHealth}\n" +
                $"Mana: {finalStats.maxMana}\n" +
                $"Physical: {finalStats.physicalAttack}\n" +
                $"Magic: {finalStats.magicPower}\n" +
                $"Defense: {finalStats.defense}\n" +
                $"Speed: {finalStats.moveSpeed}"
            );
        }
    }
}