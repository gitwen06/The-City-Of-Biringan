using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public TriggerController instance;

    private void Awake()
    {
        instance = this;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name);
    }
}
