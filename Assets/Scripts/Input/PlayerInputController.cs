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

        if (Input.GetKeyDown(KeyCode.Alpha1))
            combatController.ApplyStun();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            combatController.ApplyRoot();

        if (Input.GetKeyDown(KeyCode.Alpha3))
            combatController.CleanseSelf();

        if (Input.GetKeyDown(KeyCode.Alpha4))
            combatController.ApplySlow();

        if (Input.GetKeyDown(KeyCode.Alpha5))
            combatController.ApplySilence();

        if (Input.GetKeyDown(KeyCode.Alpha6))
            combatController.ApplyAttackBuff();

        if (Input.GetKeyDown(KeyCode.Alpha7))
            combatController.ApplyAttackDebuff();
    }
}