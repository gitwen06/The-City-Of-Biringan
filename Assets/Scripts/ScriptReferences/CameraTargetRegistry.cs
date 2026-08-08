using UnityEngine;
using System.Collections.Generic;

public class CameraTargetRegistry : MonoBehaviour
{
    //camera lookAt and moveAt registry since scriptableObjects doesnt allow gameobjects as input.
    public static CameraTargetRegistry instance;

    private Dictionary<string, Transform> target = new Dictionary<string, Transform>();
    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void RegisterTarget(string id, Transform t)
    {
        target[id] = t;
    }

    public Transform GetTarget(string id)
    {
        Transform foundTarget;
        bool wasFound = target.TryGetValue(id, out foundTarget);

        return wasFound ? foundTarget : null;
    }
}
