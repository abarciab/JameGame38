using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventCoordinator : MonoBehaviour
{
    public UnityEvent Event1;

    public void TriggerEvent1()
    {
        Event1.Invoke();
    }
}
