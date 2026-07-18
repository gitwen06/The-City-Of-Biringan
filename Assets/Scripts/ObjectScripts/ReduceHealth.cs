using UnityEngine;

public class ReduceHealth : MonoBehaviour
{

    public void OnTriggerStay(Collider other)
    {
        PlayerHealth.Instance.ReduceHP(1f);
    }
}
