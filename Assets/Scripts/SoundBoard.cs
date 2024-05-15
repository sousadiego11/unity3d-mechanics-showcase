using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBoard : MonoBehaviour {
    
    [SerializeField] List<Audio> audioClips;
    [SerializeField] AudioSource audioSource;

    public static SoundBoard Instance {get; private set;}

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    public void PlayOne(Audio.AudioEnum name, float volume) {
        Audio audio = audioClips.Find(a => a.name == name);
        Instance.audioSource.PlayOneShot(audio.audioClip, volume);
    }
}

[Serializable]
public struct Audio {
    public AudioClip audioClip;
    public AudioEnum name;
    public enum AudioEnum
    {
        MissileHitSFX,
        MissileMoveSFX,
        TurretShootSFX,
        BackgroundSFX
    }
}