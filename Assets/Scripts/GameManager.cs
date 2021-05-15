﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Plane = UnityEngine.Plane;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class GameManager :MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }
    
    public const int CellSize = 5;
    [SerializeField] private GameObject grid;
    [SerializeField] public Vector2Int gridSize;
    [SerializeField] public Transform _groundLevel;
    [SerializeField] private Building buildingToPlace;
    [SerializeField] public Texture2D shovelCursorTexture;
    
    [SerializeField] public int startEnergy;
    [SerializeField] public int startStone;
    [SerializeField] public int startPeople = 0;
    
    private int _energy;
    private int _people;
    private int _stone;
    private int _freePeople;

    private List<Building> _buildings;
    private List<Building> _waitList; //Building that cant be activated because of there are no enough people
    private bool _firstTurn;

    public bool FirstTurn => _firstTurn;

    public Camera mainCamera;

    private Grid _gridComponent;
    private bool _moveCamera;
    
    public Building BuildingToPlace => buildingToPlace;
    
    public int People
    {
        get => _people;
        set
        {
            if (value < _people)
            {
                _freePeople -= value;
            }
            _people = value;
            if (OnPeopleChange != null)
            {
                OnPeopleChange(_people);
                ActivateWaitingBuildings();
            }
        }
    }

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

    public int Stone
    {
        get => _stone;
        set
        {
            _stone = value;
            if (OnStoneChange != null)
                OnStoneChange(_stone);
        }
    }

    //Delegates
    public delegate void OnEnergyChangeDelegate(int value);
    public event OnEnergyChangeDelegate OnEnergyChange;
    public delegate void OnPeopleChangeDelegate(int value);
    public event OnPeopleChangeDelegate OnPeopleChange;
    public delegate void OnOreChangeDelegate(int value);
    public event OnOreChangeDelegate OnStoneChange;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        _gridComponent = grid.GetComponent<Grid>();
        InitField();
        _waitList = new List<Building>();
        _buildings = new List<Building>();
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
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

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
                    PlaceBuilding(x, y);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    Destroy(buildingToPlace.gameObject);
                    buildingToPlace = null;
                }
            }
        }
    }

    private void PlaceBuilding(int x, int y)
    {
        int xPos = x / CellSize;
        int yPos = Mathf.RoundToInt(GameManager.Instance._groundLevel.position.y +
                                    Math.Abs(y)) / GameManager.CellSize;
        if (CanBuildingPlaced(xPos, yPos))
        {
            _gridComponent.PlaceFlyingBuilding(xPos, yPos,
                buildingToPlace.transform);
            Stone -= buildingToPlace.CostsStone;
            if (buildingToPlace.Type == Type.House)
            {
                People += 10;
                _freePeople += 10;
            }
            else
            {
                _freePeople -= buildingToPlace.CostsPeople;
            }

            if (_freePeople >= 0)
            {
                buildingToPlace.IsActive = true;
            }
            else
            {
                _waitList.Add(buildingToPlace);
            }
            _buildings.Add(buildingToPlace);
            buildingToPlace = null;
        }
    }

    bool CanBuildingPlaced(int x, int y)
    {
        if (_gridComponent.IsPlaceTaken(x, y) || !_gridComponent.IsCellDigged(x, y))
            return false;
        switch (buildingToPlace.Type)
        {
            case Type.House:
                break;
            case Type.PowerPlant:
                if (!_gridComponent.IsWater(x, y))
                    return false;
                break;
            case Type.Mine:
                if (!_gridComponent.IsStone(x, y))
                    return false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }

    private bool CheckResources(Building building)
    {
        if (Stone < building.CostsStone)
        {
            //TODO tooltip
            return false;
        }

        return true;
    }
    void InitField()
    {
        _gridComponent.GenerateCells(gridSize);
        _firstTurn = true;
        Stone = startStone;
        Energy = startEnergy;
        People = startPeople;
    }

    //Create flying building, that follow the mouse
    public void StartPlacingBuilding(Building buildingPrefab)
    {
        if (!CheckResources(buildingPrefab))
            return;
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
        {
            _firstTurn = false; //First turn have bin did. Now player can dig only neighbour cells
            InvokeRepeating("MoveGround", 10.0f, 10.0f);
        }
        _gridComponent.RemoveObjectFromEnv(x, y + (int)_groundLevel.position.y / CellSize);
        Energy -= 10;
    }

    public bool IsCellEmpty(int x, int y)
    {
        return _gridComponent.IsCellDigged(x, y);
    }

    void MoveGround() {
        _gridComponent.RemoveFirstLine();
        _groundLevel.position = new Vector2(_groundLevel.position.x, _groundLevel.position.y - CellSize); //TODO Animate
        Vector3 newPos = mainCamera.transform.position;
        newPos.y -= CellSize;
        StartCoroutine("MoveCamera", newPos);
        
        if (_gridComponent.NoTurn())
        {
            SceneManager.LoadScene("GameOver");
        }
        DeactivateBuildings();
    }

    private IEnumerator MoveCamera(Vector3 newPos)
    {
        while(mainCamera.transform.position.y > newPos.y) {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, newPos, 0.5f);
            yield return new WaitForSeconds(0.1f);
        }

    }
    
    void ActivateWaitingBuildings()
    {
        int i = 0;
        while (_waitList.Count != 0 || i < _waitList.Count || _freePeople < 0)
        {
            if (_waitList[i].CostsPeople - _freePeople > 0)
            {
                _freePeople += _waitList[i].CostsPeople;
                _waitList[i].IsActive = true;
                _waitList.Remove(_waitList[i]);
            }

            i++;
        }
    }

    void DeactivateBuildings()
    {
        if(_buildings.Count == 0)
            return;
        foreach (var building in _buildings)
        {
            if (building.IsActive && building.Type != Type.House && _freePeople < 0)
            {
                building.IsActive = false;
                _freePeople += building.CostsPeople;
            }
                
        }
    }
    
    private void FixedUpdate()
    {
       
    }
}