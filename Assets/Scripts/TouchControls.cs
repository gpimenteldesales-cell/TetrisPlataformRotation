using UnityEngine;
using UnityEngine.InputSystem;

// Controles por toque para o PieceController.
//   Arrastar para os lados ... move a peça (uma casa a cada "moveStep" da largura da tela)
//   Toque rápido ............. gira a peça
//   Deslizar para baixo ...... queda instantânea
//   Segurar parado ........... queda suave enquanto o dedo estiver na tela
// Também funciona com o mouse no Editor (Pointer.current cobre mouse e toque).
public class TouchControls : MonoBehaviour
{
    [SerializeField] private PieceController pieceController;

    [Header("Gestos (em % da tela)")]
    [Tooltip("Arraste horizontal, em % da largura, para mover 1 casa.")]
    [SerializeField, Range(0.03f, 0.25f)] private float moveStep = 0.08f;
    [Tooltip("Deslize vertical, em % da altura, para a queda instantânea.")]
    [SerializeField, Range(0.05f, 0.5f)] private float dropSwipe = 0.12f;
    [Tooltip("Quanto o dedo pode andar (em % da largura) e ainda contar como toque.")]
    [SerializeField, Range(0.005f, 0.08f)] private float tapMaxMove = 0.03f;

    [Header("Tempos (segundos)")]
    [SerializeField] private float tapMaxTime = 0.25f;
    [SerializeField] private float holdTime = 0.35f;

    private bool tracking;
    private bool moved;
    private bool holding;
    private bool ignoreRest;
    private Vector2 startPos;
    private float anchorX;
    private float startTime;

    private void Update()
    {
        Pointer pointer = Pointer.current;
        if (pointer == null || pieceController == null) return;

        Vector2 pos = pointer.position.ReadValue();

        if (pointer.press.wasPressedThisFrame)
        {
            tracking = true;
            moved = false;
            holding = false;
            ignoreRest = false;
            startPos = pos;
            anchorX = pos.x;
            startTime = Time.time;
        }

        if (!tracking) return;

        if (pointer.press.isPressed && !ignoreRest)
        {
            float w = Screen.width;
            float h = Screen.height;
            Vector2 delta = pos - startPos;

            if (!moved && delta.magnitude > tapMaxMove * w) moved = true;

            // Mover para os lados (várias casas se o dedo andar bastante).
            float step = moveStep * w;
            while (pos.x - anchorX >= step) { pieceController.MoveRight(); anchorX += step; }
            while (anchorX - pos.x >= step) { pieceController.MoveLeft(); anchorX -= step; }

            // Deslizar para baixo: queda instantânea.
            if (-delta.y >= dropSwipe * h && Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
            {
                pieceController.HardDrop();
                ignoreRest = true; // ignora o resto do gesto (a próxima peça só nasce depois do giro)
                pieceController.SoftDrop = false;
                holding = false;
            }
            // Segurar parado: queda suave.
            else if (!moved && !holding && Time.time - startTime >= holdTime)
            {
                holding = true;
                pieceController.SoftDrop = true;
            }
        }

        if (pointer.press.wasReleasedThisFrame)
        {
            bool isTap = !moved && !holding && !ignoreRest && Time.time - startTime <= tapMaxTime;
            if (isTap) pieceController.Rotate(true);

            pieceController.SoftDrop = false;
            holding = false;
            tracking = false;
        }
    }

    private void OnDisable()
    {
        if (pieceController != null) pieceController.SoftDrop = false;
        tracking = false;
    }
}
