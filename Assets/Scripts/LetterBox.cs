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
    private static LetterBox _selected;
    public static LetterBox Selected {
        get => _selected; 
        set {
            var prevSelected = _selected;
            _selected = value;
            if (prevSelected != value && prevSelected != null)
            {
                prevSelected.SyncUI();
            }
        } 
    }

    public string Letter { 
        get => TextUI.text; 
        set {
            TextUI.text = value;
            OnLetterBoxChange?.Invoke(this);
            SyncUI();
        } 
    }

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

    public void OnMouseEnter()
    {
        Debug.Log("Enter");
        if (!unlocked) return;
        Hovered = true;
        SyncUI();
    }

    public void OnMouseExit()
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


    private void Update()
    {
        if (Hovered && Input.GetMouseButtonDown(0))
        {
            SelectBox();
        }
    }
}
