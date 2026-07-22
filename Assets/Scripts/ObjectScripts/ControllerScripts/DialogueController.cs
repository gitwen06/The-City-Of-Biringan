using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;


public class DialogueController : MonoBehaviour
{
    public static DialogueController instance;

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private List<Button> optionsButton;

    private List<TMP_Text> optionsText = new List<TMP_Text>();
    private DialogueData currentDialogue;
    private int currentNodeIndex;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach(var button in optionsButton)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            optionsText.Add(buttonText);
        }
    }

    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentNodeIndex = 0;

        dialogueBox.SetActive(true);
        ShowNode(); 

        HUDController.instance.DisableInteractionText();
        HUDController.instance.hideUINote();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        FreezePlayer();

        //display first node
        
    }

    public void ShowNode()
    {
        DialogueNode node = currentDialogue.nodes[currentNodeIndex];
        dialogueText.text = node.text;
        speakerNameText.text = node.speakerName;

        for(int i = 0; i < optionsButton.Count; i++)
        {
            if(i < node.options.Count)
            {
                optionsButton[i].gameObject.SetActive(true);
                optionsText[i].text = node.options[i].label;
            }
            else
            {
                optionsButton[i].gameObject.SetActive(false);

            }
        }
    }

    public void FreezePlayer()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Use the newer API to find any object of type PlayerMovement
        PlayerMovement pm = Object.FindAnyObjectByType<PlayerMovement>();
        if (pm != null)
        {
            pm.FreezeInput();
        }
    }

}
