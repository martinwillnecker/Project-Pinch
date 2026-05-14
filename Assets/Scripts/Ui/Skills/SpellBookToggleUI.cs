using UnityEngine;

public class SpellBookToggleUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject spellBookWindow;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.K;

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleSpellBook();
        }
    }

    private void ToggleSpellBook()
    {
        if (spellBookWindow == null)
        {
            Debug.LogWarning("SpellBookWindow no está asignado.");
            return;
        }

        spellBookWindow.SetActive(!spellBookWindow.activeSelf);
    }
}