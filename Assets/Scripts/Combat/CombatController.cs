using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("Basic Attack")]
    public int basicAttackDamage = 20;
    public float attackRange = 3f;
    public float basicAttackAngle = 70f;
    public Transform facingReference;

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

        if (facingReference == null)
            facingReference = transform;

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
        List<string> keys = new List<string>(cooldownTimers.Keys);

        foreach (string key in keys)
        {
            if (cooldownTimers[key] > 0f)
                cooldownTimers[key] = Mathf.Max(cooldownTimers[key] - Time.deltaTime, 0f);
        }
    }

    private bool IsOnCooldown(string skillName)
    {
        return cooldownTimers.ContainsKey(skillName) && cooldownTimers[skillName] > 0f;
    }

    private void StartCooldown(string skillName, float duration)
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

    private GameObject FindNearestEnemyInFront()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        Vector3 forward = facingReference.forward;
        forward.y = 0f;
        forward.Normalize();

        foreach (GameObject enemy in enemies)
        {
            if (!enemy.activeSelf)
                continue;

            Vector3 directionToEnemy = enemy.transform.position - transform.position;
            directionToEnemy.y = 0f;

            float distance = directionToEnemy.magnitude;

            if (distance > attackRange)
                continue;

            directionToEnemy.Normalize();

            float angle = Vector3.Angle(forward, directionToEnemy);

            if (angle > basicAttackAngle * 0.5f)
                continue;

            if (distance < nearestDistance)
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
            return;

        if (IsOnCooldown(skillName))
            return;

        GameObject enemy = FindNearestEnemyInFront();

        if (enemy == null)
            return;

        Health health = enemy.GetComponent<Health>();

        if (health == null)
            return;

        float attackMultiplier = 1f;

        if (selfStatusEffects != null)
            attackMultiplier = selfStatusEffects.GetAttackMultiplier();

        int finalDamage = Mathf.RoundToInt(basicAttackDamage * attackMultiplier);

        StatusEffectController enemyStatus = enemy.GetComponent<StatusEffectController>();

        if (enemyStatus != null)
        {
            float defenseMultiplier = enemyStatus.GetDefenseMultiplier();
            finalDamage = Mathf.RoundToInt(finalDamage / defenseMultiplier);
        }

        health.TakeDamage(
            finalDamage,
            new Color(1f, 0.9f, 0.3f),
            false
        );

        StartCooldown(skillName, basicAttackCooldown);
    }

    public void ApplyStun()
    {
        ApplyEffectSkill("Stun", StatusEffectType.Stun, stunDuration, stunCooldown);
    }

    public void ApplyRoot()
    {
        ApplyEffectSkill("Root", StatusEffectType.Root, rootDuration, rootCooldown);
    }

    public void ApplySlow()
    {
        ApplyEffectSkill("Slow", StatusEffectType.Slow, slowDuration, slowCooldown);
    }

    public void ApplySilence()
    {
        ApplyEffectSkill("Silence", StatusEffectType.Silence, silenceDuration, silenceCooldown);
    }

    public void ApplyAttackDebuff()
    {
        ApplyEffectSkill("AttackDebuff", StatusEffectType.AttackDebuff, debuffDuration, attackDebuffCooldown);
    }

    public void ApplyAttackBuff()
    {
        string skillName = "AttackBuff";

        if (!CanAct())
            return;

        if (IsOnCooldown(skillName))
            return;

        if (selfStatusEffects == null)
            return;

        selfStatusEffects.ApplyEffect(StatusEffectType.AttackBuff, buffDuration);

        StartCooldown(skillName, attackBuffCooldown);
    }

    public void CleanseSelf()
    {
        string skillName = "Cleanse";

        if (IsOnCooldown(skillName))
            return;

        if (selfStatusEffects == null)
            return;

        selfStatusEffects.RemoveNegativeEffects();

        StartCooldown(skillName, cleanseCooldown);
    }

    private void ApplyEffectSkill(
        string skillName,
        StatusEffectType effectType,
        float effectDuration,
        float cooldownDuration
    )
    {
        if (!CanAct())
            return;

        if (IsOnCooldown(skillName))
            return;

        GameObject enemy = FindNearestEnemyInFront();

        if (enemy == null)
            return;

        StatusEffectController enemyStatus = enemy.GetComponent<StatusEffectController>();

        if (enemyStatus == null)
            return;

        enemyStatus.ApplyEffect(effectType, effectDuration);

        StartCooldown(skillName, cooldownDuration);
    }
}