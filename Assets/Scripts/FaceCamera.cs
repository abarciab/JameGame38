using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] private bool _yOnly = true;
    [SerializeField] private bool _invert = false;

    void Update()
    {
        Face();
    }

    [ButtonMethod]
    void Face()
    {
        var rot = transform.localEulerAngles;
        transform.LookAt(Camera.main.transform);
        if (!_yOnly) return;

        rot.y = transform.localEulerAngles.y + 180;
        if (_invert) rot.y += 180;
        transform.localEulerAngles = rot;
    }
}
