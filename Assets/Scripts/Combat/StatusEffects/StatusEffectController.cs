using System.Collections.Generic;
using UnityEngine;

public class StatusEffectController : MonoBehaviour
{
    private Dictionary<StatusEffectType, float> activeEffects =
        new Dictionary<StatusEffectType, float>();

    private void Update()
    {
        UpdateEffects();
    }

    private void UpdateEffects()
    {
        List<StatusEffectType> keys =
            new List<StatusEffectType>(activeEffects.Keys);

        List<StatusEffectType> expiredEffects =
            new List<StatusEffectType>();

        foreach (StatusEffectType effectType in keys)
        {
            activeEffects[effectType] -= Time.deltaTime;

            if (activeEffects[effectType] <= 0f)
            {
                expiredEffects.Add(effectType);
            }
        }

        foreach (StatusEffectType effectType in expiredEffects)
        {
            activeEffects.Remove(effectType);

            Debug.Log(
                $"{gameObject.name} perdió efecto: {effectType}"
            );
        }
    }

    public void ApplyEffect(
        StatusEffectType effectType,
        float duration
    )
    {
        activeEffects[effectType] = duration;

        Debug.Log(
            $"{gameObject.name} recibió efecto: {effectType} por {duration}s"
        );
    }

    public void RemoveEffect(StatusEffectType effectType)
    {
        if (activeEffects.ContainsKey(effectType))
        {
            activeEffects.Remove(effectType);

            Debug.Log(
                $"{gameObject.name} limpió efecto: {effectType}"
            );
        }
    }

    public void RemoveNegativeEffects()
    {
        RemoveEffect(StatusEffectType.Stun);
        RemoveEffect(StatusEffectType.Root);
        RemoveEffect(StatusEffectType.Slow);
        RemoveEffect(StatusEffectType.Silence);
        RemoveEffect(StatusEffectType.AttackDebuff);
        RemoveEffect(StatusEffectType.DefenseDebuff);
        RemoveEffect(StatusEffectType.SpeedDebuff);
    }

    public bool HasEffect(StatusEffectType effectType)
    {
        return activeEffects.ContainsKey(effectType);
    }

    public bool IsStunned()
    {
        return HasEffect(StatusEffectType.Stun);
    }

    public bool IsRooted()
    {
        return HasEffect(StatusEffectType.Root);
    }

    public bool IsSilenced()
    {
        return HasEffect(StatusEffectType.Silence);
    }

    public bool IsSlowed()
    {
        return HasEffect(StatusEffectType.Slow);
    }

    public bool CanMove()
    {
        return !HasEffect(StatusEffectType.Stun)
            && !HasEffect(StatusEffectType.Root);
    }

    public bool CanAct()
    {
        return !HasEffect(StatusEffectType.Stun)
            && !HasEffect(StatusEffectType.Silence);
    }

    public float GetSpeedMultiplier()
    {
        if (HasEffect(StatusEffectType.SpeedBuff))
            return 1.3f;

        if (
            HasEffect(StatusEffectType.Slow) ||
            HasEffect(StatusEffectType.SpeedDebuff)
        )
        {
            return 0.5f;
        }

        return 1f;
    }

    public float GetAttackMultiplier()
    {
        if (HasEffect(StatusEffectType.AttackBuff))
            return 1.3f;

        if (HasEffect(StatusEffectType.AttackDebuff))
            return 0.7f;

        return 1f;
    }

    public float GetDefenseMultiplier()
    {
        if (HasEffect(StatusEffectType.DefenseBuff))
            return 1.3f;

        if (HasEffect(StatusEffectType.DefenseDebuff))
            return 0.7f;

        return 1f;
    }

    // =========================
    // Helper Methods
    // =========================

    public void ApplyStun(float duration)
    {
        ApplyEffect(StatusEffectType.Stun, duration);
    }

    public void ApplyRoot(float duration)
    {
        ApplyEffect(StatusEffectType.Root, duration);
    }

   public void ApplySlow(float multiplier, float duration)
{
    ApplyEffect(StatusEffectType.Slow, duration);
}

    public void ApplySilence(float duration)
    {
        ApplyEffect(StatusEffectType.Silence, duration);
    }

   public void ApplyAttackBuff(float multiplier, float duration)
    {
        ApplyEffect(StatusEffectType.AttackBuff, duration);
    }

    public void ApplyAttackDebuff(float multiplier, float duration)
{
    ApplyEffect(StatusEffectType.AttackDebuff, duration);
}
    public void ApplyDefenseBuff(float duration)
    {
        ApplyEffect(StatusEffectType.DefenseBuff, duration);
    }

    public void ApplyDefenseDebuff(float duration)
    {
        ApplyEffect(StatusEffectType.DefenseDebuff, duration);
    }

    public void ApplySpeedBuff(float duration)
    {
        ApplyEffect(StatusEffectType.SpeedBuff, duration);
    }

    public void ApplySpeedDebuff(float duration)
    {
        ApplyEffect(StatusEffectType.SpeedDebuff, duration);
    }

    public void Cleanse()
    {
        RemoveNegativeEffects();
    }
}