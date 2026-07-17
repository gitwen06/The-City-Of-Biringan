using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int slotIndex;

    private static int draggedFromIndex = -1;
    private static Image ghostImage;

    public static void SetGhostImage(Image img)
    {
        ghostImage = img;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggedFromIndex == -1) { return; }
        CoreInventoryController.instance.MoveItem(draggedFromIndex, slotIndex);
        draggedFromIndex = -1;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ItemScriptableObject item = CoreInventoryController.instance.GetItemAtSlot(slotIndex);

        if (item == null)
        {
            draggedFromIndex = -1; // nothing to drag, cancel
            return;
        }

        draggedFromIndex = slotIndex;
        ghostImage.gameObject.SetActive(true);
        ghostImage.sprite = item.Icon;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedFromIndex == -1) { return; } // nothing being dragged, ignore

        ghostImage.rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ghostImage.gameObject.SetActive(false);
        
    }


}
