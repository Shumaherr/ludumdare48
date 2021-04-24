using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector2Int gridSize = new Vector2Int(10, 10);
    private Vector2 _anchor;
    [SerializeField] private Transform[] buildingsPrefabs;
    [SerializeField] private Transform earthPrefab;
    private Transform[,] _grid;
    private Camera _mainCamera;
    private Renderer _mainRenderer;
    
    private void Awake()
    {
        _grid = new Transform[gridSize.x, gridSize.y];
        _mainCamera = Camera.main;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateCells()
    {
        for (int i = gridSize.x - 1; i >= 0; i--)
        {
            for (int j = gridSize.y - 1; j >= 0; j--)
            {
                _grid[i, j] = Instantiate(earthPrefab, new Vector3(transform.position.x + i, transform.position.y + j), Quaternion.identity);
                _grid[i, j].GetComponent<GridCell>().Type = CellType.Earth;
                _grid[i, j].parent = this.transform;
            }
        }
    }
}
