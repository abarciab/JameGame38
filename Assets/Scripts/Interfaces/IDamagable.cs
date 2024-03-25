using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable 
{
    void Damage(float amount);
    void Damage(float amount, Transform source);

    public bool AcceptDirectHits();
}
