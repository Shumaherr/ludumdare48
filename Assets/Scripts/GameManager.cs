using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject grid;

    private Camera _mainCamera;

    private Grid _gridComponent;
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
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
        
    }

    void InitField()
    {
        _gridComponent.GenerateCells();
    }
}
