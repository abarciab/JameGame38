using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCoordinator : MonoBehaviour
{
    List<KeyValuePair<Sound, AudioSource>> sources = new List<KeyValuePair<Sound, AudioSource>>();
    List<AudioSource> pausedSources = new List<AudioSource>();

    public void AddNewSound(Sound sound, bool restart, bool _3D = true)
    {
        var newSource = gameObject.AddComponent<AudioSource>();
        newSource.outputAudioMixerGroup = AudioManager.i.GetMixer(sound.type);
        newSource.playOnAwake = false;
        if (_3D) newSource.spatialBlend = 1;
        sound.audioSource = newSource;
        sound.Play(transform.parent, restart);

        var newPair = new KeyValuePair<Sound, AudioSource>(sound, newSource);
        sources.Add(newPair);
    }

    public void Pause()
    {
        foreach (var s in sources) {
            if (!s.Key.unpauseable && s.Value.isPlaying) {
                s.Value.Pause();
                pausedSources.Add(s.Value);
            }
        }
    }

    public void Resume()
    {
        foreach (var s in pausedSources) s.Play();
        pausedSources.Clear();
    }

}
