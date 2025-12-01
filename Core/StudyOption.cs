using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class StudyOption : MonoBehaviour
{
    public StudyMenu studyMenu;
    public List<Builder> assignedBuilders = new List<Builder>();
    public Button addButton, subtractButton;
    public TextMeshProUGUI nameText, levelText, assignedWorkersText;
    public Slider expBar;

    public void AddWorker()
    {
        int workersAvailable = studyMenu.GetWorkersAvailable();
        if (workersAvailable <= 0)
        {
            return;
        }

        int addCount = 1;
        if (Keyboard.current.shiftKey.isPressed) { addCount = Mathf.Min(workersAvailable, 10); }

        for(int ii = 0; ii < addCount; ii++)
        {
            Builder tempBuilder = studyMenu.FindAvailableWorker();

            if (tempBuilder != null)
            {
                assignedBuilders.Add(tempBuilder);
                tempBuilder.SetState(Builder.State.Studying);
                studyMenu.CheckWorkers();
            }
        }
    }

    public void RemoveWorker()
    {
        if(assignedBuilders.Count <= 0) { return; }

        int removeCount = 1;
        if (Keyboard.current.shiftKey.isPressed) { removeCount = Mathf.Min(assignedBuilders.Count, 10); }

        for(int ii = 0; ii < removeCount; ii++)
        {
            assignedBuilders[0].SetState(Builder.State.AssignedToStudy);
            assignedBuilders.RemoveAt(0);
        }

        studyMenu.CheckWorkers();
    }

    public void CheckButtons()
    {
        if (studyMenu.GetWorkersAvailable() > 0)
        {
            addButton.interactable = true;
        }
        else { addButton.interactable = false; }

        if (assignedBuilders.Count > 0)
        {
            subtractButton.interactable = true;
        }
        else { subtractButton.interactable = false; }
        assignedWorkersText.text = assignedBuilders.Count.ToString();
    }
}
