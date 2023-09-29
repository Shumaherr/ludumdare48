using System;
using System.Collections.Generic;
using UnityEngine;

public class EarthCell : MonoBehaviour {
    [SerializeField] List<Sprite> sprites;
    private Material _material;
    private Transform _grid;
    private int maxDig;
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
        _grid = transform.parent;
        maxDig = sprites.Count - 1;
    }

    private void SetTransparent(bool value) {
        _material.SetInt(Selected, value ? 1 : 0);
    }

    private void OnMouseOver() {
    }

    private void OnMouseEnter() {
        if (!GameManager.Instance.BuildingToPlace) {
            Cursor.SetCursor(GameManager.Instance.shovelCursorTexture, Vector2.zero, CursorMode.Auto);
        }

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
        if (!GameManager.Instance.FirstTurn && !_isSelected) {
            return;
        }
        
        if(currentDig < maxDig) {
            currentDig += GameManager.Instance.PlayerPick.DigPower;
            GetComponent<SpriteRenderer>().sprite = sprites[currentDig];
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