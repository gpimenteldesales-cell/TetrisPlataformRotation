using System.Collections;
using UnityEngine;

public class RotationManager : MonoBehaviour
{
    [SerializeField] private Transform platform;
    [SerializeField] private int sideCount = 4;
    [SerializeField] private float rotationDuration = 0.6f;
    [Tooltip("+1 gira para um sentido, -1 para o outro.")]
    [SerializeField] private int direction = 1;

    public int ActiveIndex { get; private set; }
    public bool IsRotating { get; private set; }
    public int Direction => direction;

    // Disparado quando o giro termina, com o índice do novo lado ativo.
    public event System.Action<int> RotationFinished;

    private float currentAngle;

    public void RotateToNext()
    {
        RotateSteps(1);
    }

    // Gira 'steps' lados de uma vez (usado para pular lados que já acabaram).
    public void RotateSteps(int steps)
    {
        if (IsRotating || steps <= 0) return;
        StartCoroutine(RotateRoutine(steps));
    }

    // Usado no reinício: cancela qualquer giro e volta ao lado 1.
    public void ResetRotation()
    {
        StopAllCoroutines();
        IsRotating = false;
        currentAngle = 0f;
        ActiveIndex = 0;
        platform.rotation = Quaternion.identity;
    }

    private IEnumerator RotateRoutine(int steps)
    {
        IsRotating = true;

        float startAngle = currentAngle;
        float endAngle = startAngle + 90f * direction * steps;
        float duration = rotationDuration * (1f + 0.5f * (steps - 1));
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
            platform.rotation = Quaternion.Euler(0f, Mathf.Lerp(startAngle, endAngle, t), 0f);
            yield return null;
        }

        // Trava no ângulo exato para não acumular erro.
        currentAngle = endAngle;
        platform.rotation = Quaternion.Euler(0f, currentAngle, 0f);

        ActiveIndex = ((ActiveIndex + direction * steps) % sideCount + sideCount) % sideCount;
        IsRotating = false;
        RotationFinished?.Invoke(ActiveIndex);
    }
}