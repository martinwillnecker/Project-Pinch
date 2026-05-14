using System.Collections.Generic;
using UnityEngine;
using ProjectPinch.Stats.Core;

public class CombatController : MonoBehaviour
{
    [Header("Basic Attack")]
    public int basicAttackDamage = 20;
    public float attackRange = 3f;
    public float basicAttackAngle = 70f;
    public Transform facingReference;

    [Header("Cooldowns")]
    public float basicAttackCooldown = 1f;
    public float stunCooldown = 5f;
    public float rootCooldown = 5f;
    public float cleanseCooldown = 8f;
    public float slowCooldown = 5f;
    public float silenceCooldown = 5f;
    public float attackBuffCooldown = 8f;
    public float attackDebuffCooldown = 8f;

    private StatusEffectController selfStatusEffects;
    private CharacterStats characterStats;

    private Dictionary<string, float> cooldownTimers =
        new Dictionary<string, float>();

    private void Awake()
    {
        selfStatusEffects = GetComponent<StatusEffectController>();
        characterStats = GetComponent<CharacterStats>();

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
        List<string> keys = new List<string>(cooldownTimers.Keys);

        foreach (string key in keys)
        {
            if (cooldownTimers[key] > 0f)
            {
                cooldownTimers[key] -= Time.deltaTime;
            }
        }
    }

    private bool IsOnCooldown(string abilityName)
    {
        return cooldownTimers[abilityName] > 0f;
    }

    private void StartCooldown(string abilityName, float duration)
    {
        cooldownTimers[abilityName] = duration;
    }

    public void BasicAttack()
    {
        if (IsOnCooldown("BasicAttack"))
        {
            Debug.Log("Basic Attack en cooldown.");
            return;
        }

        if (selfStatusEffects != null && selfStatusEffects.IsStunned())
        {
            Debug.Log("No podés atacar mientras estás stun.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            if (!hit.CompareTag("Enemy"))
                continue;

            Vector3 directionToTarget =
                (hit.transform.position - transform.position).normalized;

            float angle =
                Vector3.Angle(facingReference.forward, directionToTarget);

            if (angle > basicAttackAngle * 0.5f)
                continue;

            CharacterStats targetStats =
            hit.GetComponent<CharacterStats>();

            if (targetStats != null)
            {
                float attackMultiplier = 1f;

                if (selfStatusEffects != null)
                {
                    attackMultiplier =
                        selfStatusEffects.GetAttackMultiplier();
                }

                int baseDamage = basicAttackDamage;

                if (characterStats != null &&
                    characterStats.FinalStats != null)
                {
                    baseDamage =
                        characterStats.FinalStats.physicalAttack;
                }

                int finalDamage =
                    Mathf.RoundToInt(baseDamage * attackMultiplier);

                targetStats.TakeDamage(finalDamage);

                Debug.Log(
                    $"Basic Attack hit {hit.name} for {finalDamage} damage."
                );

                StartCooldown("BasicAttack", basicAttackCooldown);

                return;
            }
        }

        Debug.Log("No enemy hit.");
        StartCooldown("BasicAttack", basicAttackCooldown);
    }

    public void ApplyStun()
    {
        if (IsOnCooldown("Stun"))
        {
            Debug.Log("Stun en cooldown.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            StatusEffectController status =
                hit.GetComponent<StatusEffectController>();

            if (status != null)
            {
                status.ApplyStun(2f);

                Debug.Log($"Stunned {hit.name}");

                StartCooldown("Stun", stunCooldown);

                return;
            }
        }
    }

    public void ApplyRoot()
    {
        if (IsOnCooldown("Root"))
        {
            Debug.Log("Root en cooldown.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            StatusEffectController status =
                hit.GetComponent<StatusEffectController>();

            if (status != null)
            {
                status.ApplyRoot(3f);

                Debug.Log($"Rooted {hit.name}");

                StartCooldown("Root", rootCooldown);

                return;
            }
        }
    }

    public void ApplySlow()
    {
        if (IsOnCooldown("Slow"))
        {
            Debug.Log("Slow en cooldown.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            StatusEffectController status =
                hit.GetComponent<StatusEffectController>();

            if (status != null)
            {
                status.ApplySlow(0.5f, 4f);

                Debug.Log($"Slowed {hit.name}");

                StartCooldown("Slow", slowCooldown);

                return;
            }
        }
    }

    public void ApplySilence()
    {
        if (IsOnCooldown("Silence"))
        {
            Debug.Log("Silence en cooldown.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            StatusEffectController status =
                hit.GetComponent<StatusEffectController>();

            if (status != null)
            {
                status.ApplySilence(3f);

                Debug.Log($"Silenced {hit.name}");

                StartCooldown("Silence", silenceCooldown);

                return;
            }
        }
    }

    public void ApplyCleanse()
    {
        if (IsOnCooldown("Cleanse"))
        {
            Debug.Log("Cleanse en cooldown.");
            return;
        }

        if (selfStatusEffects != null)
        {
            selfStatusEffects.Cleanse();

            Debug.Log("Cleanse aplicado.");

            StartCooldown("Cleanse", cleanseCooldown);
        }
    }

    public void ApplyAttackBuff()
    {
        if (IsOnCooldown("AttackBuff"))
        {
            Debug.Log("Attack Buff en cooldown.");
            return;
        }

        if (selfStatusEffects != null)
        {
            selfStatusEffects.ApplyAttackBuff(1.5f, 5f);

            Debug.Log("Attack Buff aplicado.");

            StartCooldown("AttackBuff", attackBuffCooldown);
        }
    }

    public void ApplyAttackDebuff()
    {
        if (IsOnCooldown("AttackDebuff"))
        {
            Debug.Log("Attack Debuff en cooldown.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            StatusEffectController status =
                hit.GetComponent<StatusEffectController>();

            if (status != null)
            {
                status.ApplyAttackDebuff(0.5f, 5f);

                Debug.Log($"Attack Debuff aplicado a {hit.name}");

                StartCooldown(
                    "AttackDebuff",
                    attackDebuffCooldown
                );

                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (facingReference != null)
        {
            Vector3 leftBoundary =
                Quaternion.Euler(0, -basicAttackAngle / 2, 0)
                * facingReference.forward;

            Vector3 rightBoundary =
                Quaternion.Euler(0, basicAttackAngle / 2, 0)
                * facingReference.forward;

            Gizmos.color = Color.yellow;

            Gizmos.DrawRay(
                transform.position,
                leftBoundary * attackRange
            );

            Gizmos.DrawRay(
                transform.position,
                rightBoundary * attackRange
            );
        }
    }
}