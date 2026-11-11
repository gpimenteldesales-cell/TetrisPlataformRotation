using UnityEngine;

public class GridIndependenceTest : MonoBehaviour
{
    [SerializeField] private GridManager[] grids = new GridManager[4];

    // Awake de todos os GridManagers roda antes de qualquer Start,
    // então os dados já existem aqui.
    private void Start()
    {
        bool allPassed = true;

        // 1) Grid i ocupa a célula (i, 0, 0).
        for (int i = 0; i < grids.Length; i++)
            grids[i].Data.Occupy(new Vector3Int(i, 0, 0));

        // 2) Cada grid só pode ter ocupada a sua própria célula.
        allPassed &= CheckState("Após ocupar uma célula por grid");

        // 3) Limpar o grid 0 não pode afetar os outros.
        grids[0].Data.Clear();
        for (int j = 0; j < grids.Length; j++)
        {
            for (int i = 0; i < grids.Length; i++)
            {
                bool expected = (j != 0) && (i == j);
                bool actual = grids[j].Data.IsOccupied(new Vector3Int(i, 0, 0));
                if (actual != expected)
                {
                    Debug.LogError($"[FALHOU] Após Clear do grid 0: grid {j}, célula ({i},0,0) = {actual}, esperado {expected}");
                    allPassed = false;
                }
            }
        }

        Debug.Log(allPassed
            ? "<color=green>ETAPA 2: todos os testes passaram. Os 4 grids são independentes.</color>"
            : "<color=red>ETAPA 2: há falhas, veja os erros acima.</color>");
    }

    private bool CheckState(string label)
    {
        bool ok = true;
        for (int j = 0; j < grids.Length; j++)
        {
            for (int i = 0; i < grids.Length; i++)
            {
                bool expected = (i == j);
                bool actual = grids[j].Data.IsOccupied(new Vector3Int(i, 0, 0));
                if (actual != expected)
                {
                    Debug.LogError($"[FALHOU] {label}: grid {j}, célula ({i},0,0) = {actual}, esperado {expected}");
                    ok = false;
                }
            }
        }
        return ok;
    }
}