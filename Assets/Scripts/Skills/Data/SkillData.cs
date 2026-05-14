using UnityEngine;

public enum SkillTargetType
{
    None,
    Self,
    Enemy,
    Ally,
    Area
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "Project Pinch/Skills/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Basic Info")]
    public int id;
    public string skillName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Combat")]
    public float cooldown = 1f;
    public bool usesGlobalCooldown = true;
    public float castTime = 0f;
    public int manaCost = 0;
    public float range = 2f;

    [Header("Targeting")]
    public SkillTargetType targetType = SkillTargetType.Enemy;

    [Header("Debug")]
    public int damage = 10;
}