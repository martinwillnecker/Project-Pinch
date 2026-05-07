using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("Basic Attack")]
    public int basicAttackDamage = 20;
    public float attackRange = 3f;

    [Header("Crowd Control")]
    public float stunDuration = 2f;
    public float rootDuration = 3f;
    public float slowDuration = 4f;
    public float silenceDuration = 2f;

    [Header("Buffs / Debuffs")]
    public float buffDuration = 5f;
    public float debuffDuration = 5f;

    [Header("Cooldowns")]
    public float basicAttackCooldown = 1f;
    public float stunCooldown = 8f;
    public float rootCooldown = 6f;
    public float cleanseCooldown = 12f;
    public float slowCooldown = 5f;
    public float silenceCooldown = 10f;
    public float attackBuffCooldown = 15f;
    public float attackDebuffCooldown = 8f;

    private StatusEffectController selfStatusEffects;

    private Dictionary<string, float> cooldownTimers =
        new Dictionary<string, float>();

    private void Awake()
    {
        selfStatusEffects = GetComponent<StatusEffectController>();

        cooldownTimers["BasicAttack"] = 0f;
        cooldownTimers["Stun"] = 0f;
        cooldownTimers["Root"] = 0f;
        cooldownTimers["Cleanse"] = 0f;
        cooldownTimers["Slow"] = 0f;
        cooldownTimers["Silence"] = 0f;
        cooldownTimers["AttackBuff"] = 0f;
        cooldownTimers["AttackDebuff"] = 0f;
    }

    private void Update()
    {
        UpdateCooldowns();
    }

    private void UpdateCooldowns()
    {
        List<string> keys =
            new List<string>(cooldownTimers.Keys);

        foreach (string key in keys)
        {
            if (cooldownTimers[key] > 0f)
            {
                cooldownTimers[key] -= Time.deltaTime;

                cooldownTimers[key] =
                    Mathf.Max(cooldownTimers[key], 0f);
            }
        }
    }

    private bool IsOnCooldown(string skillName)
    {
        return cooldownTimers.ContainsKey(skillName)
            && cooldownTimers[skillName] > 0f;
    }

    private void StartCooldown(
        string skillName,
        float duration
    )
    {
        cooldownTimers[skillName] = duration;
    }

    private float GetCooldownRemaining(string skillName)
    {
        if (!cooldownTimers.ContainsKey(skillName))
            return 0f;

        return cooldownTimers[skillName];
    }

    private bool CanAct()
    {
        if (selfStatusEffects == null)
            return true;

        return selfStatusEffects.CanAct();
    }

    private GameObject FindNearestEnemyInRange()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        GameObject nearestEnemy = null;

        float nearestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (!enemy.activeSelf)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (
                distance <= attackRange
                && distance < nearestDistance
            )
            {
                nearestEnemy = enemy;
                nearestDistance = distance;
            }
        }

        return nearestEnemy;
    }

    public void BasicAttack()
    {
        string skillName = "BasicAttack";

        if (!CanAct())
        {
            Debug.Log(
                "No podés atacar: stunned o silenced."
            );

            return;
        }

        if (IsOnCooldown(skillName))
        {
            Debug.Log(
                $"Ataque básico en cooldown: " +
                $"{GetCooldownRemaining(skillName):0.0}s"
            );

            return;
        }

        GameObject enemy =
            FindNearestEnemyInRange();

        if (enemy == null)
        {
            Debug.Log("No hay enemigos en rango.");
            return;
        }

        Health health =
            enemy.GetComponent<Health>();

        if (health == null)
        {
            Debug.Log(
                "El enemigo no tiene Health."
            );

            return;
        }

        float attackMultiplier = 1f;

        if (selfStatusEffects != null)
        {
            attackMultiplier =
                selfStatusEffects.GetAttackMultiplier();
        }

        int finalDamage = Mathf.RoundToInt(
            basicAttackDamage * attackMultiplier
        );

        StatusEffectController enemyStatus =
            enemy.GetComponent<StatusEffectController>();

        if (enemyStatus != null)
        {
            float defenseMultiplier =
                enemyStatus.GetDefenseMultiplier();

            finalDamage = Mathf.RoundToInt(
                finalDamage / defenseMultiplier
            );
        }

        health.TakeDamage(finalDamage);

        StartCooldown(
            skillName,
            basicAttackCooldown
        );
    }

    public void ApplyStun()
    {
        ApplyEffectSkill(
            "Stun",
            StatusEffectType.Stun,
            stunDuration,
            stunCooldown
        );
    }

    public void ApplyRoot()
    {
        ApplyEffectSkill(
            "Root",
            StatusEffectType.Root,
            rootDuration,
            rootCooldown
        );
    }

    public void ApplySlow()
    {
        ApplyEffectSkill(
            "Slow",
            StatusEffectType.Slow,
            slowDuration,
            slowCooldown
        );
    }

    public void ApplySilence()
    {
        ApplyEffectSkill(
            "Silence",
            StatusEffectType.Silence,
            silenceDuration,
            silenceCooldown
        );
    }

    public void ApplyAttackDebuff()
    {
        ApplyEffectSkill(
            "AttackDebuff",
            StatusEffectType.AttackDebuff,
            debuffDuration,
            attackDebuffCooldown
        );
    }

    public void ApplyAttackBuff()
    {
        string skillName = "AttackBuff";

        if (!CanAct())
        {
            Debug.Log(
                "No podés aplicar buff."
            );

            return;
        }

        if (IsOnCooldown(skillName))
        {
            Debug.Log(
                $"Attack buff en cooldown: " +
                $"{GetCooldownRemaining(skillName):0.0}s"
            );

            return;
        }

        if (selfStatusEffects == null)
        {
            Debug.Log(
                "El Player no tiene StatusEffectController."
            );

            return;
        }

        selfStatusEffects.ApplyEffect(
            StatusEffectType.AttackBuff,
            buffDuration
        );

        StartCooldown(
            skillName,
            attackBuffCooldown
        );

        Debug.Log("Attack buff aplicado.");
    }

    public void CleanseSelf()
    {
        string skillName = "Cleanse";

        if (IsOnCooldown(skillName))
        {
            Debug.Log(
                $"Cleanse en cooldown: " +
                $"{GetCooldownRemaining(skillName):0.0}s"
            );

            return;
        }

        if (selfStatusEffects == null)
        {
            Debug.Log(
                "El Player no tiene StatusEffectController."
            );

            return;
        }

        selfStatusEffects.RemoveNegativeEffects();

        StartCooldown(
            skillName,
            cleanseCooldown
        );

        Debug.Log("Cleanse usado.");
    }

    private void ApplyEffectSkill(
        string skillName,
        StatusEffectType effectType,
        float effectDuration,
        float cooldownDuration
    )
    {
        if (!CanAct())
        {
            Debug.Log(
                $"No podés usar {skillName}: stunned o silenced."
            );

            return;
        }

        if (IsOnCooldown(skillName))
        {
            Debug.Log(
                $"{skillName} en cooldown: " +
                $"{GetCooldownRemaining(skillName):0.0}s"
            );

            return;
        }

        GameObject enemy =
            FindNearestEnemyInRange();

        if (enemy == null)
        {
            Debug.Log(
                $"No hay enemigos en rango para {skillName}."
            );

            return;
        }

        StatusEffectController enemyStatus =
            enemy.GetComponent<StatusEffectController>();

        if (enemyStatus == null)
        {
            Debug.Log(
                "El enemigo no tiene StatusEffectController."
            );

            return;
        }

        enemyStatus.ApplyEffect(
            effectType,
            effectDuration
        );

        StartCooldown(
            skillName,
            cooldownDuration
        );
    }
}