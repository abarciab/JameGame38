using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTester : MonoBehaviour
{
    [SerializeField] Sound sound;
    Sound instanced;
    public bool play, _3D, restart = true, makeNew = false, makeNewAuto = true;
    private void Start()
    {
        MakeNew();
    }

    void MakeNew()
    {
        if (instanced) instanced.Delete();
        instanced = Instantiate(sound);
    }

    void PlaySound()
    {
        if (makeNewAuto) MakeNew();

        if (_3D) instanced.Play(transform, restart);
        else instanced.Play(restart: restart);
    }

    private void Update()
    {
        if (makeNew) {
            makeNew = false;
            MakeNew();
        }

        if (play) {
            play = false;
            PlaySound();
        }
    }
}
