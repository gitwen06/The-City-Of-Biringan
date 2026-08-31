using UnityEngine;

public class ReduceHealth : MonoBehaviour
{

    public void OnTriggerStay(Collider other)
    {
        PlayerHealth.instance.ReduceHP(1f);
    }
}
