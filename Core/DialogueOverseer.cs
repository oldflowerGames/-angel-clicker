using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueOverseer : MonoBehaviour
{
    public GameManager GM;
    public DialogueManager[] dialogues = new DialogueManager[5];
    public DialogueManager activeDialogue;
    public TextMeshProUGUI text;
    public Image portrait;
    public CanvasGroup canvasGroup;
    public int stepTracker;
    public bool debugMode;
    public bool available;
    DialogueStep currentStep;

    public void OnEnable()
    {
        if (debugMode)
        {
            if(activeDialogue != null)
            {
                QueueDialogue(activeDialogue);
            }
        }
    }

    public void QueueDialogue(DialogueManager pass)
    {
        activeDialogue = pass;
        StartDialogue();
        canvasGroup.interactable = true;
    }

    public void QueueDialogue(int index)
    {
        activeDialogue = dialogues[index];
        StartDialogue();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void QueueDialogueDelay(int index, float delay)
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        StartCoroutine(DialogueDelay(index, delay));
    }

    public IEnumerator DialogueDelay(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        QueueDialogue(index);
    }

    public void StartDialogue()
    {
        stepTracker = 0;
        available = true;
        NextStep();
        available = false;
        GM.fade.StartFadeIn(canvasGroup);
        StartCoroutine(AvailableDelay());//slight delay before being allowed to click, to prevent accidentally skipping
    }

    public IEnumerator AvailableDelay()
    {
        available = false;
        yield return new WaitForSeconds(2f);
        available = true;
    }

    public void NextStep()
    {
        if(activeDialogue == null) { return; }
        if(available == false) { return; }
        if (stepTracker >= activeDialogue.steps.Length)
        {
            FinishDialogue();
            return;
        }

        currentStep = activeDialogue.steps[stepTracker];
        switch (currentStep.specialStepIndex)
        {
            case 1:
                GM.hellScore.gameObject.SetActive(true);
                GM.hellScore.SetScore(66600);
                GM.sfx.PlaySound(GM.sfx.badSurprise);
                //play a goofy sound effect
                break;
        }
        text.text = currentStep.text;
        if(currentStep.portrait == null) { portrait.gameObject.SetActive(false); }
        else
        {
            portrait.gameObject.SetActive(true);
            portrait.sprite = currentStep.portrait;
        }

        stepTracker++;
    }

    public void FinishDialogue()
    {
        GM.fade.StartFadeOut(canvasGroup, 0, 1f, 0, canvasGroup.alpha);
        canvasGroup.interactable = false;
        if(activeDialogue == null) { return; }
        switch (activeDialogue.dialogueIndex)
        {
            case 1:
                GM.tutorialWindow.gameObject.SetActive(true);
                break;
            case 2:
                GM.buildings[5].notification.SetActive(true);
                break;
        }
        activeDialogue = null;
    }
}
