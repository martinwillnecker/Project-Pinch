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

    [Header("Config")]
    [SerializeField] private int slotIndex;

    private KeyCode assignedKey;
    private bool isListeningForKey = false;

    private float cooldownDuration = 0f;
    private float cooldownRemaining = 0f;

    private void Start()
    {
        LoadKeybind();
        UpdateKeybindText();

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0f;

        if (cooldownText != null)
            cooldownText.text = "";
    }

    private void Update()
    {
        HandleInput();
        HandleKeybindRebind();
        UpdateCooldown();
    }

    private void HandleInput()
    {
        if (isListeningForKey) return;

        if (Input.GetKeyDown(assignedKey))
        {
            TryUseAbility();
        }
    }

    private void TryUseAbility()
    {
        if (assignedSkill == null)
        {
            Debug.Log($"Slot {slotIndex} vacío.");
            return;
        }

        if (cooldownRemaining > 0f)
            return;

        SkillExecutor.Instance.TryCast(assignedSkill);

        StartCooldown(assignedSkill.cooldown);
    }

    private void StartCooldown(float duration)
    {
        cooldownDuration = duration;
        cooldownRemaining = duration;
    }

    private void UpdateCooldown()
    {
        if (cooldownRemaining <= 0f)
        {
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = 0f;

            if (cooldownText != null)
                cooldownText.text = "";

            return;
        }

        cooldownRemaining -= Time.deltaTime;

        float fill = cooldownRemaining / cooldownDuration;

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = fill;

        if (cooldownText != null)
            cooldownText.text = Mathf.CeilToInt(cooldownRemaining).ToString();
    }

    public void StartListeningForKey()
    {
        isListeningForKey = true;

        if (keybindText != null)
            keybindText.text = "...";
    }

    private void HandleKeybindRebind()
    {
        if (!isListeningForKey) return;

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
        if (SkillDragManager.DraggedSkill == null) return;

        assignedSkill = SkillDragManager.DraggedSkill;

        if (iconImage != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = assignedSkill.icon;
        }

        Debug.Log($"Skill {assignedSkill.skillName} asignada al slot {slotIndex}");
    }
}