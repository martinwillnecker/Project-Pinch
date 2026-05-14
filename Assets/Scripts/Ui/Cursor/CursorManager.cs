using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Cursor Textures")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D draggingItemCursor;
    [SerializeField] private Texture2D draggingSpellCursor;
    [SerializeField] private Texture2D spellSelectedCursor;

    [Header("Hotspots")]
    [SerializeField] private Vector2 hotspot = new Vector2(8, 8);

    private GameCursorState currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetCursor(GameCursorState.Default);
    }

    public void SetCursor(GameCursorState state)
    {
        currentState = state;

        Texture2D selectedCursor = null;

        switch (state)
        {
            case GameCursorState.Default:
                selectedCursor = defaultCursor;
                break;

            case GameCursorState.DraggingItem:
                selectedCursor = draggingItemCursor;
                break;

            case GameCursorState.DraggingSpell:
                selectedCursor = draggingSpellCursor;
                break;

            case GameCursorState.SpellSelected:
                selectedCursor = spellSelectedCursor;
                break;
        }

        if (selectedCursor == null)
        {
            Debug.LogWarning($"Cursor NULL for state: {state}");
            return;
        }

        Cursor.SetCursor(
            selectedCursor,
            hotspot,
            CursorMode.Auto
        );

        Debug.Log($"Cursor changed to: {state}");
    }

    public void ResetCursor()
    {
        SetCursor(GameCursorState.Default);
    }
}