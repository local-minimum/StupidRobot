using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public delegate void AbilitiesChange(string[] abilities);

public class LetterField : MonoBehaviour
{
    public static event AbilitiesChange OnAbilitiesChange;

    private LetterBox[] letterBoxes;

    private void Awake()
    {
        letterBoxes = GetComponentsInChildren<LetterBox>();
    }

    private void Start()
    {
        SelectNextLetterBox(false);
    }

    private void OnEnable()
    {
        LetterBox.OnLetterBoxChange += LetterBox_OnLetterBoxChange;
        OnAbilitiesChange += LetterField_OnAbilitiesChange;
    }

    private void OnDisable()
    {
        OnAbilitiesChange -= LetterField_OnAbilitiesChange;
        LetterBox.OnLetterBoxChange -= LetterBox_OnLetterBoxChange;    
    }

    public bool CanCleanBoxes { get; private set; }

    private void LetterField_OnAbilitiesChange(string[] abilities)
    {
        CanCleanBoxes = abilities.Contains("CLEAN");
    }

    private void LetterBox_OnLetterBoxChange(LetterBox letterBox, bool backSpace)
    {
        CheckWords();
        SelectNextLetterBox(backSpace);
    }

    [SerializeField]
    string[] Words;

    [SerializeField]
    int columns = 6;

    bool MarkHorizontalWord(string word, int start)
    {
        for (int boxIdx = start, wIdx=0; wIdx<word.Length; boxIdx++, wIdx++)
        {
            if (letterBoxes[boxIdx].Letter == word.Substring(wIdx, 1))
            {
                letterBoxes[boxIdx].InWord = true;
            } else
            {
                Debug.LogError($"Failed horizontal word match {word} from {start}");
                return false;
            }
        }
        return true;
    }

    bool MarkVerticalWord(string word, int start)
    {
        for (int boxIdx = start, wIdx=0; wIdx<word.Length; wIdx++, boxIdx+=columns)
        {
            if (letterBoxes[boxIdx].Letter == word.Substring(wIdx, 1))
            {
                letterBoxes[boxIdx].InWord = true;
            } else
            {
                Debug.LogError($"Failed vertical word match {word} from {start}");
                return false;
            }
        }
        return true;
    }

    void ClearWordMarks()
    {
        for (int i = 0; i<letterBoxes.Length; i++)
        {
            letterBoxes[i].InWord = false;
        }
    }

    void CheckWords()
    {
        ClearWordMarks();
        var abilites = new List<string>();

        var rows = letterBoxes
            .Select((box, idx) => new { box.Letter, idx })
            .GroupBy(v => v.idx / columns)
            .Select(row => string.Join("", row.Select(e => e.Letter)))
            .ToArray();

        for (var wordIdx = 0; wordIdx < Words.Length; wordIdx++)
        {
            var word = Words[wordIdx];
            bool foundWord = false;

            for (var rIdx = 0; rIdx < rows.Length; rIdx++)
            {
                var row = rows[rIdx];
                if (row.Contains(word))
                {
                    var wordStart = row.IndexOf(word) + rIdx * columns;
                    if (MarkHorizontalWord(word, wordStart))
                    {
                        foundWord = true;
                        abilites.Add(word);
                        break;
                    }
                }
            }

            if (foundWord) continue;

            for (int c = 0; c<columns; c++)
            {
                var col = string.Join("", rows.Select(r => r.Substring(c, 1)));
                if (col.Contains(word))
                {
                    var wordStart = col.IndexOf(word) * columns + c;
                    if (MarkVerticalWord(word, wordStart))
                    {
                        abilites.Add(word);
                        break;
                    }
                }
            }
        }

        OnAbilitiesChange?.Invoke(abilites.ToArray());
    }

    void SelectNextLetterBox(bool backwards)
    {
        int index = backwards ? letterBoxes.Length - 1 : 0;
        System.Func<bool> done = () => backwards ? index == 0 : index == letterBoxes.Length;
        int direction = backwards ? -1 : 1;

        var after = LetterBox.Selected;
        while (!done())
        {
            var box = letterBoxes[index];
            if (after == box)
            {
                after = null;
            } else if (after == null && box.Unlocked) {
                box.SelectBox();
                return;
            }
            index += direction;
        }

        if (after != null)
        {
            Debug.LogError($"Didn't find currently selected box {after.name}!");
            return;
        }

        index = backwards ? letterBoxes.Length - 1 : 0;
        while (!done())
        {
            var box = letterBoxes[index];
            if (box.Unlocked)
            {
                box.SelectBox();
                return;
            }

            index += direction;
        }
    }

    static string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            LetterBox.Selected.Letter = "";
        }
        else
        {
            var text = Input.inputString;
            if (text.Length > 0)
            {
                var charText = text.Substring(text.Length - 1).ToUpper();
                if (validChars.Contains(charText))
                {
                    LetterBox.Selected.Letter = charText;
                }
            }
        }
    }
}
