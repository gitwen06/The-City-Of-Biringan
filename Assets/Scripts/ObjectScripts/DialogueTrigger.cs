using UnityEngine;

/* TODO IN DIALOGUETRIGGER:
 * Set up the dialogue trigger to check for the game flag "HasCube" and if it is true, then it will trigger the hasSomethingDialogue instead of the normalDialogue.
 * Set up gameflags that passes into savemanager to know if the player has talked to the NPC before, and if they have, then it will trigger the talkedDialogue instead of the normalDialogue. 
 */

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

        Debug.Log("DialogueTrigger.Interact() called");

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
