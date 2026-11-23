using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controla quais lados da plataforma ainda estão em jogo.
/// Não depende de grid nem de peça: só guarda estado e responde perguntas.
/// </summary>
public class SideStateManager : MonoBehaviour
{
    public const int SideCount = 4;

    public UnityEvent<int> OnSideGameOver;   // envia o índice do lado (0 a 3)
    public UnityEvent OnAllSidesGameOver;

    private readonly bool[] isDead = new bool[SideCount];

    public bool IsAlive(int side) => !isDead[side];

    public bool AnyAlive()
    {
        for (int i = 0; i < SideCount; i++)
            if (!isDead[i]) return true;
        return false;
    }

    /// <summary>Chamado quando a peça nova não consegue nascer neste lado.</summary>
    public void ReportSpawnBlocked(int side)
    {
        if (isDead[side]) return;

        isDead[side] = true;
        OnSideGameOver?.Invoke(side);

        if (!AnyAlive())
            OnAllSidesGameOver?.Invoke();
    }

    /// <summary>Próximo lado vivo depois de 'from', na ordem de giro. Retorna -1 se não houver.</summary>
    public int GetNextAliveSide(int from)
    {
        for (int i = 1; i <= SideCount; i++)
        {
            int candidate = (from + i) % SideCount;
            if (!isDead[candidate]) return candidate;
        }
        return -1;
    }

    public void ResetAll()
    {
        for (int i = 0; i < SideCount; i++) isDead[i] = false;
    }
}