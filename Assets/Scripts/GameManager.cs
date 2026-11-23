using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Tooltip("Na ordem: Lado1, Lado2, Lado3, Lado4")]
    [SerializeField] private GridManager[] grids = new GridManager[4];
    [SerializeField] private PieceController pieceController;
    [SerializeField] private RotationManager rotationManager;
    [SerializeField] private LayerChecker layerChecker;
    [SerializeField] private SideStateManager sideStateManager;
    [Tooltip("Opcional: se ficar vazio, o jogo roda sem som.")]
    [SerializeField] private AudioManager audioManager;

    [Header("Interface")]
    [Tooltip("Painel mostrado quando todos os lados acabam (pode ficar vazio).")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Tempo")]
    [SerializeField] private float clearDelay = 0.3f;
    [SerializeField] private float deadSideDelay = 0.6f;

    private void OnEnable()
    {
        pieceController.PieceLocked += OnPieceLocked;
        pieceController.SpawnBlocked += OnSpawnBlocked;
        rotationManager.RotationFinished += OnRotationFinished;
    }

    private void OnDisable()
    {
        pieceController.PieceLocked -= OnPieceLocked;
        pieceController.SpawnBlocked -= OnSpawnBlocked;
        rotationManager.RotationFinished -= OnRotationFinished;
    }

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (audioManager != null) audioManager.PlayGameplayMusic();
        pieceController.BeginPiece(grids[rotationManager.ActiveIndex]);
    }

    private void Update()
    {
        // Atalho para testar no Editor: R reinicia a partida.
        Keyboard kb = Keyboard.current;
        if (kb != null && kb.rKey.wasPressedThisFrame) RestartGame();
    }

    // Ligue o botão "Reiniciar" a este método (OnClick).
    public void RestartGame()
    {
        StopAllCoroutines();                 // cancela AfterLock / AfterSideDied pendentes
        pieceController.CancelPiece();       // descarta a peça em queda, se houver
        rotationManager.ResetRotation();     // plataforma volta ao lado 1

        for (int i = 0; i < grids.Length; i++)
        {
            grids[i].Data.Clear();
            grids[i].GetComponent<GridBlockView>().ClearAll();
        }

        sideStateManager.ResetAll();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (audioManager != null)
        {
            audioManager.PlayButtonClick();
            audioManager.PlayGameplayMusic();
        }

        pieceController.BeginPiece(grids[0]);
    }

    private void OnPieceLocked(GridManager grid)
    {
        if (audioManager != null) audioManager.PlayPieceLanded();
        StartCoroutine(AfterLock(grid));
    }

    private IEnumerator AfterLock(GridManager grid)
    {
        int cleared = layerChecker.CheckAndClear(grid);
        if (cleared > 0)
        {
            Debug.Log($"Linhas removidas: {cleared}");
            if (audioManager != null) audioManager.PlayLineClear();
            yield return new WaitForSeconds(clearDelay);
        }
        GoToNextAliveSide();
    }

    // A peça nova não coube: este lado acabou.
    private void OnSpawnBlocked(GridManager grid)
    {
        int side = System.Array.IndexOf(grids, grid);
        if (side < 0) return;

        Debug.Log($"Game Over no lado {side + 1}.");
        sideStateManager.ReportSpawnBlocked(side);
        StartCoroutine(AfterSideDied());
    }

    private IEnumerator AfterSideDied()
    {
        yield return new WaitForSeconds(deadSideDelay);
        GoToNextAliveSide();
    }

    // Gira para o próximo lado vivo, no sentido do giro, pulando os mortos.
    private void GoToNextAliveSide()
    {
        if (!sideStateManager.AnyAlive())
        {
            Debug.Log("GAME OVER: todos os lados acabaram.");
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            if (audioManager != null) audioManager.PlayMenuMusic();
            return;
        }

        int current = rotationManager.ActiveIndex;
        int dir = rotationManager.Direction;
        int count = grids.Length;

        for (int step = 1; step <= count; step++)
        {
            int idx = ((current + dir * step) % count + count) % count;
            if (!sideStateManager.IsAlive(idx)) continue;

            if (step == count)
                pieceController.BeginPiece(grids[current]); // só este lado resta: sem giro
            else
                rotationManager.RotateSteps(step);
            return;
        }
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