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
    Dash,
}

public enum StatusEventType
{
    None,
    Eager,
    Destruct
}

public delegate void ActionEvent(ActionEventType eventType);
public delegate void StatusEvent(StatusEventType eventType, bool enabled);

public class ActionsController : MonoBehaviour
{   
    [System.Serializable]
    private struct ButtonAction
    {
        public Button button;
        public string actionName;
        public ActionEventType eventType;
    }

    [System.Serializable]
    private struct StatusText
    {
        public TMPro.TextMeshProUGUI textUI;
        public string actionName;
        public StatusEventType eventType;
        public bool isPositive;
        public bool beneficial;

        Color ActiveColor(Color positive, Color negative) => beneficial ? positive : negative;

        private Color GetColor(Color positive, Color negative, Color disabled, bool status)
        {
            return status ? ActiveColor(positive, negative) : disabled;
        }

        public void ResolveColor(Color positive, Color negative, Color disabled, bool status)
        {
            textUI.color = GetColor(positive, negative, disabled, status);
            textUI.gameObject.SetActive(true);
        }
    }

    Dictionary<string, bool> statusStates = new Dictionary<string, bool>();

    public static event ActionEvent OnActionEvent;
    public static event StatusEvent OnStatusEvent;

    [SerializeField]
    ButtonAction[] buttonActions;

    [SerializeField]
    StatusText[] statuses;

    [SerializeField]
    Color activeNegativeStatus;
    [SerializeField]
    Color activePositiveStatus;
    [SerializeField]
    Color disabledStatus;

    bool clearedActionMemory = false;

    private void Start()
    {
        ClearActionMemory();
        InitStatuses();
    }

    void ClearActionMemory()
    {
        if (clearedActionMemory) return;
        for (int i = 0; i<buttonActions.Length; i++)
        {
            var action = buttonActions[i];
            action.button.gameObject.SetActive(false);
        }

        clearedActionMemory = true;
    }

    bool initedStatuses = false;

    void InitStatuses()
    {
        if (initedStatuses) return;

        for (int i = 0; i<statuses.Length; i++)
        {
            var status = statuses[i];

            statusStates[status.actionName] = !status.isPositive;

            if (!status.isPositive)
            {
                status.ResolveColor(activePositiveStatus, activeNegativeStatus, disabledStatus, true);
            } else
            {
                status.textUI.gameObject.SetActive(false);
            }
            OnStatusEvent?.Invoke(status.eventType, statusStates[status.actionName]);
        }

        initedStatuses = true;
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
        ClearActionMemory();
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

    void SyncAbility(StatusText statusText, string[] abilities)
    {
        InitStatuses();
        var active = abilities.Contains(statusText.actionName) == statusText.isPositive;
        var current = statusStates[statusText.actionName];

        if (active == current) return;

        statusStates[statusText.actionName] = active;
        statusText.ResolveColor(activePositiveStatus, activeNegativeStatus, disabledStatus, active);
        OnStatusEvent?.Invoke(statusText.eventType, active);

    }

    private void LetterField_OnAbilitiesChange(string[] abilities)
    {
        Debug.Log(string.Join(",", abilities));
        for (int i = 0; i<buttonActions.Length; i++)
        {
            var action = buttonActions[i];
            SyncButton(action.button, action.actionName, abilities);
        }

        for (int i = 0; i<statuses.Length; i++)
        {
            SyncAbility(statuses[i], abilities);
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
