using UnityEngine;
using System.Collections.Generic;

public class CameraObstructionHandler : MonoBehaviour
{
    public Transform target; // O player
    public LayerMask obstructionMask;

    private List<Renderer> fadedObjects = new();
    private Dictionary<Renderer, Material[]> originalMaterials = new();

    void LateUpdate()
    {
        ClearObstructions();

        Vector3 direction = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(target.position, transform.position);

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, obstructionMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                if (!originalMaterials.ContainsKey(rend))
                    originalMaterials[rend] = rend.materials;

                SetObjectTransparent(rend);
                fadedObjects.Add(rend);
            }
        }
    }

    void SetObjectTransparent(Renderer rend)
    {
        foreach (Material mat in rend.materials)
        {
            mat.SetFloat("_Mode", 2); // Fade
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;

            Color color = mat.color;
            color.a = 0.3f;
            mat.color = color;
        }
    }

    void RestoreOriginal(Renderer rend)
    {
        if (originalMaterials.ContainsKey(rend))
        {
            Material[] mats = originalMaterials[rend];
            rend.materials = mats;
            originalMaterials.Remove(rend);
        }
    }

    void ClearObstructions()
    {
        foreach (Renderer r in fadedObjects)
        {
            RestoreOriginal(r);
        }

        fadedObjects.Clear();
    }
}
