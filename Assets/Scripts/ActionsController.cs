using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ActionsController : MonoBehaviour
{
    [SerializeField]
    Button MoveButton;
    [SerializeField]
    string MoveAction;

    private void Start()
    {
        ClearActionMemory();
    }

    void ClearActionMemory()
    {
        MoveButton.gameObject.SetActive(false);
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
        SyncButton(MoveButton, MoveAction, abilities);
    }
}
