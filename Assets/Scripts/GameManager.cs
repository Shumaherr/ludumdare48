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
    [SerializeField] public Texture2D shovelCursorTexture;
    
    [SerializeField] public int startEnergy;
    [SerializeField] public int startOre;
    
    private int _energy;

    public int Energy
    {
        get => _energy;
        set
        {
            _energy = value;
            if (OnEnergyChange != null)
                OnEnergyChange(_energy);
        }
    }

    public int Ore
    {
        get => _ore;
        set
        {
            _ore = value;
            if (OnOreChange != null)
                OnOreChange(_ore);
        }
    }

    private int _ore;

    private bool _firstTurn;

    public bool FirstTurn => _firstTurn;

    public Camera mainCamera;

    private Grid _gridComponent;
    
    //Delegates
    public delegate void OnEnergyChangeDelegate(int value);
    public event OnEnergyChangeDelegate OnEnergyChange;
    public delegate void OnOreChangeDelegate(int value);
    public event OnOreChangeDelegate OnOreChange;

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
        _firstTurn = true;
        Ore = startOre;
        Energy = startEnergy;
    }

    //Create flying building, that follow the mouse
    public void StartPlacingBuilding(Building buildingPrefab)
    {
        if (buildingToPlace != null)
        {
            Destroy(buildingToPlace.gameObject);
        }

        buildingToPlace = Instantiate(buildingPrefab);
    }

    //Remove object from the main matrix
    public void DigCell(int x, int y)
    {
        if (_firstTurn)
            _firstTurn = false; //First turn have bin did. Now player can dig only neighbour cells
        _gridComponent.RemoveObjectFromEnv(x, y);
    }

    public bool IsCellEmpty(int x, int y)
    {
        return _gridComponent.IsCellDigged(x, y);
    }
}