using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance;

    public GameObject CurrentTarget { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetTarget(GameObject target)
    {
        CurrentTarget = target;
    }

    public void ClearTarget()
    {
        CurrentTarget = null;
    }
}