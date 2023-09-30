using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterField : MonoBehaviour
{
    private LetterBox[] letterBoxes;

    private void Awake()
    {
        letterBoxes = GetComponentsInChildren<LetterBox>();
    }

    private void Start()
    {
        SelectNextLetterBox();
    }

    private void OnEnable()
    {
        LetterBox.OnLetterBoxChange += LetterBox_OnLetterBoxChange;
    }

    private void OnDisable()
    {
        LetterBox.OnLetterBoxChange -= LetterBox_OnLetterBoxChange;    
    }

    private void LetterBox_OnLetterBoxChange(LetterBox letterBox)
    {
        CheckWords();
        SelectNextLetterBox();
    }

    void CheckWords()
    {

    }

    void SelectNextLetterBox()
    {
        var after = LetterBox.Selected;
        for (int i = 0; i<letterBoxes.Length; i++)
        {
            var box = letterBoxes[i];
            if (after == box)
            {
                after = null;
            } else if (box.Unlocked) {
                box.SelectBox();
                return;
            }
        }

        if (after != null)
        {
            Debug.LogError($"Didn't find currently selected box {after.name}!");
            return;
        }

        for (int i = 0; i<letterBoxes.Length; i++)
        {
            var box = letterBoxes[i];
            if (box.Unlocked)
            {
                box.SelectBox();
                return;
            }
        }
    }

}
