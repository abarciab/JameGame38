using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventCoordinator : MonoBehaviour
{
    public UnityEvent Event1;
    [SerializeField] private Sound _sound1;

    private void Start()
    {
        if(_sound1) _sound1 = Instantiate(_sound1);
    }

    public void TriggerEvent1()
    {
        Event1.Invoke();
    }

    public void PlaySound1()
    {
        _sound1.Play();
    }
}
