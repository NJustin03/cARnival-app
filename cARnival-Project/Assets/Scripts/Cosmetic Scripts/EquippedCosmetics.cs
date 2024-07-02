using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedCosmetics : MonoBehaviour
{
    public static CosmeticMaterial duckMaterial;
    public static CosmeticMaterial basketballMaterial;
    public static CosmeticParticle archeryParticle;

    public static void EquipCosmetic(Cosmetic selectedCosmetic)
    {
        if (selectedCosmetic.game == "duck")
        {
            duckMaterial = (CosmeticMaterial)selectedCosmetic;
        }
        else if (selectedCosmetic.game == "basketball")
        {
            basketballMaterial = (CosmeticMaterial)selectedCosmetic;
        }
        else if (selectedCosmetic.game == "archery")
        {
            archeryParticle = (CosmeticParticle)selectedCosmetic;
        }
        else
        {
            return;
        }
    }

    public static void EquipDefaults()
    {
        
    }
}
