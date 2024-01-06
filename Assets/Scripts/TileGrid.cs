using UnityEngine;

public class TileGrid : MonoBehaviour
{


    public TileRow[] rows { get; private set; }
    public TileCell[] cells { get; private set; }
    public int size => cells.Length;
    public int height => rows.Length;
    public int width => size / height;


    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }

    private void Start()
    {
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].cells.Length; x++)
            {
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
        }
    }

    public TileCell GetCell(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < width)
            return rows[y].cells[x];
        else
            return null;
    }

    public TileCell GetCell(Vector2Int coordinate)
    {
        if (coordinate.x >= 0 && coordinate.x < width && coordinate.y >= 0 && coordinate.y < width)
            return rows[coordinate.y].cells[coordinate.x];
        else
            return null;
    }

    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinate = cell.coordinates;
        // x start from left, move to right
        coordinate.x += direction.x;
        // y start from top, move to bot
        coordinate.y -= direction.y;

        return GetCell(coordinate);
    }

    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, cells.Length);
        int startingIndex = index;

        while (cells[index].isOccupied)
        {
            index++;

            if (index >= cells.Length)
            {
                index = 0;
            }

            if (index == startingIndex)
            {
                return null;
            }
        }

        return cells[index];
    }


}
