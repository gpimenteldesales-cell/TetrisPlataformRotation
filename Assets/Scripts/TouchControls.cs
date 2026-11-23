using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Lê gestos de toque e dispara eventos. Não conhece o PieceController:
/// a ligação é feita por UnityEvents, mantendo o código modular.
/// </summary>
public class TouchControls : MonoBehaviour
{
    [Header("Sensibilidade (em polegadas, independe da resolução)")]
    [SerializeField] private float stepInches = 0.3f;     // arrasto por célula
    [SerializeField] private float tapMaxInches = 0.1f;   // movimento máximo para contar como toque
    [SerializeField] private float tapMaxTime = 0.2f;
    [SerializeField] private float flickMinInches = 0.6f; // distância mínima do flick para baixo
    [SerializeField] private float flickMaxTime = 0.25f;

    [Header("Eventos")]
    public UnityEvent OnMoveLeft;
    public UnityEvent OnMoveRight;
    public UnityEvent<bool> OnSoftDropHeld;   // true ao começar a puxar para baixo, false ao soltar
    public UnityEvent OnRotate;
    public UnityEvent OnHardDrop;

    private Vector2 startPos, anchorPos;
    private float startTime;
    private bool moved;
    private bool softHeld;

    private float Dpi => Screen.dpi > 0 ? Screen.dpi : 160f;

    private void Update()
    {
        var screen = Touchscreen.current;
        if (screen == null) return;

        var touch = screen.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            startPos = anchorPos = touch.position.ReadValue();
            startTime = Time.unscaledTime;
            moved = false;
        }
        else if (touch.press.isPressed)
        {
            HandleDrag(touch.position.ReadValue());
        }
        else if (touch.press.wasReleasedThisFrame)
        {
            HandleRelease(touch.position.ReadValue());
        }
    }

    private void HandleDrag(Vector2 pos)
    {
        float step = stepInches * Dpi;
        Vector2 delta = pos - anchorPos;

        if (Mathf.Abs(delta.x) >= step && Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0) OnMoveRight?.Invoke(); else OnMoveLeft?.Invoke();
            anchorPos.x = pos.x;
            moved = true;
        }
        else if (-delta.y >= step && Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
        {
            if (!softHeld)
            {
                softHeld = true;
                OnSoftDropHeld?.Invoke(true);
            }
            anchorPos.y = pos.y;
            moved = true;
        }
    }

    private void HandleRelease(Vector2 endPos)
    {
        if (softHeld)
        {
            softHeld = false;
            OnSoftDropHeld?.Invoke(false);
        }

        Vector2 total = endPos - startPos;
        float time = Time.unscaledTime - startTime;
        float dpi = Dpi;

        bool isTap = !moved && total.magnitude < tapMaxInches * dpi && time < tapMaxTime;
        bool isFlickDown = -total.y > flickMinInches * dpi
                           && Mathf.Abs(total.y) > Mathf.Abs(total.x)
                           && time < flickMaxTime;

        if (isTap) OnRotate?.Invoke();
        else if (isFlickDown) OnHardDrop?.Invoke();
    }
}