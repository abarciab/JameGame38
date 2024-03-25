using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaCamera : MonoBehaviour
{
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _fovTarget = 105;
    CameraController _camera;

    private void Start()
    {
         _camera = FindObjectOfType<CameraController>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStats>()) _camera.EnterArenaMode(_cameraTarget, _fovTarget);
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStats>()) _camera.ExitArenaMode(_cameraTarget);
    }
}

