using UnityEngine;

public class LayerChecker : MonoBehaviour
{
    // Remove todas as linhas completas do grid e devolve quantas foram removidas.
    public int CheckAndClear(GridManager grid)
    {
        GridData data = grid.Data;
        GridBlockView view = grid.GetComponent<GridBlockView>();
        int cleared = 0;

        for (int z = 0; z < data.Depth; z++)
        {
            int y = 0;
            while (y < data.Height)
            {
                if (data.IsRowFull(y, z))
                {
                    view.RemoveRow(y, z);
                    data.RemoveRow(y, z);
                    cleared++;
                    // não avança y: a linha de cima desceu para esta posição
                }
                else
                {
                    y++;
                }
            }
        }

        return cleared;
    }
}