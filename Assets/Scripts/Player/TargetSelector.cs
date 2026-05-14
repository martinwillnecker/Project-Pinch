using UnityEngine;
using UnityEngine.EventSystems;

public class TargetSelector : MonoBehaviour
{
    [SerializeField] private Targetable currentTarget;
    public Targetable CurrentTarget => currentTarget;

    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        if (SkillExecutor.Instance != null &&
            SkillExecutor.Instance.IsAiming)
        {
            SkillExecutor.Instance.ConfirmCast();
            return;
        }

        SelectTarget();
    }

    private void SelectTarget()
    {
        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 500f);

        if (hits.Length == 0)
        {
            ClearTarget();
            return;
        }

        System.Array.Sort(
            hits,
            (a, b) => a.distance.CompareTo(b.distance)
        );

        foreach (RaycastHit hit in hits)
        {
            Targetable target =
                hit.collider.GetComponentInParent<Targetable>();

            if (target != null)
            {
                SetTarget(target);
                return;
            }
        }

        ClearTarget();
    }

    public void SetTarget(Targetable target)
    {
        if (target == null)
            return;

        if (currentTarget != null)
            currentTarget.SetSelected(false);

        currentTarget = target;
        currentTarget.SetSelected(true);

        if (TargetManager.Instance != null)
            TargetManager.Instance.SetTarget(currentTarget.gameObject);
    }

    public void ClearTarget()
    {
        if (currentTarget != null)
            currentTarget.SetSelected(false);

        currentTarget = null;

        if (TargetManager.Instance != null)
            TargetManager.Instance.ClearTarget();
    }
}