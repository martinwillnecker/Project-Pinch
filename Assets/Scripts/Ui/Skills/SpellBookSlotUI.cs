using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SpellBookSlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text skillNameText;

    [Header("Data")]
    [SerializeField] private SkillData assignedSkill;

    private void Start()
    {
        Refresh();
    }

    public void SetSkill(SkillData skill)
    {
        assignedSkill = skill;
        Refresh();
    }

    private void Refresh()
    {
        if (assignedSkill == null)
        {
            if (iconImage != null) iconImage.enabled = false;
            if (skillNameText != null) skillNameText.text = "";
            return;
        }

        if (iconImage != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = assignedSkill.icon;
        }

        if (skillNameText != null)
            skillNameText.text = assignedSkill.skillName;
    }

    public SkillData GetSkill()
    {
        return assignedSkill;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (assignedSkill == null) return;

        SkillDragManager.DraggedSkill = assignedSkill;
        Debug.Log($"Dragging skill: {assignedSkill.skillName}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Después agregamos ícono flotante siguiendo el mouse.
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SkillDragManager.DraggedSkill = null;
    }
}