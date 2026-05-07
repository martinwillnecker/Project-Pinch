using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    public static SkillExecutor Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform playerTransform;

    private void Awake()
    {
        Instance = this;
    }

    public void TryCast(SkillData skill)
    {
        if (skill == null)
            return;

        Targetable target = FindObjectOfType<TargetSelector>().CurrentTarget;

        if (target == null)
        {
            Debug.Log("No tenés target.");
            return;
        }

        float distance = Vector3.Distance(playerTransform.position, target.transform.position);

        if (distance > skill.range)
        {
            Debug.Log("Target fuera de rango.");
            return;
        }

        Health health = target.GetComponent<Health>();

        if (health == null)
        {
            Debug.Log("El target no tiene Health.");
            return;
        }

        health.TakeDamage(skill.damage);

        Debug.Log($"{skill.skillName} impactó a {target.name} por {skill.damage} daño.");
    }
}