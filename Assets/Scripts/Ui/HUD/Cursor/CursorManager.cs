using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Cursor Textures")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D draggingCursor;
    [SerializeField] private Texture2D spellSelectedCursor;

    [Header("Hotspots")]
    [SerializeField] private Vector2 defaultHotspot = Vector2.zero;
    [SerializeField] private Vector2 draggingHotspot = Vector2.zero;
    [SerializeField] private Vector2 spellSelectedHotspot = Vector2.zero;

    private GameCursorState currentState = GameCursorState.Default;

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

        switch (state)
        {
            case GameCursorState.Default:
                ApplyCursor(defaultCursor, defaultHotspot);
                break;

            case GameCursorState.Dragging:
                ApplyCursor(draggingCursor, draggingHotspot);
                break;

            case GameCursorState.SpellSelected:
                ApplyCursor(spellSelectedCursor, spellSelectedHotspot);
                break;
        }
    }

    private void ApplyCursor(Texture2D cursorTexture, Vector2 hotspot)
    {
        if (cursorTexture == null)
        {
            Debug.LogWarning($"Cursor texture missing for state: {currentState}");
            return;
        }

        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    public void ResetCursor()
    {
        SetCursor(GameCursorState.Default);
    }

    public GameCursorState GetCurrentState()
    {
        return currentState;
    }
}