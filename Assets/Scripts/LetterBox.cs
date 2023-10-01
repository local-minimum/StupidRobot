using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void LetterBoxChangeEvent(LetterBox letterBox, bool backSpace);

public class LetterBox : MonoBehaviour
{
    public static event LetterBoxChangeEvent OnLetterBoxChange;
    LetterField _field;
    LetterField field
    {
        get
        {
            if (_field == null)
            {
                _field = GetComponentInParent<LetterField>();
            }
            return _field;
        }
    }

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
    Color selectedBg;

    [SerializeField]
    Color inWordFg;

    [SerializeField]
    Color inWordBg;

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
            var empty = string.IsNullOrEmpty(value);
            TextUI.text = empty ? " " : value;
            OnLetterBoxChange?.Invoke(this, empty);
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
                Unlocked = field.CanCleanBoxes;
            }
            SyncUI();
        } 
    }

    void SyncUI()
    {
        TextUI.color = unlocked ? unlockedText : lockedText;

        if (Selected == this)
        {
            BgImage.color = selectedBg;
            FgImage.color = selectedFg;
            FgImage.enabled = true;
        }
        else if (Hovered)
        {
            BgImage.color = hoverBg;
            FgImage.color = hoverFg;
            FgImage.enabled = true;
        } else if (inWord)
        {
            BgImage.color = inWordBg;
            FgImage.color = inWordFg;
            FgImage.enabled = true;
        } else 
        {
            BgImage.color = unlocked ? unlockedBg : lockedBg;
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
        if (!unlocked) return;
        Hovered = true;
        SyncUI();
    }

    public void OnMouseExit()
    {
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
