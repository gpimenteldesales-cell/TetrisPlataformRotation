using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Dimensões (valores de teste)")]
    [SerializeField] private int width = 7;
    [SerializeField] private int height = 10;
    [SerializeField] private int depth = 3;
    [SerializeField] private float cellSize = 1f;

    [Header("Orientação")]
    [Tooltip("Se ligado, a profundidade (Z) cresce para fora da face do cubo.")]
    [SerializeField] private bool depthOutward = true;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private bool drawOccupiedGizmos = false;

    public GridData Data { get; private set; }
    public float CellSize => cellSize;

    private void Awake()
    {
        Data = new GridData(width, height, depth);
    }

    // Posição local (relativa a este grid) do centro da célula.
    public Vector3 CellToLocal(Vector3Int cell)
    {
        float zSign = depthOutward ? -1f : 1f;
        return new Vector3(
            (cell.x + 0.5f) * cellSize,
            (cell.y + 0.5f) * cellSize,
            (cell.z + 0.5f) * cellSize * zSign);
    }

    public Vector3 CellToWorld(Vector3Int cell)
    {
        return transform.TransformPoint(CellToLocal(cell));
    }

    [ContextMenu("Testar Grid")]
    private void TestGrid()
    {
        var test = new GridData(width, height, depth);
        Debug.Log($"Ocupar (0,0,0): {test.Occupy(new Vector3Int(0, 0, 0))}");
        Debug.Log($"Ocupar (0,0,0) de novo: {test.Occupy(new Vector3Int(0, 0, 0))}");
        Debug.Log($"Fora do limite (-1,0,0) ocupado? {test.IsOccupied(new Vector3Int(-1, 0, 0))}");
        test.Free(new Vector3Int(0, 0, 0));
        Debug.Log($"Após liberar, pode ocupar? {test.CanPlace(new Vector3Int(0, 0, 0))}");
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        float zSign = depthOutward ? -1f : 1f;
        Vector3 size = new Vector3(width, height, depth) * cellSize;
        Vector3 localCenter = new Vector3(size.x * 0.5f, size.y * 0.5f, size.z * 0.5f * zSign);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.TransformPoint(localCenter), size);

        if (!drawOccupiedGizmos || Data == null) return;
        Gizmos.color = Color.red;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < depth; z++)
                {
                    var c = new Vector3Int(x, y, z);
                    if (Data.IsOccupied(c))
                        Gizmos.DrawCube(CellToWorld(c), Vector3.one * cellSize * 0.9f);
                }
    }
}