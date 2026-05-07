using UnityEngine;

public class Targetable : MonoBehaviour
{
    public string targetName = "Enemy";
    public bool IsSelected { get; private set; }

    private Renderer objectRenderer;
    private Color originalColor;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        originalColor = objectRenderer.material.color;
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;

        if (objectRenderer != null)
        {
            objectRenderer.material.color = selected ? Color.yellow : originalColor;
        }
    }
}