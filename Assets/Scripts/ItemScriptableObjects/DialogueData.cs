using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class DialogueData : ScriptableObject
{
    public List<DialogueNode> nodes;  
}
