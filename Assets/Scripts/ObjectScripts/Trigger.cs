using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class Trigger : MonoBehaviour
{
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    [SerializeField] private PlayableDirector cutscene;
    private bool isPlayed;
    public Outline outline;

    private void Start()
    {
        outline.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name);
        if (other.CompareTag("Player"))
        {
            onTriggerEnter.Invoke();
            if (cutscene != null && !isPlayed)
            {
                CutsceneRunner.instance.Play(cutscene);
                isPlayed = true;
            }
        }
        else
        {
            Debug.Log("Not a player");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exited by: " + other.name);
        if (other.CompareTag("Player"))
        {
            onTriggerExit.Invoke();
        }
        else
        {
            Debug.Log("Not a player");
        }
    }
}