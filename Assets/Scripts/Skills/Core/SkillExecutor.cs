using UnityEngine;
using UnityEngine.EventSystems;
using ProjectPinch.Stats.Core;

public class SkillExecutor : MonoBehaviour
{
    public static SkillExecutor Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private SkillCooldownManager cooldownManager;

    private CharacterStats casterStats;
    private SkillData pendingSkill;
    private bool isAiming;

    public bool IsAiming => isAiming;
    public SkillData PendingSkill => pendingSkill;

    private void Awake()
    {
        Instance = this;

        casterStats = GetComponent<CharacterStats>();

        if (cooldownManager == null)
            cooldownManager = FindFirstObjectByType<SkillCooldownManager>();

        if (playerTransform == null)
            playerTransform = transform;
    }

    public void TryCast(SkillData skill)
    {
        BeginAim(skill);
    }

    public void BeginAim(SkillData skill)
    {
        if (skill == null)
            return;

        if (cooldownManager == null)
            return;

        if (!cooldownManager.CanUseSkill(skill))
        {
            LogCooldownBlock(skill);
            return;
        }

        if (casterStats != null && !casterStats.HasEnoughMana(skill.manaCost))
            return;

        pendingSkill = skill;
        isAiming = true;

        if (CursorManager.Instance != null)
            CursorManager.Instance.SetCursor(GameCursorState.SpellSelected);
    }

    public void ConfirmCast()
    {
        if (!isAiming || pendingSkill == null)
            return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        SkillData skill = pendingSkill;

        if (cooldownManager == null)
        {
            CancelAim();
            return;
        }

        if (!cooldownManager.CanUseSkill(skill))
        {
            LogCooldownBlock(skill);
            CancelAim();
            return;
        }

        if (casterStats != null && !casterStats.HasEnoughMana(skill.manaCost))
        {
            CancelAim();
            return;
        }

        if (playerTransform == null)
        {
            CancelAim();
            return;
        }

        if (skill.targetType == SkillTargetType.Self)
        {
            if (!SpendMana(skill))
            {
                CancelAim();
                return;
            }

            CastSelfSkill(skill);
            cooldownManager.StartCooldown(skill);
            ClearAim();
            return;
        }

        Targetable target = GetClickedTarget();

        if (target == null)
        {
            FailCast(skill);
            return;
        }

        float distance = Vector3.Distance(
            playerTransform.position,
            target.transform.position
        );

        if (distance > skill.range)
        {
            FailCast(skill);
            return;
        }

        CharacterStats targetStats =
            target.GetComponentInParent<CharacterStats>();

        if (targetStats == null)
        {
            FailCast(skill);
            return;
        }

        if (!SpendMana(skill))
        {
            CancelAim();
            return;
        }

        int finalDamage = CalculateSkillDamage(skill);

        targetStats.TakeDamage(
            finalDamage,
            new Color(0.3f, 0.8f, 1f),
            false
        );

        cooldownManager.StartCooldown(skill);
        ClearAim();
    }

    public void CancelAim()
    {
        if (!isAiming)
            return;

        ClearAim();
    }

    private bool SpendMana(SkillData skill)
    {
        if (casterStats == null)
            return true;

        return casterStats.SpendMana(skill.manaCost);
    }

    private int CalculateSkillDamage(SkillData skill)
    {
        int finalDamage = skill.damage;

        if (casterStats != null && casterStats.FinalStats != null)
            finalDamage += casterStats.FinalStats.magicPower;

        return Mathf.Max(finalDamage, 0);
    }

    private Targetable GetClickedTarget()
    {
        Camera cam = Camera.main;

        if (cam == null)
            return null;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 500f);

        System.Array.Sort(
            hits,
            (a, b) => a.distance.CompareTo(b.distance)
        );

        foreach (RaycastHit hit in hits)
        {
            Targetable target =
                hit.collider.GetComponentInParent<Targetable>();

            if (target != null)
                return target;
        }

        return null;
    }

    private void FailCast(SkillData skill)
    {
        if (cooldownManager != null && skill != null)
            cooldownManager.StartCooldown(skill);

        ClearAim();
    }

    private void ClearAim()
    {
        pendingSkill = null;
        isAiming = false;

        if (CursorManager.Instance != null)
            CursorManager.Instance.ResetCursor();
    }

    private void CastSelfSkill(SkillData skill)
    {
        if (skill.damage < 0 && casterStats != null)
        {
            int healAmount = Mathf.Abs(skill.damage);

            if (casterStats.FinalStats != null)
                healAmount += casterStats.FinalStats.magicPower;

            casterStats.Heal(healAmount);
        }
    }

    private void LogCooldownBlock(SkillData skill)
    {
        float skillRemaining = cooldownManager.GetSkillCooldownRemaining(skill);
        float gcdRemaining = cooldownManager.GetGlobalCooldownRemaining();

        if (skillRemaining > 0f)
            Debug.LogWarning($"{skill.skillName} CD: {skillRemaining:F1}s");

        if (gcdRemaining > 0f)
            Debug.LogWarning($"GCD: {gcdRemaining:F1}s");
    }
}