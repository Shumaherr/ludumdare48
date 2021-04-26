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
            GameManager.CellSize))
            IsSelected = true;
    }

    void OnMouseEnter()
    {
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
        Debug.Log(Mathf.RoundToInt(transform.position.x / GameManager.CellSize) + " " +
                  (Mathf.RoundToInt(GameManager.Instance._groundLevel.position.y + Math.Abs(transform.position.y)) /
                   GameManager.CellSize));
        GameManager.Instance.DigCell(Mathf.RoundToInt(transform.position.x / GameManager.CellSize),
            Mathf.RoundToInt(GameManager.Instance._groundLevel.position.y + Math.Abs(transform.position.y)) /
            GameManager.CellSize);
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

            return GameManager.Instance.IsCellEmpty(x + 1, y) || GameManager.Instance.IsCellEmpty(x, y + 1) ||
                   GameManager.Instance.IsCellEmpty(x, y - 1);
        }

        if (y == 0)
        {
            return GameManager.Instance.IsCellEmpty(x + 1, y) || GameManager.Instance.IsCellEmpty(x, y + 1) ||
                   GameManager.Instance.IsCellEmpty(x - 1, y);
        }

        return GameManager.Instance.IsCellEmpty(x + 1, y) || GameManager.Instance.IsCellEmpty(x, y + 1) ||
               GameManager.Instance.IsCellEmpty(x - 1, y) || GameManager.Instance.IsCellEmpty(x, y - 1);
    }
}