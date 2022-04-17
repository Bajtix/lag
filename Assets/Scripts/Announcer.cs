using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Announcer : Singleton<Announcer> {
    private AudioSource m_source;

    private void Start() {
        m_source = GetComponent<AudioSource>();
    }

    public AudioClip[] clips;

    public void PlayClip(string clip) {
        GetComponent<AudioSource>().PlayOneShot(clips.Where(c => c.name == clip).First());
    }

    public static void Play(string clip) => Instance.PlayClip(clip);

    public static void PlayRandom(string key) {
        var clips = Instance.clips.Where(c => c.name.Contains(key));
        Play(clips.ElementAt(Random.Range(0, clips.Count())).name); // dumb, should work
    }
}
