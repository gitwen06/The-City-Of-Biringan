using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class CoreInventoryController : MonoBehaviour
{
    [SerializeField] private GameObject Toolbar;
    [SerializeField] private GameObject MainInventory;
    [SerializeField] private TMP_Text cursorText;

    [SerializeField] private Image ghostImageSource;

    [SerializeField] private Transform handAnchor;

    [SerializeField] private Sprite placeholderImage;

    private GameObject currentHandModel;

    public static CoreInventoryController instance; //singleton 
    private bool isMainInventoryOpen = false;

    private InputSystem_Actions inputActions;

    private int selectedSlot = -1; // -1 at instantiation so inventory doesnt highlight any indexes
    private int toolbarSlots = 7;
    private int mainInventroySlots = 7;

    private List<InventorySlot> itemList = Enumerable.Repeat<InventorySlot>(null, 14).ToList(); // make the list full of null entries for dragging system
    private List<InventorySlotUI> itemUIList = new List<InventorySlotUI>(14);

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        instance = this;

        //get all the images in the toolbar and main inventory and add them to the itemUIList of toolbar
        Image[] toolbarImages = Toolbar.GetComponentsInChildren<Image>();

        for(int i = 0; i <  toolbarImages.Length; i++)
        {
            InventorySlotUI slotUI = new InventorySlotUI();
            slotUI.inventorySlotImage = toolbarImages[i];
            slotUI.amountText = toolbarImages[i].GetComponentInChildren<TextMeshProUGUI>();
            slotUI.outline = toolbarImages[i].GetComponentInChildren<UnityEngine.UI.Outline>();
            DraggableSlot dragSlot = toolbarImages[i].GetComponent<DraggableSlot>();
            dragSlot.slotIndex = i;
            itemUIList.Add(slotUI);
        }

        //get all the images in the main inventory and add them to the itemUIList of MainInventory
        Image[] MainInventoryImages = MainInventory.GetComponentsInChildren<Image>();

        for(int i = 0; i <  MainInventoryImages.Length; i++)
        {
            InventorySlotUI slotUI = new InventorySlotUI();
            slotUI.inventorySlotImage = MainInventoryImages[i];
            slotUI.amountText = MainInventoryImages[i].GetComponentInChildren<TextMeshProUGUI>();
            slotUI.outline = MainInventoryImages[i].GetComponentInChildren<UnityEngine.UI.Outline>();
            DraggableSlot dragSlot = MainInventoryImages[i].GetComponent<DraggableSlot>();
            dragSlot.slotIndex = toolbarSlots + i;
            itemUIList.Add(slotUI);
        }

        DraggableSlot.SetGhostImage(ghostImageSource);

    }

    public void Start()
    {
        UpdateInventoryUI();
    }

    public void Update()
    {
        if (inputActions.Player.OpenInventory.WasPressedThisFrame() && !NoteController.instance.isreading)
        {
            Debug.Log(NoteController.instance.isreading);
            isMainInventoryOpen = !isMainInventoryOpen;
            Debug.Log($"Tab pressed, isMainInventoryOpen: {isMainInventoryOpen}");

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
        int counter = 0;
        int remaining = quantity; //how much of the item we still need to add to the inventory

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] != null && itemList[i].item == item && itemList[i].quantity < item.maxStackSize)
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
            if (counter >= 14)
            {
                NotificationController.instance.ShowNotification("Inventory Full!");
                break;
            }

            if (itemList[counter] == null)
            {
                InventorySlot newSlot = new InventorySlot();
                newSlot.item = item;
                newSlot.quantity = Mathf.Min(remaining, item.maxStackSize);
                itemList[counter] = newSlot;
                remaining -= newSlot.quantity;
            }

            counter++;
        }
        UpdateInventoryUI();
    }

    public void MoveItem(int FromIndex, int ToIndex)
    {
        if (itemList[FromIndex] == null) { return; }
        if (FromIndex == ToIndex) { return; }

        //if item slot is null go there
        if (itemList[ToIndex] == null)
        {
            //instance item in toindex
            //destroy fromindex item
            itemList[ToIndex] = itemList[FromIndex];
            itemList[FromIndex] = null;

        }
        //if item type is the same type
        else if (itemList[FromIndex].item == itemList[ToIndex].item)
        {
            if (itemList[ToIndex].quantity >= itemList[ToIndex].item.maxStackSize)
            {
                return; // target already completely full, do nothing
            }

            AddItem(itemList[FromIndex].item, itemList[FromIndex].quantity);
            itemList[FromIndex] = null;
        }
        //if not same type do nithign
        else { return; }

        UpdateInventoryUI(); //updateinventory ui since only additem calls it for 1 instance.

    }

    public void UseSelectedItem()
    {
        if(selectedSlot >= 0 && selectedSlot < itemList.Count && itemList[selectedSlot] != null)
        {
            itemList[selectedSlot].item.Use();
        }

    }

    public void UpdateHandDisplay()
    {
        if (currentHandModel != null)
        {
            Destroy(currentHandModel);
        }
        if (selectedSlot >= 0 && selectedSlot < itemList.Count)
        {
            if (itemList[selectedSlot] != null && itemList[selectedSlot].item.handModel != null)
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
            if (itemList[i] != null)
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


    //called by draggable slot(for reference)
    public ItemScriptableObject GetItemAtSlot(int slot)
    {
        if(itemList[slot] != null)
        {
            return itemList[slot].item;
        }
        else
        {
            return null;
        }
    }
}
