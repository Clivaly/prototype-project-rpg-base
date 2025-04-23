using UnityEngine;

public class PlayerTarget : MonoBehaviour, ITargetable
{
    //[Header("Feedback visual")]
    private GameObject selectionCircle;

    [Header("Nome do Jogador")]
    public string playerName;

    void Awake()
    {
        // Define nome do jogador se n�o foi setado manualmente
        if (string.IsNullOrEmpty(playerName))
            playerName = gameObject.name;

        // Busca autom�tica pelo SelectionCircleMarker
        if (selectionCircle == null)
        {
            // Procura entre todos os filhos, at� mesmo os desativados
            SelectionCircleMarker[] markers = GetComponentsInChildren<SelectionCircleMarker>(true);

            if (markers.Length > 0)
            {
                selectionCircle = markers[0].gameObject;
                Debug.Log($"[PlayerTarget] SelectionCircle encontrado: {selectionCircle.name}", this);
            }
            else
            {
                Debug.LogWarning("[PlayerTarget] SelectionCircleMarker n�o encontrado!", this);
            }
        }

    }

    public void Select()
    {
        Debug.Log("Jogador selecionado!");
        if (selectionCircle != null)
        {
            selectionCircle.SetActive(true);
            Debug.Log("C�rculo ativado: " + selectionCircle.name);
        }
        else
        {
            Debug.LogWarning("selectionCircle � nulo!");
        }
    }

    //public void Select()
    //{
    //    // Seleciona e ignora o proprio Player
    //    if (this.CompareTag("Player"))
    //    {
    //        Debug.LogWarning("Tentou selecionar o pr�prio Player controlado. Ignorando.");
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

    public float GetCurrentHealth() => 0; // N�o mostra vida de players

    public float GetMaxHealth() => 0;
}
