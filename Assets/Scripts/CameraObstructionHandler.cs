using UnityEngine;
using System.Collections.Generic;

public class CameraObstructionHandler : MonoBehaviour
{
    public Transform target; // O player (pode ser o Player ou PlayerRoot, depende de onde voc� quer mirar)
    public LayerMask obstructionMask; // Quais objetos podem ficar transparentes

    private List<Renderer> fadedObjects = new(); // Lista dos objetos que est�o transparentes
    private Dictionary<Renderer, Material[]> originalMaterials = new(); // Armazena os materiais originais

    void LateUpdate()
    {
        ClearObstructions(); // Restaura os objetos que estavam transparentes na �ltima frame

        // Dire��o da c�mera at� o player
        Vector3 direction = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(target.position, transform.position);

        // Detecta todos os objetos entre a c�mera e o player
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, obstructionMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                // S� salva os materiais uma vez (para restaurar depois)
                if (!originalMaterials.ContainsKey(rend))
                    originalMaterials[rend] = rend.sharedMaterials; // <-- sharedMaterials aqui!

                SetObjectTransparent(rend); // Aplica transpar�ncia
                fadedObjects.Add(rend);     // Adiciona � lista pra restaurar depois
            }
        }
    }

    void SetObjectTransparent(Renderer rend)
    {
        // Cria uma nova c�pia dos materiais para n�o afetar outros objetos
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
            color.a = 0.3f; // Define a transpar�ncia
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

        // Limpa a lista pra pr�xima frame
        fadedObjects.Clear();
    }
}
