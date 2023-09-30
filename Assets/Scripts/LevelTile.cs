using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
    [SerializeField]
    bool accessible = true;

    [SerializeField]
    int elevation = 0;

    public int Elevation => elevation;

    private GameObject occupier;

    public bool Accessible
    {
        get => accessible && occupier == null;
    }

    Color GizmoColor()
    {
        if (!accessible)
        {
            return Color.magenta;
        }

        return Color.cyan;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor();
        Gizmos.DrawWireCube(transform.position, Vector3.one * GriddedLevel.GridSize * 0.97f);
    }

    public bool Contains(Vector3 position)
    {
        var center = transform.position;
        var halfSize = GriddedLevel.GridSize / 2f;

        return center.y - halfSize <= position.y && center.y + halfSize >= position.y
            && center.x - halfSize <= position.x && center.x + halfSize >= position.x;
    }

    public void Occupy(GameObject go)
    {
        if (occupier == null || occupier == go)
        {
            occupier = go;
        } else
        {
            Debug.LogError($"{go} attempted to occupy {name} but {occupier} was already there");
        }
    }

    public void Free(GameObject go)
    {
        if (occupier == go)
        {
            occupier = null;
        } else
        {
            Debug.LogWarning($"{go} attempted to free {name} but wasn't occupying it ({occupier} was)");
        }
    }

    public void Arrive(GameObject go)
    {
        if (occupier == null || occupier == go)
        {
            gameObject.BroadcastMessage("OnEnterTile", occupier, SendMessageOptions.DontRequireReceiver);
        } else
        {
            Debug.LogError($"{go} attempted to arrive {name} but {occupier} was already there");
        }

    }
}
