using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float _parallaxAmount = 0.05f;
    private Transform _cam;
    private Vector3 _oldPos;

    private async void Start()
    {
        _cam = Camera.main.transform.parent;
        await Task.Yield();
        _oldPos = _cam.position;
    }

    private void Update()
    {
        var difference = _cam.position - _oldPos;
        var delta = Vector2.Lerp(Vector2.zero, (Vector2) difference, _parallaxAmount);
        transform.position += (Vector3)delta;
        _oldPos = _cam.position;
    }

}
