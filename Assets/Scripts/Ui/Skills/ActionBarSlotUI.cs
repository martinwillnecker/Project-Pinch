using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ActionBarSlotUI : MonoBehaviour, IDropHandler
{
    [Header("Skill")]
    [SerializeField] private SkillData assignedSkill;

    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TMP_Text keybindText;
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private Image highlightBorder;

    [Header("Config")]
    [SerializeField] private int slotIndex;

    private KeyCode assignedKey;
    private bool isListeningForKey = false;

    private SkillCooldownManager cooldownManager;

    private bool lastHighlightState = false;
    private float lastCooldownFill = -1f;
    private string lastCooldownText = "";

    private void Start()
    {
        cooldownManager = FindFirstObjectByType<SkillCooldownManager>();

        LoadKeybind();
        UpdateKeybindText();

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0f;

        if (cooldownText != null)
            cooldownText.text = "";

        if (highlightBorder != null)
        {
            Color color = highlightBorder.color;
            color.a = 0f;
            highlightBorder.color = color;
            highlightBorder.raycastTarget = false;
        }

        RefreshIcon();
    }

    private void Update()
    {
        HandleInput();
        HandleKeybindRebind();
        UpdateCooldown();
        UpdateHighlight();
    }

    private void HandleInput()
    {
        if (isListeningForKey)
            return;

        if (Input.GetKeyDown(assignedKey))
            TryUseAbility();
    }

    private void TryUseAbility()
    {
        if (assignedSkill == null)
        {
            Debug.Log($"Slot {slotIndex} vacío.");
            return;
        }

        if (SkillExecutor.Instance == null)
        {
            Debug.LogWarning("No existe SkillExecutor en la escena.");
            return;
        }

        SkillExecutor.Instance.TryCast(assignedSkill);
    }

    private void UpdateCooldown()
    {
        if (assignedSkill == null)
        {
            SetCooldownVisual(0f, "");
            return;
        }

        if (cooldownManager == null)
            return;

        float remaining =
            cooldownManager.GetSkillCooldownRemaining(assignedSkill);

        float fill =
            cooldownManager.GetSkillCooldownPercent(assignedSkill);

        string text =
            remaining > 0f
            ? Mathf.CeilToInt(remaining).ToString()
            : "";

        SetCooldownVisual(fill, text);
    }

    private void SetCooldownVisual(float fill, string text)
    {
        if (cooldownOverlay != null)
        {
            if (!Mathf.Approximately(lastCooldownFill, fill))
            {
                cooldownOverlay.fillAmount = fill;
                lastCooldownFill = fill;
            }
        }

        if (cooldownText != null)
        {
            if (lastCooldownText != text)
            {
                cooldownText.text = text;
                lastCooldownText = text;
            }
        }
    }

    private void UpdateHighlight()
    {
        if (highlightBorder == null)
            return;

        bool isSelected =
            SkillExecutor.Instance != null &&
            SkillExecutor.Instance.IsAiming &&
            SkillExecutor.Instance.PendingSkill == assignedSkill;

        if (isSelected == lastHighlightState)
            return;

        Color color = highlightBorder.color;
        color.a = isSelected ? 0.8f : 0f;
        highlightBorder.color = color;

        lastHighlightState = isSelected;
    }

    public void StartListeningForKey()
    {
        isListeningForKey = true;

        if (keybindText != null)
            keybindText.text = "...";
    }

    private void HandleKeybindRebind()
    {
        if (!isListeningForKey)
            return;

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                assignedKey = keyCode;
                SaveKeybind();
                UpdateKeybindText();
                isListeningForKey = false;
                break;
            }
        }
    }

    private void SaveKeybind()
    {
        PlayerPrefs.SetString(
            $"ActionBarSlot_{slotIndex}_Keybind",
            assignedKey.ToString()
        );

        PlayerPrefs.Save();
    }

    private void LoadKeybind()
    {
        string defaultKey =
            GetDefaultKeyForSlot(slotIndex).ToString();

        string savedKey =
            PlayerPrefs.GetString(
                $"ActionBarSlot_{slotIndex}_Keybind",
                defaultKey
            );

        if (System.Enum.TryParse(savedKey, out KeyCode loadedKey))
            assignedKey = loadedKey;
        else
            assignedKey = GetDefaultKeyForSlot(slotIndex);
    }

    private KeyCode GetDefaultKeyForSlot(int index)
    {
        return index switch
        {
            1 => KeyCode.Alpha1,
            2 => KeyCode.Alpha2,
            3 => KeyCode.Alpha3,
            4 => KeyCode.Alpha4,
            5 => KeyCode.Alpha5,
            6 => KeyCode.Alpha6,
            7 => KeyCode.Alpha7,
            8 => KeyCode.Alpha8,
            9 => KeyCode.Alpha9,
            _ => KeyCode.None
        };
    }

    private void UpdateKeybindText()
    {
        if (keybindText != null)
            keybindText.text = FormatKeyName(assignedKey);
    }

    private string FormatKeyName(KeyCode key)
    {
        return key.ToString()
            .Replace("Alpha", "")
            .Replace("Mouse", "M");
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (SkillDragManager.DraggedSkill == null)
            return;

        assignedSkill = SkillDragManager.DraggedSkill;

        RefreshIcon();
    }

    private void RefreshIcon()
    {
        if (iconImage == null)
            return;

        iconImage.enabled = true;

        if (assignedSkill == null || assignedSkill.icon == null)
        {
            iconImage.sprite = null;
            iconImage.color = new Color(1f, 1f, 1f, 0.08f);
            return;
        }

        iconImage.sprite = assignedSkill.icon;
        iconImage.color = Color.white;
    }
}