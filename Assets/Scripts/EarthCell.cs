using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EarthCell : MonoBehaviour {
    [SerializeField] public CellData data;
    private Material _material;
    private int currentDig;

    private static readonly int Selected = Shader.PropertyToID("_IsSelected");

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

    private void Start() {
        _material = GetComponent<SpriteRenderer>().material;
    }

    private void SetTransparent(bool value) {
        _material.SetInt(Selected, value ? 1 : 0);
    }

    private void OnMouseEnter() {
        try {
            if (GameManager.Instance.BuildingToPlace || GameManager.Instance.Energy < 10) return;
            if (GameManager.Instance.FirstTurn && Utils.CellWorldPosToGridPos(transform.position).y == 0 ||
                HaveEmptyNeighbour(Utils.CellWorldPosToGridPos(transform.position))) {
                IsSelected = true;
            }
        }
        catch (IndexOutOfRangeException e) {
            // Handle the exception if needed
        }
    }

    private void OnMouseExit() {
        IsSelected = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void OnMouseDown() {
        if (!IsSelected) {
            return;
        }
        currentDig += GameManager.Instance.PlayerPick.DigPower;
        if(currentDig < data.Sprites.Count) {
            GetComponent<SpriteRenderer>().sprite = data.Sprites[currentDig];
            return;
        }

        var position = transform.position;
        GameManager.Instance.DigCell(Utils.CellWorldPosToGridPos(position));
        Utils.CreateWorldTextPopup("-10", position, 1.0f);
        Destroy(gameObject);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private bool HaveEmptyNeighbour(Vector2Int pos) {
        GameManager gm = GameManager.Instance;
        int gridSizeX = gm.gridSize.x;
        int gridSizeY = gm.gridSize.y;
        var x = pos.x;
        var y = pos.y;
        bool IsCellEmpty(int xPos, int yPos) => gm.IsCellEmpty(xPos, yPos);

        bool IsWithinGrid(int xPos, int yPos) =>
            xPos >= 0 && xPos < gridSizeX && yPos >= 0 && yPos < gridSizeY;

        return IsWithinGrid(x + 1, y) && IsCellEmpty(x + 1, y) ||
               IsWithinGrid(x - 1, y) && IsCellEmpty(x - 1, y) ||
               IsWithinGrid(x, y + 1) && IsCellEmpty(x, y + 1) ||
               IsWithinGrid(x, y - 1) && IsCellEmpty(x, y - 1);
    }
}