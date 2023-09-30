using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void LetterBoxChangeEvent(LetterBox letterBox);

public class LetterBox : MonoBehaviour
{
    public static event LetterBoxChangeEvent OnLetterBoxChange;
    
    [SerializeField]
    TMPro.TextMeshProUGUI TextUI;

    [SerializeField]
    Image BgImage;

    [SerializeField]
    Image FgImage;

    [SerializeField]
    Color lockedBg;

    [SerializeField]
    Color lockedText;

    [SerializeField]
    Color unlockedBg;

    [SerializeField]
    Color unlockedText;

    [SerializeField]
    Color hoverBg;

    [SerializeField]
    Color hoverFg;

    [SerializeField]
    Color selectedFg;

    [SerializeField]
    Color inWordFg;

    [SerializeField]
    bool startUnlocked;

    bool unlocked = false;
    public bool Unlocked
    {
        get => unlocked;
        set {
            unlocked = value;
            SyncUI();
        }
    }
    private bool Hovered { get; set; }
    public static LetterBox Selected { get; private set; }

    public string Letter { get => TextUI.text; }

    private bool inWord = false;
    public bool InWord { 
        get => inWord; 
        set {
            inWord = value;
            if (value && !Unlocked)
            {
                Unlocked = true;
            }
            SyncUI();
        } 
    }

    void SyncUI()
    {
        Debug.Log($"Syncing {name} H:{Hovered} S:{Selected == this}");
        TextUI.color = unlocked ? unlockedText : lockedText;

        if (Hovered)
        {
            BgImage.color = hoverBg;
        } else 
        {
            BgImage.color = unlocked ? unlockedBg : lockedBg;
        }
        
        if (Hovered || Selected == this)
        {
            FgImage.enabled = true;
            FgImage.color = Selected == this ? selectedFg : hoverFg;
        } else if (InWord)
        {
            FgImage.color = inWordFg;
            FgImage.enabled = true;
        } else
        {
            FgImage.enabled = false;
        }
    }

    private void Start()
    {
        unlocked = startUnlocked;
        SyncUI();
    }

    private void OnMouseEnter()
    {
        Debug.Log("Enter");
        if (!unlocked) return;
        Hovered = true;
        SyncUI();
    }

    private void OnMouseExit()
    {
        Debug.Log("Exit");
        Hovered = false;
        SyncUI();
    }

    public void SelectBox()
    {
        Selected = this;
        SyncUI();
    }

    static string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private void Update()
    {
        if (Hovered && Input.GetMouseButtonDown(0))
        {
            SelectBox();
        } else if (Selected == this)
        {
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Space))
            {
                TextUI.text = "";
                OnLetterBoxChange?.Invoke(this);
                SyncUI();
            }

            var text = Input.inputString;
            if (text.Length > 0)
            {
                var charText = text.Substring(text.Length - 1).ToUpper();
                if (validChars.Contains(charText))
                {
                    TextUI.text = charText;
                    OnLetterBoxChange?.Invoke(this);
                    SyncUI();
                }
            }
        }
    }
}
