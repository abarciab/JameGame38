using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobUpAndDown : MonoBehaviour
{
    [SerializeField] private float _amplitude = 0.005f;
    [SerializeField] private float _frequency = 5;

    private void Start()
    {
        _amplitude += Random.Range(-0.002f, 0.002f);
        _frequency += Random.Range(-1, 1);
    }

    void Update()
    {
        transform.position += Vector3.up * Mathf.Sin(Time.time * _frequency) * _amplitude;
    }
}
