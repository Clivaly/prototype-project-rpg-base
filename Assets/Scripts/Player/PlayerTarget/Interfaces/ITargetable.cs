using UnityEngine;

public interface ITargetable
{
    Transform GetTransform();         // Posi��o do alvo
    string GetDisplayName();          // Nome pra mostrar na UI
    TargetType GetTargetType();       // Tipo (Enemy, Player)
    float GetCurrentHealth();         // Vida atual (0 se n�o usar)
    float GetMaxHealth();             // Vida m�xima (0 se n�o usar)
    void Select();                    // Feedback visual
    void Deselect();                  // Remove feedback
}

