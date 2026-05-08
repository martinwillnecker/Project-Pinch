using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    public static SkillExecutor Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private TargetSelector targetSelector;
    [SerializeField] private SkillCooldownManager cooldownManager;

    private SkillData pendingSkill;
    private bool isAiming;

    public bool IsAiming => isAiming;
    public SkillData PendingSkill => pendingSkill;

    private void Awake()
    {
        Instance = this;

        Debug.Log("SkillExecutor Awake OK");

        if (targetSelector == null)
            targetSelector = FindFirstObjectByType<TargetSelector>();

        if (cooldownManager == null)
            cooldownManager = FindFirstObjectByType<SkillCooldownManager>();
    }

    public void TryCast(SkillData skill)
    {
        BeginAim(skill);
    }

    public void BeginAim(SkillData skill)
    {
        if (skill == null)
        {
            Debug.LogWarning("Skill es NULL");
            return;
        }

        if (cooldownManager == null)
        {
            Debug.LogWarning("No se encontró SkillCooldownManager");
            return;
        }

        if (!cooldownManager.CanUseSkill(skill))
        {
            LogCooldownBlock(skill);
            return;
        }

        pendingSkill = skill;
        isAiming = true;

        Debug.Log($"Aiming iniciado: {skill.skillName}. Hacé click izquierdo para lanzar.");
    }

    public void ConfirmCast()
    {
        if (!isAiming || pendingSkill == null)
            return;

        SkillData skill = pendingSkill;

        if (cooldownManager == null)
        {
            Debug.LogWarning("No se encontró SkillCooldownManager");
            CancelAim();
            return;
        }

        if (!cooldownManager.CanUseSkill(skill))
        {
            LogCooldownBlock(skill);
            CancelAim();
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform no está asignado");
            CancelAim();
            return;
        }

        if (skill.targetType == SkillTargetType.Self)
        {
            CastSelfSkill(skill);
            cooldownManager.StartCooldown(skill);
            ClearAim();
            return;
        }

        if (targetSelector == null)
        {
            Debug.LogWarning("No se encontró TargetSelector");
            CancelAim();
            return;
        }

        Targetable target = targetSelector.CurrentTarget;

        if (target == null)
        {
            Debug.LogWarning("No tenés target seleccionado");
            CancelAim();
            return;
        }

        Debug.Log($"Target seleccionado: {target.name}");

        float distance = Vector3.Distance(playerTransform.position, target.transform.position);

        Debug.Log($"Distancia al target: {distance:F1} / Rango skill: {skill.range}");

        if (distance > skill.range)
        {
            Debug.LogWarning("Target fuera de rango");
            CancelAim();
            return;
        }

        Health health = target.GetComponent<Health>();

        if (health == null)
        {
            Debug.LogWarning("El target no tiene componente Health");
            CancelAim();
            return;
        }

        health.TakeDamage(skill.damage);
        cooldownManager.StartCooldown(skill);

        Debug.Log($"{skill.skillName} impactó a {target.name} por {skill.damage} daño.");

        ClearAim();
    }

    public void CancelAim()
    {
        if (!isAiming)
            return;

        Debug.Log($"Aiming cancelado: {(pendingSkill != null ? pendingSkill.skillName : "sin skill")}");

        ClearAim();
    }

    private void ClearAim()
    {
        pendingSkill = null;
        isAiming = false;
    }

    private void CastSelfSkill(SkillData skill)
    {
        Debug.Log($"{skill.skillName} casteado sobre uno mismo.");

        // Más adelante acá van buffs, cleanses, heals, defensivos, etc.
    }

    private void LogCooldownBlock(SkillData skill)
    {
        float skillRemaining = cooldownManager.GetSkillCooldownRemaining(skill);
        float gcdRemaining = cooldownManager.GetGlobalCooldownRemaining();

        if (skillRemaining > 0f && gcdRemaining > 0f)
        {
            Debug.LogWarning($"{skill.skillName} bloqueada. Skill CD: {skillRemaining:F1}s | GCD: {gcdRemaining:F1}s");
        }
        else if (skillRemaining > 0f)
        {
            Debug.LogWarning($"{skill.skillName} está en cooldown. Falta: {skillRemaining:F1}s");
        }
        else if (gcdRemaining > 0f)
        {
            Debug.LogWarning($"Global Cooldown activo. Falta: {gcdRemaining:F1}s");
        }
    }
}