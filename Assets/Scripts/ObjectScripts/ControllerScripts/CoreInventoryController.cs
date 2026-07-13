using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Reflection.Metadata;

public class CoreInventoryController : MonoBehaviour
{
    [SerializeField] private GameObject Toolbar;
    [SerializeField] private GameObject MainInventory;

    [SerializeField] private Sprite placeholderImage;

    private int toolbarSlots = 7;
    private int mainInventroySlots = 7;

    private List<InventorySlot> itemList = new List<InventorySlot>();
    private List<InventorySlotUI> itemUIList = new List<InventorySlotUI>();

    private void Awake()
    {
        //get all the images in the toolbar and main inventory and add them to the itemUIList of toolbar
        Image[] toolbarImages = Toolbar.GetComponentsInChildren<Image>();

        foreach (Image img in toolbarImages)
        {
            InventorySlotUI slotUI = new InventorySlotUI();
            slotUI.inventorySlotImage = img;
            slotUI.amountText = img.GetComponentInChildren<TextMeshProUGUI>();
            itemUIList.Add(slotUI);
        }

        //get all the images in the main inventory and add them to the itemUIList of MainInventory
        Image[] MainInventoryImages = MainInventory.GetComponentsInChildren<Image>();

        foreach (Image img in MainInventoryImages)
        {
            InventorySlotUI slotUI = new InventorySlotUI();
            slotUI.inventorySlotImage = img;
            slotUI.amountText = img.GetComponentInChildren<TextMeshProUGUI>();
            itemUIList.Add(slotUI);
        }

    }

    public void AddItem(ItemScriptableObject item, int quantity)
    {
        // Check if the item already exists in the inventory
        InventorySlot existingSlot = itemList.Find(slot => slot.item == item);
        if (existingSlot != null)
        {
            // If the item exists, increase the quantity
            existingSlot.quantity += quantity;
        }
        else
        {
            // If the item doesn't exist, create a new slot and add it to the inventory
            InventorySlot newSlot = new InventorySlot();
            newSlot.item = item;
            newSlot.quantity = quantity;
            itemList.Add(newSlot);
        }
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        for(int i = 0; i < toolbarSlots + mainInventroySlots; i++)
        {
            if (i < itemList.Count)
            {
                itemUIList[i].inventorySlotImage.sprite = itemList[i].item.Icon;
                itemUIList[i].amountText.text = itemList[i].quantity.ToString();
            }
            else
            {
                itemUIList[i].inventorySlotImage.sprite = placeholderImage;
                itemUIList[i].amountText.text = "";
            }
        }

    }

}
