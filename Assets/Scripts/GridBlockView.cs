using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridManager))]
public class GridBlockView : MonoBehaviour
{
    private GridManager grid;
    private readonly Dictionary<Vector3Int, GameObject> blocks = new Dictionary<Vector3Int, GameObject>();

    private void Awake()
    {
        grid = GetComponent<GridManager>();
    }

    public Vector3 CellToLocal(Vector3Int cell)
    {
        return grid.CellToLocal(cell);
    }

    public GameObject CreateBlock(Material material)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(block.GetComponent<Collider>()); // ocupação é lógica, não física
        block.transform.SetParent(transform, false);
        block.transform.localScale = Vector3.one * grid.CellSize * 0.95f;
        if (material != null) block.GetComponent<MeshRenderer>().sharedMaterial = material;
        return block;
    }

    public void SetBlockCell(GameObject block, Vector3Int cell)
    {
        block.transform.localPosition = CellToLocal(cell);
    }

    // Registra um bloco visual fixado em uma célula (usado depois para remover camadas).
    public void RegisterBlock(Vector3Int cell, GameObject block)
    {
        blocks[cell] = block;
        SetBlockCell(block, cell);
    }
    public void RemoveRow(int y, int z)
    {
        GridData data = grid.Data;

        for (int x = 0; x < data.Width; x++)
        {
            var cell = new Vector3Int(x, y, z);
            if (blocks.TryGetValue(cell, out GameObject block))
            {
                Destroy(block);
                blocks.Remove(cell);
            }
        }

        for (int yy = y + 1; yy < data.Height; yy++)
        {
            for (int x = 0; x < data.Width; x++)
            {
                var from = new Vector3Int(x, yy, z);
                if (blocks.TryGetValue(from, out GameObject block))
                {
                    var to = new Vector3Int(x, yy - 1, z);
                    blocks.Remove(from);
                    blocks[to] = block;
                    SetBlockCell(block, to);
                }
            }
        }
    }
}