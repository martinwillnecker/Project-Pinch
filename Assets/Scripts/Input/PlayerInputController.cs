using UnityEngine;

[RequireComponent(typeof(CombatController))]
public class PlayerInputController : MonoBehaviour
{
    private CombatController combatController;

    private void Awake()
    {
        combatController = GetComponent<CombatController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            combatController.BasicAttack();

        if (Input.GetMouseButtonDown(0))
        {
            if (SkillExecutor.Instance != null && SkillExecutor.Instance.IsAiming)
                SkillExecutor.Instance.ConfirmCast();
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (SkillExecutor.Instance != null && SkillExecutor.Instance.IsAiming)
                SkillExecutor.Instance.CancelAim();
        }

        // Las skills Alpha1-Alpha9 las sigue manejando ActionBarSlotUI.
        // La ActionBar llama a SkillExecutor.TryCast(skill),
        // pero ahora TryCast solo entra en modo aiming.
    }
}