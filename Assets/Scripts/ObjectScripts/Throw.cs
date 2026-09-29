using UnityEngine;

public class Throw : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private Transform attackPoint;

    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float throwUpwardForce = 2f;
    [SerializeField] private float throwCooldown = 0.5f;
    [SerializeField] private float spawnLiftOffset = 0.1f;

    private InputSystem_Actions inputActions;
    private float cooldownTimer = 0f;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();

        if (cam == null) Debug.LogError("Throw: 'cam' is not assigned in the Inspector.");
        if (attackPoint == null) Debug.LogError("Throw: 'attackPoint' is not assigned in the Inspector.");
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (inputActions.Player.Throw.triggered)
        {
            Debug.Log($"Throw: G pressed. cooldownTimer={cooldownTimer}");

            if (cooldownTimer <= 0f)
            {
                TryThrow();
            }
            else
            {
                Debug.Log("Throw: still on cooldown, ignoring press.");
            }
        }
    }

    private void TryThrow()
    {
        Debug.Log("Throw: TryThrow() started.");

        if (attackPoint == null)
        {
            Debug.LogError("Throw: attackPoint is null — aborting throw.");
            return;
        }

        ItemScriptableObject item = CoreInventoryController.instance.GetSelectedItem();
        Debug.Log($"Throw: selected item = {(item == null ? "NULL" : item.itemName)}");

        if (item == null)
        {
            Debug.Log("Throw: no item selected, aborting.");
            return;
        }

        if (item.worldModel == null)
        {
            Debug.LogWarning($"Throw: {item.itemName} has no worldModel assigned, aborting.");
            return;
        }

        CoreInventoryController.instance.RemoveOneFromSelectedSlot();
        Debug.Log("Throw: removed one from inventory.");

        Vector3 spawnPos = attackPoint.position + Vector3.up * spawnLiftOffset;
        GameObject projectile = Instantiate(item.worldModel, spawnPos, cam.rotation);
        Debug.Log($"Throw: instantiated '{projectile.name}' at {spawnPos}");

        ItemPickup pickup = projectile.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            pickup.isDynamicInstance = true;
            Debug.Log("Throw: set isDynamicInstance = true on spawned ItemPickup.");
        }
        else
        {
            Debug.Log("Throw: spawned object has no ItemPickup component.");
        }

        MeshRenderer mr = projectile.GetComponent<MeshRenderer>();
        Debug.Log($"Throw: spawned object MeshRenderer enabled = {(mr != null ? mr.enabled.ToString() : "NO MESHRENDERER FOUND")}");

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb == null)
        {
            projectileRb = projectile.AddComponent<Rigidbody>();
            Debug.Log("Throw: added Rigidbody at runtime.");
        }

        Vector3 forceDirection = cam.forward;
        Vector3 upwardForce = Vector3.up * throwUpwardForce;
        Vector3 totalForce = (forceDirection + upwardForce).normalized * throwForce;
        projectileRb.AddForce(totalForce, ForceMode.Impulse);
        Debug.Log($"Throw: applied force {totalForce}");

        cooldownTimer = throwCooldown;
    }
}