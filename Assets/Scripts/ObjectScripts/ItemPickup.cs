using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ItemPickup : MonoBehaviour, Iinteractable
{
    [SerializeField] private ItemScriptableObject item;
    private CoreInventoryController inventoryController;

    private bool pickedUp = false;
    private MeshRenderer thisObject;
    private Collider thisObjectCollider;

    Outline outline;

    public void Start()
    {
        inventoryController = CoreInventoryController.instance;
        outline = GetComponent<Outline>();
        outline.enabled = false;
        thisObject = GetComponent<MeshRenderer>();
        thisObjectCollider = GetComponent<Collider>();

        if (GameFlags.instance.GetFlag(item.itemName + "_pickedup"))
        {
            pickedUp = true;
            thisObject.enabled = false;
            thisObjectCollider.enabled = false;
            Debug.Log("Item already picked up: " + item.itemName);
        }

        Debug.Log("Instantiated item: " + item.itemName);
    }

    public void Interact()
    {
        if (!pickedUp)
        {
            inventoryController.AddItem(item, 1);
            GameFlags.instance.SetFlag(item.itemName + "_pickedup", true);
            pickedUp = true;
            thisObject.enabled = false;
            thisObjectCollider.enabled = false;
            StartCoroutine(OnObjectDestroy());
        }
    }

    IEnumerator OnObjectDestroy()
    {
        //playsound
        //playparticle
        yield return new WaitForSeconds(2f);
        //destroy
        Destroy(gameObject);
        yield return null;
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }
}
