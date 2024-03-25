using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Sound testSound;

    private void Start()
    {
         testSound = Instantiate(testSound);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) testSound.Play();
    }

}
