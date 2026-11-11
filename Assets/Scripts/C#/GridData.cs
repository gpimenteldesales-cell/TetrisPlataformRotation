using UnityEngine;

public class GridData
{
    public int Width { get; }
    public int Height { get; }
    public int Depth { get; }

    private readonly bool[,,] cells;

    public GridData(int width, int height, int depth)
    {
        Width = width;
        Height = height;
        Depth = depth;
        cells = new bool[width, height, depth];
    }

    public bool IsInside(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < Width
            && pos.y >= 0 && pos.y < Height
            && pos.z >= 0 && pos.z < Depth;
    }

    // Fora dos limites conta como ocupado, para bloquear peças nas bordas.
    public bool IsOccupied(Vector3Int pos)
    {
        if (!IsInside(pos)) return true;
        return cells[pos.x, pos.y, pos.z];
    }

    public bool CanPlace(Vector3Int pos)
    {
        return IsInside(pos) && !cells[pos.x, pos.y, pos.z];
    }

    public bool Occupy(Vector3Int pos)
    {
        if (!CanPlace(pos)) return false;
        cells[pos.x, pos.y, pos.z] = true;
        return true;
    }

    public void Free(Vector3Int pos)
    {
        if (IsInside(pos)) cells[pos.x, pos.y, pos.z] = false;
    }

    public void Clear()
    {
        System.Array.Clear(cells, 0, cells.Length);
    }
}