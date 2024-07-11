using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterials : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material[] newMaterials = LanguageHoopsManager.shared.basketballColors.ToArray();

        if (renderer != null)
        {
            Material[] currentMaterials = renderer.materials;

            // Create a new materials array with the same length as the current materials array
            Material[] updatedMaterials = new Material[currentMaterials.Length];

            for (int i = 0; i < updatedMaterials.Length; i++)
            {
                if (i < newMaterials.Length)
                {
                    // Use new material if available
                    updatedMaterials[i] = newMaterials[i];
                }
                else
                {
                    // Use original material if new material is not available
                    updatedMaterials[i] = currentMaterials[i];
                }
            }

            // Assign the updated materials to the renderer
            renderer.materials = updatedMaterials;
        }
    }
}
