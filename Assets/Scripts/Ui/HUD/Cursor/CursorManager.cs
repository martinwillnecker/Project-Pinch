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

    private void Update()
    {
        // DEBUG TEMPORAL

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetCursor(GameCursorState.Default);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetCursor(GameCursorState.DraggingItem);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SetCursor(GameCursorState.DraggingSpell);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SetCursor(GameCursorState.SpellSelected);
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
            CursorMode.ForceSoftware
        );

        Debug.Log($"Cursor changed to: {state}");
    }

    public void ResetCursor()
    {
        SetCursor(GameCursorState.Default);
    }
}