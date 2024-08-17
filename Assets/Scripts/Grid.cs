using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour {
    [SerializeField] private Transform earthPrefab;
    [SerializeField] private List<Transform> earthPrefabs;
    [SerializeField] private Transform waterPrefab;
    [SerializeField] private Transform backgroundPrefab;
    [SerializeField] private Transform stonePrefab;
    private GridCell[,] _gridEnv;
    private GridCell[,] _gridBuildings;
    private Renderer _mainRenderer;
    private Dictionary<CellData, Transform> _cellDatas;

    private void Awake() {
        _cellDatas = new Dictionary<CellData, Transform>();
        foreach (Transform prefab in earthPrefabs) {
            _cellDatas.Add(prefab.GetComponent<EarthCell>().data, prefab);
        }
    }

    public void PlaceFlyingBuilding(int placeX, int placeY, Transform building) {
        _gridBuildings[placeX, placeY].CurrentTransform = building;
        Debug.Log($"Placed {building.name} at {placeX} {placeY}");
    }

    public bool IsPlaceTaken(int placeX, int placeY) {
        return _gridBuildings[placeX, placeY].CurrentTransform != null;
    }
    
    public Building GetBuilding(int placeX, int placeY) {
        return _gridBuildings[placeX, placeY].CurrentTransform.GetComponent<Building>();
    }

    public bool IsCellDigged(int placeX, int placeY) {
        return !_gridEnv[placeX, placeY].CurrentTransform.CompareTag("Ground");
    }

    public void GenerateCells(Vector2Int gridSize) {
        _gridEnv = new GridCell[gridSize.x, gridSize.y];
        for (int i = 0; i < gridSize.x; i++) {
            for (int j = 0; j < gridSize.y; j++) {
                _gridEnv[i, j] = gameObject.AddComponent<GridCell>();
                _gridEnv[i, j].CurrentTransform = Instantiate(SelectPrefab(j),
                    new Vector3(transform.position.x + i * 5, transform.position.y - j * 5), Quaternion.identity);
                _gridEnv[i, j].CurrentTransform.parent = this.transform;
            }
        }

        _gridBuildings = new GridCell[gridSize.x, gridSize.y];
        for (int i = gridSize.x - 1; i >= 0; i--) {
            for (int j = gridSize.y - 1; j >= 0; j--) {
                _gridBuildings[i, j] = gameObject.AddComponent<GridCell>();
            }
        }
    }
    
    public void GenerateNextRow() {
        for (int i = 0; i < GameManager.Instance.gridSize.x; i++) {
            _gridEnv[i, GameManager.Instance.gridSize.y] = gameObject.AddComponent<GridCell>();
            _gridEnv[i, GameManager.Instance.gridSize.y].CurrentTransform = Instantiate(SelectPrefab(GameManager.Instance.gridSize.y),
                new Vector3(transform.position.x + i * 5, transform.position.y - GameManager.Instance.gridSize.y * 5), Quaternion.identity);
            _gridEnv[i, GameManager.Instance.gridSize.y].CurrentTransform.parent = this.transform;
        }
    }

    private Transform SelectPrefab(int groundLine) {
        // Step 1: Filter the _cellDatas dictionary
        var filteredTransforms = _cellDatas
            .Where(pair => pair.Key.LineToProbability.Any(entry => entry.Key <= groundLine)).ToList();

        // Step 2: Create a list of tuples
        var transformsWithProbabilities = filteredTransforms
            .SelectMany(pair => pair.Key.LineToProbability.Where(entry => entry.Key <= groundLine)
                .Select(entry => new { Transform = pair.Value, Probability = entry.Value })).ToList();

        // Step 3: Select one Transform based on the probabilities
        float totalProbability = transformsWithProbabilities.Sum(item => item.Probability);
        float randomPoint = Random.value * totalProbability;

        foreach (var item in transformsWithProbabilities) {
            if (randomPoint < item.Probability)
                return item.Transform;
            else
                randomPoint -= item.Probability;
        }

        // If no Transform is selected (which should not happen), return null
        return null;
    }

    public void RemoveObjectFromEnv(int x, int y) {
        Debug.Log($"Trying to remove {x} {y}");
        Vector2 newPos = transform.position;
        newPos.x += x * GameManager.CellSize;
        newPos.y -= y * GameManager.CellSize - GameManager.Instance._groundLevel.position.y;
        if (Random.value > 0.85)
            _gridEnv[x, y].CurrentTransform = Instantiate(waterPrefab,
                newPos, Quaternion.identity);
        else if (Random.value > 0.85) {
            _gridEnv[x, y].CurrentTransform = Instantiate(stonePrefab,
                newPos, Quaternion.identity);
        }
        else {
            _gridEnv[x, y].CurrentTransform = Instantiate(backgroundPrefab,
                newPos, Quaternion.identity);
        }
    }

    public void RemoveFirstLine() {
        for (int i = 0; i < GameManager.Instance.gridSize.x; i++) {
            if (_gridBuildings[i, 0].CurrentTransform != null) {
                _gridBuildings[i, 0].CurrentTransform.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                _gridBuildings[i, 0].CurrentTransform.gameObject.GetComponent<Building>().IsActive = false;
                GameManager.Instance.RemoveBuilding(_gridBuildings[i, 0].CurrentTransform.gameObject
                    .GetComponent<Building>());
                if (GameManager.Instance.WaitList.Contains(_gridBuildings[i, 0].CurrentTransform
                        .GetComponent<Building>())) {
                    GameManager.Instance.WaitList.Remove(_gridBuildings[i, 0].CurrentTransform
                        .GetComponent<Building>());
                }
            }

            if (_gridEnv[i, 0].CurrentTransform != null)
                _gridEnv[i, 0].CurrentTransform.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }

        _gridBuildings = Utils.TrimArray(0, _gridBuildings);
        _gridEnv = Utils.TrimArray(0, _gridEnv);
        GameManager.Instance.gridSize.y -= 1;
    }

    public bool NoTurn() {
        bool result = true;
        for (int i = 0; i < GameManager.Instance.gridSize.x; i++) {
            for (int j = 0; j < GameManager.Instance.gridSize.y; j++) {
                if (!_gridEnv[i, j].CurrentTransform.CompareTag("Ground")) {
                    result = false;
                }
            }
        }

        return result;
    }

    public bool IsWater(int x, int y) {
        return _gridEnv[x, y].CurrentTransform.CompareTag("Water");
    }

    public bool IsStone(int x, int y) {
        return _gridEnv[x, y].CurrentTransform.CompareTag("Stones");
    }

    /*private void OnDrawGizmos()
    {
        for (int i = 0; i < GameManager.Instance.gridSize.x; i++) {
            for (int j = 0; j < GameManager.Instance.gridSize.y; j++) {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.fontSize = 10;
                Vector3 textPosition = _gridEnv[i,j].CurrentTransform.position + new Vector3(0, 0.5f, 0);
                Handles.Label(textPosition, $"{i}, {j}", style);
            }
        }
    }*/
}