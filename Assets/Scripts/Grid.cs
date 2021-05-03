using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    [SerializeField] private Transform[] buildingsPrefabs;
    [SerializeField] private Transform earthPrefab;
    [SerializeField] private Transform waterPrefab;
    [SerializeField] private Transform backgroundPrefab;
    [SerializeField] private Transform stonePrefab;
    private GridCell[,] _gridEnv;
    private GridCell[,] _gridBuildings;
    private Renderer _mainRenderer;


    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
    }


    public void PlaceFlyingBuilding(int placeX, int placeY, Transform building)
    {
        _gridBuildings[placeX, placeY].CurrentTransform = building;
    }

    public bool IsPlaceTaken(int placeX, int placeY)
    {
        return _gridBuildings[placeX, placeY].CurrentTransform != null;
    }

    public bool IsCellDigged(int placeX, int placeY)
    {
        return !_gridEnv[placeX, placeY].CurrentTransform.CompareTag("Ground");
    }

    public void GenerateCells(Vector2Int gridSize)
    {
        _gridEnv = new GridCell[gridSize.x, gridSize.y];
        for (int i = gridSize.x - 1; i >= 0; i--)
        {
            for (int j = gridSize.y - 1; j >= 0; j--)
            {
                _gridEnv[i, j] = gameObject.AddComponent<GridCell>();
                _gridEnv[i, j].CurrentTransform = Instantiate(earthPrefab,
                    new Vector3(transform.position.x + i * 5, transform.position.y - j * 5), Quaternion.identity);
                _gridEnv[i, j].CurrentTransform.parent = this.transform;
            }
        }

        _gridBuildings = new GridCell[gridSize.x, gridSize.y];
        for (int i = gridSize.x - 1; i >= 0; i--)
        {
            for (int j = gridSize.y - 1; j >= 0; j--)
            {
                _gridBuildings[i, j] = gameObject.AddComponent<GridCell>();
            }
        }
    }

    public void RemoveObjectFromEnv(int x, int y)
    {
        Vector2 newPos = transform.position;
        newPos.x += x * GameManager.CellSize;
        newPos.y -= y * GameManager.CellSize - GameManager.Instance._groundLevel.position.y;
        if (Random.value > 0.85)
            _gridEnv[x, y].CurrentTransform = Instantiate(waterPrefab,
                newPos, Quaternion.identity);
        else if (Random.value > 0.85)
        {
            _gridEnv[x, y].CurrentTransform = Instantiate(stonePrefab,
                newPos, Quaternion.identity);
        }
        else
        {
            _gridEnv[x, y].CurrentTransform = Instantiate(backgroundPrefab,
                newPos, Quaternion.identity);
        }
    }

    public void RemoveFirstLine()
    {
        for (int i = 0; i < GameManager.Instance.gridSize.x; i++)
        {
            if (_gridBuildings[i, 0].CurrentTransform != null)
            {
                _gridBuildings[i, 0].CurrentTransform.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                _gridBuildings[i, 0].CurrentTransform.gameObject.GetComponent<Building>().IsActive = false;
            }

            if (_gridEnv[i, 0].CurrentTransform != null)
                _gridEnv[i, 0].CurrentTransform.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }

        _gridBuildings = Utils.TrimArray(0, _gridBuildings);
        _gridEnv = Utils.TrimArray(0, _gridEnv);
        GameManager.Instance.gridSize.y -= 1;
    }

    public bool NoTurn()
    {
        bool result = true;
        for (int i = 0; i < GameManager.Instance.gridSize.x; i++)
        {
            for (int j = 0; j < GameManager.Instance.gridSize.y; j++)
            {
                if (!_gridEnv[i, j].CurrentTransform.CompareTag("Ground"))
                {
                    result = false;
                }
            }
        }

        return result;
    }

    public bool IsWater(int x, int y)
    {
        return _gridEnv[x, y].CurrentTransform.CompareTag("Water");
    }

    public bool IsStone(int x, int y)
    {
        return _gridEnv[x, y].CurrentTransform.CompareTag("Stones");
    }
}