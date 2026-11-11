using UnityEngine;

public class GridData
{
    public int Width { get; }
    public int Height { get; }
    public int Depth { get; }

    private readonly bool[,,] cells;

    public bool IsRowFull(int y, int z)
    {
        for (int x = 0; x < Width; x++)
            if (!cells[x, y, z]) return false;
        return true;
    }

    // Remove a linha e desce tudo que estava acima, na mesma camada Z.
    public void RemoveRow(int y, int z)
    {
        for (int yy = y; yy < Height - 1; yy++)
            for (int x = 0; x < Width; x++)
                cells[x, yy, z] = cells[x, yy + 1, z];

        for (int x = 0; x < Width; x++)
            cells[x, Height - 1, z] = false;
    }
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