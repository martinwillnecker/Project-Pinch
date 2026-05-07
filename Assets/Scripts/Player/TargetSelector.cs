using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    [SerializeField] private Targetable currentTarget;

    public Targetable CurrentTarget => currentTarget;

    public Camera mainCamera;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectTarget();
        }
    }

    private void SelectTarget()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Targetable target = hit.collider.GetComponent<Targetable>();

            if (target != null)
            {
                SetTarget(target);
            }
            else
            {
                ClearTarget();
            }
        }
        else
        {
            ClearTarget();
        }
    }

    private void SetTarget(Targetable target)
    {
        if (currentTarget != null)
        {
            currentTarget.SetSelected(false);
        }

        currentTarget = target;
        currentTarget.SetSelected(true);

        TargetManager.Instance.SetTarget(currentTarget.gameObject);
    }

    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.SetSelected(false);
        }

        currentTarget = null;

        TargetManager.Instance.ClearTarget();
    }
}