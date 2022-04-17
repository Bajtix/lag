using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour {
    public SoundEffect sfx;
    public float volume = 1;
    public void Play() {
        sfx.Play(transform.position, volume);
    }
}
