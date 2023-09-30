using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GriddedLevel : MonoBehaviour
{
    public static GriddedLevel instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance = this)
        {
            instance = null;
        }
    }

    public static float GridSize { get => instance.gridSize; }
    [SerializeField]
    float gridSize = 3f;
}
