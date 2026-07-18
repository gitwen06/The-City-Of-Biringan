using UnityEngine;

public class IncreaseHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnTriggerStay(Collider other)
    {
        PlayerHealth.Instance.HealHP(1f);
    }
}
