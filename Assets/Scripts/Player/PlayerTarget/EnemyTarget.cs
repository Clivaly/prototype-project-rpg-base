using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    [Header("Feedback visual")]
    public GameObject selectionCircle; // Arraste um c�rculo visual aqui

    public void Select()
    {
        Debug.Log("Selecionado!");
        if (selectionCircle != null)
            selectionCircle.SetActive(true); // Mostra o c�rculo
    }

    public void Deselect()
    {
        if (selectionCircle != null)
            selectionCircle.SetActive(false); // Esconde o c�rculo
    }
}
