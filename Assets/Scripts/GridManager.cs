using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Dimensões (valores de teste)")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 20;
    [SerializeField] private int depth = 1;
    [SerializeField] private float cellSize = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    public GridData Data { get; private set; }
    public float CellSize => cellSize;

    private void Awake()
    {
        Data = new GridData(width, height, depth);
    }

    // Converte célula do grid em posição de mundo (centro da célula).
    public Vector3 CellToWorld(Vector3Int cell)
    {
        Vector3 local = new Vector3(cell.x + 0.5f, cell.y + 0.5f, cell.z + 0.5f) * cellSize;
        return transform.TransformPoint(local);
    }

    // Teste rápido: clique com o botão direito no componente > "Testar Grid".
    [ContextMenu("Testar Grid")]
    private void TestGrid()
    {
        var test = new GridData(width, height, depth);
        Debug.Log($"Ocupar (0,0,0): {test.Occupy(new Vector3Int(0, 0, 0))}");   // True
        Debug.Log($"Ocupar (0,0,0) de novo: {test.Occupy(new Vector3Int(0, 0, 0))}"); // False
        Debug.Log($"Fora do limite (-1,0,0) ocupado? {test.IsOccupied(new Vector3Int(-1, 0, 0))}"); // True
        test.Free(new Vector3Int(0, 0, 0));
        Debug.Log($"Após liberar, pode ocupar? {test.CanPlace(new Vector3Int(0, 0, 0))}"); // True
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Gizmos.color = Color.cyan;
        Vector3 size = new Vector3(width, height, depth) * cellSize;
        Vector3 center = transform.TransformPoint(size * 0.5f);
        Gizmos.DrawWireCube(center, size);

        if (Data == null) return;
        Gizmos.color = Color.red;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < depth; z++)
                {
                    var c = new Vector3Int(x, y, z);
                    if (Data.IsInside(c) && Data.IsOccupied(c))
                        Gizmos.DrawCube(CellToWorld(c), Vector3.one * cellSize * 0.9f);
                }
    }
}