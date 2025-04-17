using UnityEngine;
using System.Collections.Generic;

public class CameraObstructionHandler : MonoBehaviour
{
    public Transform target; // O player (pode ser o Player ou PlayerRoot, depende de onde você quer mirar)
    public LayerMask obstructionMask; // Quais objetos podem ficar transparentes

    private List<Renderer> fadedObjects = new(); // Lista dos objetos que estão transparentes
    private Dictionary<Renderer, Material[]> originalMaterials = new(); // Armazena os materiais originais

    void LateUpdate()
    {
        ClearObstructions(); // Restaura os objetos que estavam transparentes na última frame

        // Direção da câmera até o player
        Vector3 direction = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(target.position, transform.position);

        // Detecta todos os objetos entre a câmera e o player
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, obstructionMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                // Só salva os materiais uma vez (para restaurar depois)
                if (!originalMaterials.ContainsKey(rend))
                    originalMaterials[rend] = rend.sharedMaterials; // <-- sharedMaterials aqui!

                SetObjectTransparent(rend); // Aplica transparência
                fadedObjects.Add(rend);     // Adiciona à lista pra restaurar depois
            }
        }
    }

    void SetObjectTransparent(Renderer rend)
    {
        // Cria uma nova cópia dos materiais para não afetar outros objetos
        Material[] transparentMats = new Material[rend.materials.Length];

        for (int i = 0; i < rend.materials.Length; i++)
        {
            // Cria um novo material baseado no material original
            transparentMats[i] = new Material(rend.materials[i]);

            // Configura o material para modo transparente
            Material mat = transparentMats[i];
            mat.SetFloat("_Mode", 2); // Fade
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;

            Color color = mat.color;
            color.a = 0.3f; // Define a transparência
            mat.color = color;
        }

        // Aplica os novos materiais ao objeto
        rend.materials = transparentMats;
    }

    void RestoreOriginal(Renderer rend)
    {
        if (originalMaterials.ContainsKey(rend))
        {
            // Restaura os materiais salvos
            rend.materials = originalMaterials[rend];
            originalMaterials.Remove(rend);
        }
    }

    void ClearObstructions()
    {
        // Restaura todos os objetos que estavam transparentes
        foreach (Renderer r in fadedObjects)
        {
            RestoreOriginal(r);
        }

        // Limpa a lista pra próxima frame
        fadedObjects.Clear();
    }
}
