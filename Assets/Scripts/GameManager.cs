using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("Na ordem: Lado1, Lado2, Lado3, Lado4")]
    [SerializeField] private GridManager[] grids = new GridManager[4];
    [SerializeField] private PieceController pieceController;
    [SerializeField] private RotationManager rotationManager;
    [SerializeField] private LayerChecker layerChecker;

    [Header("Tempo")]
    [SerializeField] private float clearDelay = 0.3f;

    private void OnEnable()
    {
        pieceController.PieceLocked += OnPieceLocked;
        rotationManager.RotationFinished += OnRotationFinished;
    }

    private void OnDisable()
    {
        pieceController.PieceLocked -= OnPieceLocked;
        rotationManager.RotationFinished -= OnRotationFinished;
    }

    private void Start()
    {
        pieceController.BeginPiece(grids[rotationManager.ActiveIndex]);
    }

    private void OnPieceLocked(GridManager grid)
    {
        StartCoroutine(AfterLock(grid));
    }

    private IEnumerator AfterLock(GridManager grid)
    {
        int cleared = layerChecker.CheckAndClear(grid);
        if (cleared > 0)
        {
            Debug.Log($"Linhas removidas: {cleared}");
            yield return new WaitForSeconds(clearDelay);
        }
        rotationManager.RotateToNext();
    }

    private void OnRotationFinished(int newIndex)
    {
        pieceController.BeginPiece(grids[newIndex]);
    }

    // TEMPORÁRIO, só para testar: com o jogo rodando, clique com o botão direito
    // no componente e escolha "Preencher linha de teste". Ocupa 6 das 7 células
    // da linha 0 do lado ativo, para você completar com uma peça.
    [ContextMenu("Preencher linha de teste")]
    private void FillTestRow()
    {
        if (!Application.isPlaying) return;
        GridManager grid = grids[rotationManager.ActiveIndex];
        GridBlockView view = grid.GetComponent<GridBlockView>();

        for (int x = 0; x < grid.Data.Width - 1; x++)
        {
            var cell = new Vector3Int(x, 0, 0);
            if (grid.Data.Occupy(cell))
                view.RegisterBlock(cell, view.CreateBlock(null));
        }
    }
}