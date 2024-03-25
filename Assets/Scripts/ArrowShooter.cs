using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowShooter : MonoBehaviour
{
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private float _shootResetTime;
    [SerializeField] private float _shootSpeed;
    [SerializeField] private Vector2 _shootOffset;
    private float _shootCooldown;

    private void Update()
    {
        _shootCooldown -= Time.deltaTime;
        if (_shootCooldown <= 0) Shoot();
    }

    private void Shoot()
    {
        _shootCooldown = _shootResetTime;

        var arrowPos = transform.TransformPoint(_shootOffset);
        Vector3 dir = arrowPos - transform.position;

        Quaternion arrowRot = Quaternion.LookRotation(-dir, Vector3.up);
        var arrowEuler = arrowRot.eulerAngles - new Vector3(0, 90, 0);
        arrowRot = Quaternion.Euler(arrowEuler);

        var arrow = Instantiate(_arrowPrefab, arrowPos, arrowRot);
        arrow.GetComponent<Rigidbody2D>().velocity = dir.normalized * _shootSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.TransformPoint(_shootOffset), 0.5f);
    }
}
