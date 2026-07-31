using UnityEngine;
using System.Collections.Generic;

public class CameraTargetRegistry : MonoBehaviour
{
    //camera lookAt and moveAt registry since scriptableObjects doesnt allow gameobjects as input.
    public static CameraTargetRegistry instance;

    public Dictionary<string, Transform> target = new Dictionary<string, Transform>();

    public void Awake()
    {
        instance = this;
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
