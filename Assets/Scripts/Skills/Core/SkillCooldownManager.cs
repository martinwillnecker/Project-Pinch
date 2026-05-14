using System.Collections.Generic;
using UnityEngine;

public class SkillCooldownManager : MonoBehaviour
{
    [Header("Global Cooldown")]
    [SerializeField] private float globalCooldownDuration = 1f;

    private float globalCooldownEndTime = 0f;

    private readonly Dictionary<SkillData, float> skillCooldownEndTimes = new();

    public bool IsGlobalCooldownActive()
    {
        return Time.time < globalCooldownEndTime;
    }

    public float GetGlobalCooldownRemaining()
    {
        return Mathf.Max(0f, globalCooldownEndTime - Time.time);
    }

    public bool IsSkillOnCooldown(SkillData skill)
    {
        if (skill == null)
            return false;

        if (!skillCooldownEndTimes.ContainsKey(skill))
            return false;

        return Time.time < skillCooldownEndTimes[skill];
    }

    public float GetSkillCooldownRemaining(SkillData skill)
    {
        if (skill == null)
            return 0f;

        if (!skillCooldownEndTimes.ContainsKey(skill))
            return 0f;

        return Mathf.Max(0f, skillCooldownEndTimes[skill] - Time.time);
    }

    public float GetSkillCooldownPercent(SkillData skill)
    {
        if (skill == null || skill.cooldown <= 0f)
            return 0f;

        float remaining = GetSkillCooldownRemaining(skill);
        return Mathf.Clamp01(remaining / skill.cooldown);
    }

    public bool CanUseSkill(SkillData skill)
    {
        if (skill == null)
            return false;

        if (IsGlobalCooldownActive())
            return false;

        if (IsSkillOnCooldown(skill))
            return false;

        return true;
    }

    public void StartCooldown(SkillData skill)
    {
        if (skill == null)
            return;

        if (skill.cooldown > 0f)
        {
            skillCooldownEndTimes[skill] = Time.time + skill.cooldown;
        }

        if (skill.usesGlobalCooldown)
        {
            globalCooldownEndTime = Time.time + globalCooldownDuration;
        }
    }
}