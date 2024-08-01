using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class of cosmetics that swap particles (archery).
public class CosmeticParticle : Cosmetic
{
    public ParticleSystem particles;
    public CosmeticParticle (Cosmetic c, ParticleSystem particle)
        : base (c)
    { 
        particles = particle;
    }

    public CosmeticParticle(CosmeticParticle c)
        : base(c)
    {
        particles = c.particles;
    }
}
