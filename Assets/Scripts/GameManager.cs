using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Plane = UnityEngine.Plane;
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
    [SerializeField] public int startOre;
    
    private int _energy;

    public Building BuildingToPlace => buildingToPlace;
    
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
    private bool _moveCamera;

    //Delegates
    public delegate void OnEnergyChangeDelegate(int value);
    public event OnEnergyChangeDelegate OnEnergyChange;
    public delegate void OnOreChangeDelegate(int value);
    public event OnOreChangeDelegate OnOreChange;


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
                    int xPos = x / CellSize;
                    int yPos = Mathf.RoundToInt(GameManager.Instance._groundLevel.position.y +
                                                Math.Abs(y)) / GameManager.CellSize;
                    if (CanBuildingPlaced(xPos,yPos))
                    {
                        _gridComponent.PlaceFlyingBuilding(xPos, yPos,
                            buildingToPlace.transform);
                        buildingToPlace.IsActive = true;
                        buildingToPlace = null;
                    }
                }
            }
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
                if (!_gridComponent.IsOre(x, y))
                    return false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
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
    }

    private IEnumerator MoveCamera(Vector3 newPos)
    {
        while(mainCamera.transform.position.y > newPos.y) {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, newPos, 0.5f);
            yield return new WaitForSeconds(0.1f);
        }

    }

    private void FixedUpdate()
    {
       
    }
}