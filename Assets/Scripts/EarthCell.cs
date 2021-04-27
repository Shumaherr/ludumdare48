using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

public class EarthCell : MonoBehaviour
{
    private Material _material;
    private bool _isSelected;
    private Transform _grid;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            SetTransparent(value);
            _isSelected = value;
        }
    }

    private void SetTransparent(bool value)
    {
        if (value)
            _material.SetInt("_IsSelected", 1);
        else
        {
            _material.SetInt("_IsSelected", 0);
        }
    }

    private void Start()
    {
        _material = GetComponent<SpriteRenderer>().material;
        _grid = GetComponentInParent<Transform>();
    }

    private void Update()
    {
    }

    private void OnMouseOver()
    {
        if(HaveEmptyNeighbour(Mathf.RoundToInt(transform.position.x / GameManager.CellSize),
            Mathf.RoundToInt(GameManager.Instance._groundLevel.position.y + Math.Abs(transform.position.y)) /
            GameManager.CellSize) && !GameManager.Instance.BuildingToPlace && GameManager.Instance.Energy - 10 >= 0)
            IsSelected = true;
    }

    void OnMouseEnter()
    {
        if(!GameManager.Instance.BuildingToPlace)
            Cursor.SetCursor(GameManager.Instance.shovelCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        IsSelected = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void OnMouseDown()
    {
        if(!_isSelected && !GameManager.Instance.FirstTurn)
            return;
        GameManager.Instance.DigCell(Mathf.RoundToInt(transform.position.x / GameManager.CellSize),
            Mathf.RoundToInt(GameManager.Instance._groundLevel.position.y + Math.Abs(transform.position.y)) /
            GameManager.CellSize);
        CodeMonkey.Utils.UtilsClass.CreateWorldTextPopup("-10", transform.position, 1.0f);
        Destroy(gameObject);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private bool HaveEmptyNeighbour(int x, int y)
    {
        if (x == 0)
        {
            if (y == 0)
            {
                return GameManager.Instance.IsCellEmpty(x + 1, y) || GameManager.Instance.IsCellEmpty(x, y + 1);
            }
            
            if (y == GameManager.Instance.gridSize.y - 1)
            {
                return GameManager.Instance.IsCellEmpty(x + 1, y) || GameManager.Instance.IsCellEmpty(x, y - 1);
            }
            return GameManager.Instance.IsCellEmpty(x + 1, y) || GameManager.Instance.IsCellEmpty(x, y + 1) ||
                   GameManager.Instance.IsCellEmpty(x, y - 1);
        }

        if (y == 0)
        {
            return GameManager.Instance.IsCellEmpty(x + 1, y) || GameManager.Instance.IsCellEmpty(x, y + 1) ||
                   GameManager.Instance.IsCellEmpty(x - 1, y);
        }
        if (x == GameManager.Instance.gridSize.x - 1)
        {
            if (y == GameManager.Instance.gridSize.y - 1)
            {
                return GameManager.Instance.IsCellEmpty(x - 1, y) || GameManager.Instance.IsCellEmpty(x, y - 1);
            }

            return GameManager.Instance.IsCellEmpty(x - 1, y) || GameManager.Instance.IsCellEmpty(x, y - 1) ||
                   GameManager.Instance.IsCellEmpty(x, y - 1);
        }

        if (y == GameManager.Instance.gridSize.y - 1)
        {
            return GameManager.Instance.IsCellEmpty(x + 1, y) || GameManager.Instance.IsCellEmpty(x, y - 1) ||
                   GameManager.Instance.IsCellEmpty(x - 1, y);
        }

        return GameManager.Instance.IsCellEmpty(x + 1, y) || GameManager.Instance.IsCellEmpty(x, y + 1) ||
               GameManager.Instance.IsCellEmpty(x - 1, y) || GameManager.Instance.IsCellEmpty(x, y - 1);
    }
}