using UnityEngine;
using UnityEngine.InputSystem;

public class PieceController : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Ordem: I, O, T, S, Z, J, L")]
    [SerializeField] private Material[] pieceMaterials = new Material[7];

    [Header("Velocidade")]
    [SerializeField] private float fallInterval = 0.8f;
    [SerializeField] private float softDropInterval = 0.05f;

    public event System.Action<GridManager> PieceLocked;

    private GridManager activeGrid;
    private GridBlockView view;
    private GridData Data => activeGrid.Data;

    private Vector3Int origin;
    private Vector2Int[] cells;
    private int shapeIndex;
    private readonly GameObject[] cubes = new GameObject[4];
    private float timer;
    private bool active;

    // Chamado pelo GameManager: define o lado e faz nascer a peça nele.
    public void BeginPiece(GridManager grid)
    {
        activeGrid = grid;
        view = grid.GetComponent<GridBlockView>();
        Spawn();
    }

    private void Update()
    {
        if (!active) return;
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame) TryMove(Vector3Int.left);
        if (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame) TryMove(Vector3Int.right);
        if (kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame) TryMove(new Vector3Int(0, 0, 1));
        if (kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame) TryMove(new Vector3Int(0, 0, -1));
        if (kb.eKey.wasPressedThisFrame) TryRotate(true);
        if (kb.qKey.wasPressedThisFrame) TryRotate(false);

        if (kb.spaceKey.wasPressedThisFrame)
        {
            while (TryMove(Vector3Int.down)) { }
            Lock();
            return;
        }

        timer += Time.deltaTime;
        float interval = kb.leftShiftKey.isPressed ? softDropInterval : fallInterval;
        if (timer >= interval)
        {
            timer = 0f;
            if (!TryMove(Vector3Int.down)) Lock();
        }
    }

    private void Spawn()
    {
        shapeIndex = Random.Range(0, PieceShapes.Count);
        cells = (Vector2Int[])PieceShapes.Cells[shapeIndex].Clone();
        origin = new Vector3Int(Data.Width / 2, Data.Height - 2, Data.Depth / 2);
        timer = 0f;

        if (!IsValid(origin, cells))
        {
            active = false;
            Debug.Log("Sem espaço para nova peça (fim de jogo neste lado).");
            return;
        }

        Material mat = shapeIndex < pieceMaterials.Length ? pieceMaterials[shapeIndex] : null;
        for (int i = 0; i < cubes.Length; i++)
            cubes[i] = view.CreateBlock(mat);

        active = true;
        Refresh();
    }

    private Vector3Int CellAt(int i, Vector3Int o, Vector2Int[] c)
    {
        return new Vector3Int(o.x + c[i].x, o.y + c[i].y, o.z);
    }

    private bool IsValid(Vector3Int o, Vector2Int[] c)
    {
        for (int i = 0; i < c.Length; i++)
            if (!Data.CanPlace(CellAt(i, o, c))) return false;
        return true;
    }

    private bool TryMove(Vector3Int delta)
    {
        Vector3Int newOrigin = origin + delta;
        if (!IsValid(newOrigin, cells)) return false;
        origin = newOrigin;
        Refresh();
        return true;
    }

    private void TryRotate(bool clockwise)
    {
        if (shapeIndex == PieceShapes.OIndex) return;

        Vector2Int[] rotated = new Vector2Int[cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            Vector2Int c = cells[i];
            rotated[i] = clockwise ? new Vector2Int(c.y, -c.x) : new Vector2Int(-c.y, c.x);
        }

        int[] kicks = { 0, -1, 1 };
        foreach (int kick in kicks)
        {
            Vector3Int o = origin + new Vector3Int(kick, 0, 0);
            if (IsValid(o, rotated))
            {
                origin = o;
                cells = rotated;
                Refresh();
                return;
            }
        }
    }

    private void Lock()
    {
        active = false; // espera a plataforma girar

        for (int i = 0; i < cubes.Length; i++)
        {
            Vector3Int cell = CellAt(i, origin, cells);
            Data.Occupy(cell);
            view.RegisterBlock(cell, cubes[i]);
        }

        PieceLocked?.Invoke(activeGrid);
    }

    private void Refresh()
    {
        for (int i = 0; i < cubes.Length; i++)
            view.SetBlockCell(cubes[i], CellAt(i, origin, cells));
    }
}