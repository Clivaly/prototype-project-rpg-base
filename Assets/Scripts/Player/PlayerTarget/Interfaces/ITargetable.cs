using UnityEngine;

public interface ITargetable
{
    Transform GetTransform();         // Posição do alvo
    string GetDisplayName();          // Nome pra mostrar na UI
    TargetType GetTargetType();       // Tipo (Enemy, Player)
    float GetCurrentHealth();         // Vida atual (0 se não usar)
    float GetMaxHealth();             // Vida máxima (0 se não usar)
    void Select();                    // Feedback visual
    void Deselect();                  // Remove feedback
}

