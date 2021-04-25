using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public const int CellSize = 5;
    [SerializeField] private GameObject grid;
    [SerializeField] public Vector2Int gridSize;
    [SerializeField] public Transform _groundLevel;
    [SerializeField] private Building buildingToPlace;
    public Camera mainCamera;

    private Grid _gridComponent;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        _gridComponent = grid.GetComponent<Grid>();
        InitField();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (buildingToPlace != null)
        {
            var groundPlane = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = GameManager.Instance.mainCamera.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);
                int x = DoubleExtension.RoundToValue(worldPosition.x, CellSize);
                int y = DoubleExtension.RoundToValue(worldPosition.y, CellSize);

                if (x < _groundLevel.position.x)
                    x = DoubleExtension.RoundToValue(_groundLevel.position.x, CellSize);
                if (y > _groundLevel.position.y)
                    y = DoubleExtension.RoundToValue(_groundLevel.position.y, CellSize);
                if (x > gridSize.x * CellSize)
                    x = DoubleExtension.RoundToValue((gridSize.x) * CellSize, CellSize); //TODO
                if (y > gridSize.y * CellSize)
                    y = DoubleExtension.RoundToValue(gridSize.y * CellSize, CellSize);

                buildingToPlace.transform.position = new Vector3(x, y, 0);

                if (Input.GetMouseButtonDown(0))
                {
                    if (!_gridComponent.IsPlaceTaken(x / CellSize,
                        Mathf.RoundToInt(GameManager.Instance._groundLevel.position.y +
                                         Math.Abs(y)) / GameManager.CellSize))
                    {
                        _gridComponent.PlaceFlyingBuilding(x / CellSize, Math.Abs(y) / CellSize,
                            buildingToPlace.transform);
                        buildingToPlace = null;
                    }
                }
            }
        }
    }

    void InitField()
    {
        _gridComponent.GenerateCells(gridSize);
    }

    public void StartPlacingBuilding(Building buildingPrefab)
    {
        if (buildingToPlace != null)
        {
            Destroy(buildingToPlace.gameObject);
        }

        buildingToPlace = Instantiate(buildingPrefab);
    }

    public void RemoveObjectFromCell(int x, int y)
    {
        _gridComponent.RemoveObjectFromCell(x, y);
    }
}