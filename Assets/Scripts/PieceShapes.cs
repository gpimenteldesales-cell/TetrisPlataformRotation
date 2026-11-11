using UnityEngine;

public static class PieceShapes
{
    // Ordem: 0=I, 1=O, 2=T, 3=S, 4=Z, 5=J, 6=L
    public static readonly Vector2Int[][] Cells =
    {
        new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0) },   // I
        new[] { new Vector2Int(0,0),  new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1) },   // O
        new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1) },   // T
        new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(1,1) },   // S
        new[] { new Vector2Int(-1,1), new Vector2Int(0,1), new Vector2Int(0,0), new Vector2Int(1,0) },   // Z
        new[] { new Vector2Int(-1,1), new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0) },  // J
        new[] { new Vector2Int(1,1),  new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0) },  // L
    };

    public const int Count = 7;
    public const int OIndex = 1; // o quadrado não gira
}   