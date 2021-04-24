using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public enum CellType
{
    Earth,
    Empty,
    Water
}

public class GridCell : MonoBehaviour
{
    private CellType _type;

    public CellType Type
    {
        get => _type;
        set => _type = value;
    }

    private Material _material;
    private Color _defaultColor;
    private bool _isSelected;

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
    }

    private void Update()
    {
    }

    private void OnMouseOver()
    {
        IsSelected = true;
    }

    private void OnMouseExit()
    {
        IsSelected = false;
    }
}