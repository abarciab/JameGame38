using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] private float _speed;

    private void Update()
    {
        transform.localEulerAngles += Vector3.forward * _speed * Time.deltaTime;
    }
}
