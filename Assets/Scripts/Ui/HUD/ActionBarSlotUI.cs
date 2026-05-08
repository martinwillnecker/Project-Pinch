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
            highlightBorder.gameObject.SetActive(false);
            highlightBorder.raycastTarget = false;
            highlightBorder.transform.SetAsLastSibling();
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
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = 0f;

            if (cooldownText != null)
                cooldownText.text = "";

            return;
        }

        if (cooldownManager == null)
            cooldownManager = FindFirstObjectByType<SkillCooldownManager>();

        if (cooldownManager == null)
            return;

        float remaining = cooldownManager.GetSkillCooldownRemaining(assignedSkill);
        float fill = cooldownManager.GetSkillCooldownPercent(assignedSkill);

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = fill;

        if (cooldownText != null)
            cooldownText.text = remaining > 0f
                ? Mathf.CeilToInt(remaining).ToString()
                : "";
    }

    private void UpdateHighlight()
    {
        if (highlightBorder == null)
            return;

        bool isSelected =
            SkillExecutor.Instance != null &&
            SkillExecutor.Instance.IsAiming &&
            SkillExecutor.Instance.PendingSkill == assignedSkill;

        highlightBorder.gameObject.SetActive(isSelected);

        if (isSelected)
            highlightBorder.transform.SetAsLastSibling();
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
        PlayerPrefs.SetString($"ActionBarSlot_{slotIndex}_Keybind", assignedKey.ToString());
        PlayerPrefs.Save();
    }

    private void LoadKeybind()
    {
        string defaultKey = GetDefaultKeyForSlot(slotIndex).ToString();
        string savedKey = PlayerPrefs.GetString($"ActionBarSlot_{slotIndex}_Keybind", defaultKey);

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

        Debug.Log($"Skill {assignedSkill.skillName} asignada al slot {slotIndex}");
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