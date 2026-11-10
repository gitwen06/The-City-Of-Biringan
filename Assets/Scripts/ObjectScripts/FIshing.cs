using UnityEngine;

public class Fishing : MonoBehaviour
{
    [SerializeField] private Transform RodPos;

    private bool isIdle;
    private bool isWaiting;
    private bool isReeling;
    private bool isFishCaught;

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        //hiatus
    }
}
