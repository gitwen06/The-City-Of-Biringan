using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class DialogueNode
{
    public string speakerName;
    public string text;
    public List<DialogueOption> options;
    public UnityEvent eventAction;

    public CameraEventType cameraEvent;
    public string cameraTargetId;

    public enum CameraEventType
    {
        None,
        LookAt,
        MoveTo,
        ResetAnimated,
        ResetStatic
    }
}

