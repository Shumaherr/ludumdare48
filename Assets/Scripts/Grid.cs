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
        return _gridBuildings[placeX, placeY].CurrentTransform != null ||
               (_gridEnv[placeX, placeY].CurrentTransform != null && _gridEnv[placeX, placeY].CurrentTransform.CompareTag("Ground"));
    }
    
    public bool IsCellDigged(int placeX, int placeY)
    {
        return _gridEnv[placeX, placeY].CurrentTransform == null || !_gridEnv[placeX, placeY].CurrentTransform.CompareTag("Ground");
    }

    public void GenerateCells(Vector2Int gridSize)
    {
        _gridEnv = new GridCell[gridSize.x, gridSize.y];
        for (int i = gridSize.x - 1; i >= 0; i--)
        {
            for (int j = gridSize.y - 1; j >= 0; j--)
            {
                _gridEnv[i, j] = new GridCell();
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
                _gridBuildings[i, j] = new GridCell();
            }
        }
    }

    public void RemoveObjectFromCell(int x, int y)
    {
        if (Random.value > 0.85)
            _gridEnv[x, y].CurrentTransform = Instantiate(waterPrefab,
                new Vector3(transform.position.x + x * 5, transform.position.y - y * 5), Quaternion.identity);
        else
        {
            _gridEnv[x, y].CurrentTransform = null;
        }
    }

    public void RemoveObjectFromEnv(int x, int y)
    {
        if (Random.value > 0.85)
                    _gridEnv[x, y].CurrentTransform = Instantiate(waterPrefab,
                        new Vector3(transform.position.x + x * 5, transform.position.y - y * 5), Quaternion.identity);
                else
                {
                    _gridEnv[x, y].CurrentTransform = null;
                }
    }
}