using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager> {
    public float time;
    public int tick => (int)m_tck;

    private bool m_paused = true;

    public bool Paused => m_paused;

    private float m_tck = 0;
    public float vidmoTimeScale = 0.5f;

    public float playbackSpeed => LevelManager.Instance.currentStageType == LevelStage.Vidmo ? vidmoTimeScale : 1;

    public void Unpause() {
        m_paused = false;
    }

    public void Pause() {
        m_paused = true;
    }

    private void Start() {
        time = 0;
    }

    private void Update() {
        if (!Paused)
            time += Time.deltaTime;
    }

    private void FixedUpdate() {
        if (!Paused)
            m_tck += playbackSpeed;
    }

    public void Reset() {
        time = 0;
        //tick = 0;
        m_tck = 0;
    }
}
