using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GriddedLevel : MonoBehaviour
{
    private static GriddedLevel _instance;
    public static GriddedLevel instance { 
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GriddedLevel>();
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        tiles = GetComponentsInChildren<LevelTile>();
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

    public static LevelTile GetTile(Vector3 position) => instance._GetTile(position);

    LevelTile[] tiles;

    private LevelTile _GetTile(Vector3 position)
    {
        for (int i = 0; i<tiles.Length; i++)
        {
            if (tiles[i].Contains(position))
            {
                return tiles[i];
            }
        }
        return null;
    }
}
