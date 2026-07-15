using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Reflection.Metadata;

public class CoreInventoryController : MonoBehaviour
{
    [SerializeField] private GameObject Toolbar;
    [SerializeField] private GameObject MainInventory;
    [SerializeField] private TMP_Text cursorText;

    [SerializeField] private Sprite placeholderImage;

    public static CoreInventoryController instance; //singleton 

    private UnityEvent onTabPressed; //for opening main inventory
    private InputSystem_Actions inputActions;


    private int toolbarSlots = 7;
    private int mainInventroySlots = 7;

    private List<InventorySlot> itemList = new List<InventorySlot>();
    private List<InventorySlotUI> itemUIList = new List<InventorySlotUI>();

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        instance = this;

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

    public void Start()
    {
        UpdateInventoryUI();
    }

    public void Update()
    {
        if (inputActions.Player.OpenInventory.WasPressedThisFrame())
        {
            onTabPressed.Invoke();
        }
    }

    public void OnEnable()
    {
        inputActions.Player.Enable();

    }

    public void OnDisable()
    {
        inputActions.Player.Disable();
    }


    public void AddItem(ItemScriptableObject item, int quantity)
    {
        int remaining = quantity; //how much of the item we still need to add to the inventory

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].item == item && itemList[i].quantity < item.maxStackSize)
            {
                int room = item.maxStackSize - itemList[i].quantity;
                int amountToAdd = Mathf.Min(room, remaining); //return which one is smaller
                itemList[i].quantity += amountToAdd;
                remaining -= amountToAdd;
                if(remaining <= 0)
                {
                    UpdateInventoryUI();
                    return;
                }
            }
        }
        while (remaining > 0)
        {
            if(itemList.Count >= 14)
            {
                NotificationController.instance.ShowNotification("Inventory Full!");
                break;
            }

            InventorySlot newSlot = new InventorySlot();
            newSlot.item = item;
            newSlot.quantity = Mathf.Min(remaining, item.maxStackSize); //for items that come in batch(e.g coins, 3)
            itemList.Add(newSlot);
            remaining -= newSlot.quantity;
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
