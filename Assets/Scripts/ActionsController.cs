using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum ActionEventType
{
    None,
    Move,
    Left,
    Right,
}

public delegate void ActionEvent(ActionEventType eventType);


public class ActionsController : MonoBehaviour
{
    [System.Serializable]
    private struct ButtonAction
    {
        public Button button;
        public string actionName;
        public ActionEventType eventType;
    }

    public static event ActionEvent OnActionEvent;

    [SerializeField]
    ButtonAction[] buttonActions;

    private void Start()
    {
        ClearActionMemory();
    }

    void ClearActionMemory()
    {
        for (int i = 0; i<buttonActions.Length; i++)
        {
            var action = buttonActions[i];
            action.button.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        LetterField.OnAbilitiesChange += LetterField_OnAbilitiesChange;
    }

    private void OnDisable()
    {
        LetterField.OnAbilitiesChange -= LetterField_OnAbilitiesChange;
    }
    
    void SyncButton(Button button, string action, string[] abilities)
    {
        if (abilities.Contains(action))
        {
            if (!button.gameObject.activeSelf)
            {
                button.gameObject.SetActive(true);
            }
            button.interactable = true;
        } else
        {
            button.interactable = false;
        }
    }

    private void LetterField_OnAbilitiesChange(string[] abilities)
    {
        for (int i = 0; i<buttonActions.Length; i++)
        {
            var action = buttonActions[i];
            SyncButton(action.button, action.actionName, abilities);
        }
    }

    public void OnClickButton(Button btn)
    {
        for (int i = 0; i<buttonActions.Length; i++)
        {
            var action = buttonActions[i];
            if (action.button == btn)
            {
                OnActionEvent?.Invoke(action.eventType);
            }
        }
    }
}
