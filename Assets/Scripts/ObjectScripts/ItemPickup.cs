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
    }

    public void Interact()
    {
        if (!pickedUp)
        {
            inventoryController.AddItem(item, 1);
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
