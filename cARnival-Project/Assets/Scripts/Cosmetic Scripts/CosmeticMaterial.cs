using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticMaterial : Cosmetic
{
    public Material[] materials;
    public CosmeticMaterial(Cosmetic c, Material[] m)
        : base(c)
    {
        materials = m;
    }

    public CosmeticMaterial(CosmeticMaterial c)
    : base(c)
    {
        materials = c.materials;
    }

}
