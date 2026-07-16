using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;
using System.Reflection.Metadata;
using Unity.Cinemachine;
using Unity.VisualScripting;

public class CoreInventoryController : MonoBehaviour
{
    [SerializeField] private GameObject Toolbar;
    [SerializeField] private GameObject MainInventory;
    [SerializeField] private TMP_Text cursorText;

    [SerializeField] private Transform handAnchor;

    [SerializeField] private Sprite placeholderImage;

    private GameObject currentHandModel;

    public static CoreInventoryController instance; //singleton 
    private bool isMainInventoryOpen = false;

    private InputSystem_Actions inputActions;

    private int selectedSlot = -1;
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
            slotUI.outline = img.GetComponentInChildren<UnityEngine.UI.Outline>();
            itemUIList.Add(slotUI);
        }

        //get all the images in the main inventory and add them to the itemUIList of MainInventory
        Image[] MainInventoryImages = MainInventory.GetComponentsInChildren<Image>();

        foreach (Image img in MainInventoryImages)
        {
            InventorySlotUI slotUI = new InventorySlotUI();
            slotUI.inventorySlotImage = img;
            slotUI.amountText = img.GetComponentInChildren<TextMeshProUGUI>();
            slotUI.outline = img.GetComponentInChildren<UnityEngine.UI.Outline>();
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
            isMainInventoryOpen = !isMainInventoryOpen; 

            MainInventory.SetActive(isMainInventoryOpen);

            if (isMainInventoryOpen)
            {
                PlayerMovement pm = Object.FindAnyObjectByType<PlayerMovement>();
                pm.FreezeInput();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                PlayerMovement pm = Object.FindAnyObjectByType<PlayerMovement>();
                pm.UnfreezeInput();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.SelectItem.performed += OnSelectSlot;

    }

    public void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.SelectItem.performed -= OnSelectSlot;
    }

    private void OnSelectSlot(InputAction.CallbackContext context)
    {
        string inputName = context.control.name;
        int inputNameInt = int.Parse(inputName); //convert "1" to 1
        inputNameInt--; //make int be 0 based index

        selectedSlot = inputNameInt;

        UpdateHandDisplay();

        UpdateSelectionHightlight();
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

    public void UpdateHandDisplay()
    {
        if (currentHandModel != null)
        {
            Destroy(currentHandModel);
        }
        if (selectedSlot >= 0 && selectedSlot < itemList.Count)
        {
            if (itemList[selectedSlot].item.handModel != null)
            {
                currentHandModel = Instantiate(itemList[selectedSlot].item.handModel, handAnchor);
                currentHandModel.transform.localPosition = Vector3.zero;
                currentHandModel.transform.localRotation = Quaternion.identity;
            }
        }

    }

    public void UpdateSelectionHightlight()
    {
        for(int i = 0; i < toolbarSlots + mainInventroySlots; i++)
        {
            if(i == selectedSlot)
            {
                itemUIList[i].outline.effectDistance = new Vector2(4f, 4f); // thicker
                itemUIList[i].inventorySlotImage.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            }

            else
            {
                itemUIList[i].outline.effectDistance = new Vector2(2f, 2f); // baseline thickness
                itemUIList[i].inventorySlotImage.color = Color.white;
            }
        }
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
        UpdateHandDisplay();
        UpdateSelectionHightlight();
    }

}
