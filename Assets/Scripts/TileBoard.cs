using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileBoard : MonoBehaviour
{


    public GameManager gameManager;
    public Tile tilePrefab;
    public TileState[] tileStates;

    private TileGrid tileGrid;
    private List<Tile> tiles;
    private bool isAnimFinish = true;


    private void Awake()
    {
        tileGrid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>();
    }

    public void ClearBoard()
    {
        foreach (var cell in tileGrid.cells)
        {
            cell.tile = null;
        }

        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, tileGrid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(tileGrid.GetRandomEmptyCell());
        tiles.Add(tile);
    }

    private void Update()
    {
        if (isAnimFinish)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                // Start from top row, left most tile, move up
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                // Start from bottom row, left most tile, move down
                MoveTiles(Vector2Int.down, 0, 1, tileGrid.height - 2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                // Start from left most col, top tile, move left
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                // Start from right most col, top tile, move right
                MoveTiles(Vector2Int.right, tileGrid.width - 2, -1, 0, 1);
            }
        }
    }

    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool isAnimating = false;

        for (int x = startX; x >= 0 && x < tileGrid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < tileGrid.height; y += incrementY)
            {
                TileCell tileCell = tileGrid.GetCell(x, y);

                if(tileCell.isOccupied)
                {
                    isAnimating |= MoveTile(tileCell.tile, direction);
                }
            }
        }

        if (isAnimating)
        {
            StartCoroutine(IWaitForChange());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacentCell = tileGrid.GetAdjacentCell(tile.cell, direction);

        while (adjacentCell != null)
        {
            if (adjacentCell.isOccupied)
            {
                if (CanMerge(tile, adjacentCell.tile))
                {
                    Merge(tile, adjacentCell.tile);
                    return true;
                }
                break;
            }

            newCell = adjacentCell;
            adjacentCell = tileGrid.GetAdjacentCell(adjacentCell, direction);
        }

        if (newCell != null)
        {
            // Move cell
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return a.number == b.number;
    }

    private void Merge(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int newIndex = Mathf.Clamp(GetStateIndex(b.state) + 1, 0, tileStates.Length - 1);
        int newNumber = b.number * 2;

        gameManager.IncreaseScore(b.number);
        b.SetState(tileStates[newIndex], newNumber);
    }

    private int GetStateIndex(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i])
            {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator IWaitForChange()
    {
        isAnimFinish = false;

        yield return new WaitForSecondsRealtime(0.1f);

        isAnimFinish = true;

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].isLocked = false;
        }

        if (tiles.Count != tileGrid.size)
        {
            CreateTile();
        }

        if (CheckIsGameOver())
        {
            gameManager.GameOver();
        }
    }

    private bool CheckIsGameOver()
    {
        if (tiles.Count != tileGrid.size)
        {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = tileGrid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = tileGrid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = tileGrid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = tileGrid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile))
            {
                return false;
            }
            if (down != null && CanMerge(tile, down.tile))
            {
                return false;
            }
            if (left != null && CanMerge(tile, left.tile))
            {
                return false;
            }
            if (right != null && CanMerge(tile, right.tile))
            {
                return false;
            }
        }

        return true;
    }


}
