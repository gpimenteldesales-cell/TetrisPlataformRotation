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

    // Disparado quando o giro termina, com o índice do novo lado ativo.
    public event System.Action<int> RotationFinished;

    private float currentAngle;

    public void RotateToNext()
    {
        if (IsRotating) return;
        StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        IsRotating = true;

        float startAngle = currentAngle;
        float endAngle = startAngle + 90f * direction;
        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / rotationDuration));
            platform.rotation = Quaternion.Euler(0f, Mathf.Lerp(startAngle, endAngle, t), 0f);
            yield return null;
        }

        // Trava no ângulo exato para não acumular erro.
        currentAngle = endAngle;
        platform.rotation = Quaternion.Euler(0f, currentAngle, 0f);

        ActiveIndex = ((ActiveIndex + direction) % sideCount + sideCount) % sideCount;
        IsRotating = false;
        RotationFinished?.Invoke(ActiveIndex);
    }
}