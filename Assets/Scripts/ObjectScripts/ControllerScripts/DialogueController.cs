using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    public static DialogueController instance;

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private List<Button> optionsButton;

    private InputSystem_Actions inputActions;

    private List<TMP_Text> optionsText = new List<TMP_Text>();
    private DialogueData currentDialogue;
    private int currentNodeIndex;

    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        instance = this;
        inputActions = new InputSystem_Actions();
    }

    private void Start()
    {
        foreach (var button in optionsButton)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            optionsText.Add(buttonText);
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        if (!IsDialogueActive()) { return; }

        if (inputActions.Player.Interact.WasPressedThisFrame())
        {
            DialogueNode node = currentDialogue.nodes[currentNodeIndex];

            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = node.text;
                isTyping = false;
                ShowOptions(node);
            }
            else if (node.options.Count == 1)
            {
                SelectOptions(0);
            }
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

    public void EndDialogue()
    {
        currentDialogue = null;
        currentNodeIndex = 0;

        dialogueBox.SetActive(false);

        HUDController.instance.ShowInteractionText();
        HUDController.instance.showUiNote();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UnfreezePlayer();
    }

    public void ShowNode()
    {
        DialogueNode node = currentDialogue.nodes[currentNodeIndex];
        speakerNameText.text = node.speakerName;
        typingCoroutine = StartCoroutine(Typing(node));
    }

    public void ShowOptions(DialogueNode node)
    {
        for (int i = 0; i < optionsButton.Count; i++)
        {
            if (i < node.options.Count)
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

    public void SelectOptions(int optionIndex)
    {
        DialogueNode node = currentDialogue.nodes[currentNodeIndex];
        int nextIndex = node.options[optionIndex].nextNodeIndex;

        if (nextIndex >= 0 && nextIndex < currentDialogue.nodes.Count)
        {
            currentNodeIndex = nextIndex;
            ShowNode();
        }
        else
        {
            EndDialogue();
        }
    }

    public bool IsDialogueActive()
    {
        return currentDialogue != null;
    }

    IEnumerator Typing(DialogueNode node)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char s in node.text)
        {
            yield return new WaitForSeconds(0.1f);
            dialogueText.text += s;
            if (s == '.' || s == ',')
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        isTyping = false;
        ShowOptions(node);
    }

    public void FreezePlayer()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerMovement pm = Object.FindAnyObjectByType<PlayerMovement>();
        if (pm != null) pm.FreezeInput();
    }

    public void UnfreezePlayer()
    {
        PlayerMovement pm = Object.FindAnyObjectByType<PlayerMovement>();
        if (pm != null) pm.UnfreezeInput();
    }
}