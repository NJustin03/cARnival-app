using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CosmeticMaterial : Cosmetic
{
    public Material material;
    public CosmeticMaterial(Cosmetic c, Material m)
        : base(c)
    {
        material = m;
    }

    public CosmeticMaterial(CosmeticMaterial c)
    : base(c)
    {
        material = c.material;
    }

}
