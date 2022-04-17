using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/SFX", fileName = "New Sound")]
public class SoundEffect : ScriptableObject {
    public string subtitleName = "New Sound";
    public float minDistance = 4, maxDistance = 500;
    public Vector2 volumeRange = new Vector2(1, 1);
    public float volumeMultiplier = 1;
    public Vector2 pitchRange = new Vector2(1, 1);
    public float delayRandomness = 0;
    public List<AudioClip> clips;

    public AudioClip Play(AudioSource src, float volume = 1) {
        if (clips.Count == 0) {
            Debug.LogWarning($"No sound effects in {subtitleName}");
        }
        var clip = clips[Random.Range(0, clips.Count)];

        src.minDistance = minDistance;
        src.maxDistance = maxDistance;

        src.rolloffMode = AudioRolloffMode.Linear;
        src.spatialBlend = 1;
        src.pitch = Random.Range(pitchRange.x, pitchRange.y);
        src.volume = Random.Range(volumeRange.x, volumeRange.y) * volume * volumeMultiplier;
        src.clip = clip;
        src.playOnAwake = false;


        src.PlayDelayed(Random.Range(0, delayRandomness));

        return clip;
    }

    public AudioClip Play(Vector3 position, float volume = 1) {
        var o = new GameObject(subtitleName).AddComponent<AudioSource>();
        o.transform.position = position;
        var clip = Play(o, volume);
        Destroy(o.gameObject, clip.length + 0.3f);
        return clip;
    }
}
