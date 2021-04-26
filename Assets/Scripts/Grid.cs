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
    private GridCell[,] _grid;
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
        _grid[placeX, placeY].CurrentTransform = building;
    }

    public bool IsPlaceTaken(int placeX, int placeY)
    {
        return _grid[placeX, placeY].CurrentTransform != null;
    }

    public void GenerateCells(Vector2Int gridSize)
    {
        _grid = new GridCell[gridSize.x, gridSize.y];
        for (int i = gridSize.x - 1; i >= 0; i--)
        {
            for (int j = gridSize.y - 1; j >= 0; j--)
            {
                _grid[i, j] = new GridCell();
                _grid[i, j].CurrentTransform = Instantiate(earthPrefab,
                    new Vector3(transform.position.x + i * 5, transform.position.y - j * 5), Quaternion.identity);
                _grid[i, j].CurrentTransform.parent = this.transform;
            }
        }
    }

    public void RemoveObjectFromCell(int x, int y)
    {
        if (Random.value > 0.85)
            _grid[x, y].CurrentTransform = Instantiate(waterPrefab,
                new Vector3(transform.position.x + x * 5, transform.position.y - y * 5), Quaternion.identity);
        else
        {
            _grid[x, y].CurrentTransform = null;
        }
    }
}