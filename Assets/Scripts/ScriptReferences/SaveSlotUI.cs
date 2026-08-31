using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System;
using System.Timers;

public class SaveSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TMP_Text saveName;
    [SerializeField] private TMP_Text saveQuest;
    [SerializeField] private TMP_Text saveDate;
    [SerializeField] private TMP_Text saveGenArea;
    [SerializeField] private TMP_Text savedBy;
    [SerializeField] private UnityEngine.UI.Outline outline;

    private float hoverScale = 1.2f;
    private float hoverYOffset = 10f;
    private float animDuration = 0.25f;

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector2 originalPosition;
    private Coroutine hoverCoroutine;

    private int slotIndex;
    private SavesController controller;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void Setup(SaveSlot saveSlot, int index, SavesController savesController)
    {
        Debug.Log("setting up");
        saveName.text = saveSlot.saveName;
        saveQuest.text = saveSlot.questName;
        saveDate.text = saveSlot.saveDate;
        saveGenArea.text = saveSlot.generalArea;
        savedBy.text = saveSlot.isAutosave ? "Autosave" : "Manual Save";

        slotIndex = index;
        controller = savesController;

        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;
    }

    public int GetSlotIndex()
    {
        return slotIndex;
    }

    IEnumerator HoverAnim(bool isHovering)
    {
        Vector3 startScale = rectTransform.localScale;
        Vector2 startPos = rectTransform.anchoredPosition;

        Vector3 targetScale = isHovering ? originalScale * hoverScale : originalScale;
        Vector2 targetPos = isHovering ? originalPosition + new Vector2(0, hoverYOffset) : originalPosition;

        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            float t = elapsed / animDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, easedT);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, easedT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.localScale = targetScale;
        rectTransform.anchoredPosition = targetPos;
    }

    public void UpdateIndex(int newIndex)
    {
        slotIndex = newIndex;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine); // if coroutine exist stop
        }
        hoverCoroutine = StartCoroutine(HoverAnim(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine); // if coroutine exist stop
        }
        hoverCoroutine = StartCoroutine(HoverAnim(false));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controller == null) { return; }
        controller.SelectSlot(slotIndex);
    }

    public void Select()
    {
        if (outline != null) outline.enabled = true;
    }

    public void Deselect()
    {
        if (outline != null) outline.enabled = false;
    }
}