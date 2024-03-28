using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertVisiblity : MonoBehaviour
{
    [SerializeField] private Renderer _lead;
    [SerializeField] private Renderer _follower;

    [SerializeField] private SpriteRenderer _leadFlipper;

    void Update()
    {
        _follower.enabled = !_lead.gameObject.activeInHierarchy;
        transform.localEulerAngles = _leadFlipper.flipX ? new Vector3(0, 180, 0) : Vector3.zero;
    }
}
