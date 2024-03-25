using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent OnTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStats>() != null) Activate();
    }

    private void Activate()
    {
        OnTrigger.Invoke();
        Destroy(gameObject);
    }
}
