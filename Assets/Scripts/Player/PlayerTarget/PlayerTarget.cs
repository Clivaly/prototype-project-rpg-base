using UnityEngine;

public class PlayerTarget : MonoBehaviour, ITargetable
{
    //[Header("Feedback visual")]
    private GameObject selectionCircle;

    [Header("Nome do Jogador")]
    public string playerName;

    void Awake()
    {
        // Define nome do jogador se não foi setado manualmente
        if (string.IsNullOrEmpty(playerName))
            playerName = gameObject.name;

        // Busca automática pelo SelectionCircleMarker
        if (selectionCircle == null)
        {
            // Procura entre todos os filhos, até mesmo os desativados
            SelectionCircleMarker[] markers = GetComponentsInChildren<SelectionCircleMarker>(true);

            if (markers.Length > 0)
            {
                selectionCircle = markers[0].gameObject;
                Debug.Log($"[PlayerTarget] SelectionCircle encontrado: {selectionCircle.name}", this);
            }
            else
            {
                Debug.LogWarning("[PlayerTarget] SelectionCircleMarker não encontrado!", this);
            }
        }

    }

    public void Select()
    {
        Debug.Log("Jogador selecionado!");
        if (selectionCircle != null)
        {
            selectionCircle.SetActive(true);
            Debug.Log("Círculo ativado: " + selectionCircle.name);
        }
        else
        {
            Debug.LogWarning("selectionCircle é nulo!");
        }
    }

    //public void Select()
    //{
    //    // Seleciona e ignora o proprio Player
    //    if (this.CompareTag("Player"))
    //    {
    //        Debug.LogWarning("Tentou selecionar o próprio Player controlado. Ignorando.");
    //        return;
    //    }

    //    Debug.Log("Jogador selecionado!");
    //    if (selectionCircle != null)
    //        selectionCircle.SetActive(true);
    //}

    public void Deselect()
    {
        if (selectionCircle != null)
            selectionCircle.SetActive(false);
    }

    public Transform GetTransform() => transform;

    public string GetDisplayName() => playerName;

    public TargetType GetTargetType() => TargetType.Player;

    public float GetCurrentHealth() => 0; // Não mostra vida de players

    public float GetMaxHealth() => 0;
}
