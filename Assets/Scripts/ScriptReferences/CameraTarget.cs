using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public string targetId;
    void Start()
    {
        CameraTargetRegistry.instance.RegisterTarget(targetId, this.transform);       
    }
}
