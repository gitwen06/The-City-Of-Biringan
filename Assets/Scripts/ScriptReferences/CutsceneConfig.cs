using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class CutsceneConfig
{
    public string cutsceneName;
    public PlayableDirector director;
    public Transform trackTarget;    
    public Transform DollyAtTarget;   
}
