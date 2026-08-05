using UnityEngine;


public class DialogueTrigger : MonoBehaviour, Iinteractable
{
    [SerializeField] DialogueData normalDialogue;
    [SerializeField] DialogueData talkedDialogue;
    [SerializeField] DialogueData hasSomethingDialogue;
    [SerializeField] Camera playerCamera;
    Outline outline;

    public void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void Interact()
    {
        if (DialogueController.instance.IsDialogueActive()) { return; }



        if (!GameFlags.instance.GetFlag("talkedtoNPC1"))
        {
            //first dialogue
            DialogueController.instance.StartDialogue(normalDialogue);
            GameFlags.instance.SetFlag("talkedtoNPC1", true);
        }
        else if (GameFlags.instance.GetFlag("HasCube"))
        {
            //has cube dialogue
            DialogueController.instance.StartDialogue(hasSomethingDialogue);
        }
        else
        {
            //already talked to dialogue
            DialogueController.instance.StartDialogue(talkedDialogue);   
        }

    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }
}
