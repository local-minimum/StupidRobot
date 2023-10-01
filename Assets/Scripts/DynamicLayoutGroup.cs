using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DynamicLayoutGroup : MonoBehaviour
{
    void Start()
    {
        SyncChildren();
    }

    private void OnEnable()
    {
        SyncChildren();
    }

    [SerializeField]
    int columns = 6;

    public int Columns => columns;

    [SerializeField]
    Vector2 inset = Vector2.one;

    [SerializeField]
    Vector2 gap = Vector2.zero;

    void SyncChildren()
    {
        var childSize = (inset.x - gap.x * (columns - 1)) / columns * Vector2.one;

        var halfPadding = (Vector2.one - inset) * 0.5f;
        for (int i = 0, l = transform.childCount; i<l; i++)
        {
            var row = i / columns;
            var col = i % columns;

            var child = transform.GetChild(i);
            var childRt = child.transform as RectTransform;


            childRt.offsetMax = Vector2.zero;
            childRt.offsetMin = Vector2.zero;

            childRt.anchorMin = new Vector2(
                halfPadding.x + (gap.x + childSize.x) * col,
                1 - (halfPadding.x + childSize.x * (row + 1) + gap.x * row)
            );
                
            childRt.anchorMax = childRt.anchorMin + childSize;
        }
    }
}
