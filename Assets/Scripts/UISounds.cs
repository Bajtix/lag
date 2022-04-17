using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UISounds : MonoBehaviour {
    public AudioClip[] clips;

    public void Play(string clip) {
        GetComponent<AudioSource>().PlayOneShot(clips.Where(c => c.name == clip).First());
    }
}
