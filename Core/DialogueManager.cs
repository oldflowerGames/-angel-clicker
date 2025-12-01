using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public DialogueOverseer overseer;
    public DialogueStep[] steps = new DialogueStep[3];
    public int dialogueIndex = 0;
}
