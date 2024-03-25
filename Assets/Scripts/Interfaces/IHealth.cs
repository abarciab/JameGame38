using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IHealth
{
    public float HealthPercent();

    public UnityEvent<float> GetOnHealthChange();
}
