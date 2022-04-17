using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEntity : MonoBehaviour {

    public struct TimeEntityData {
        public Vector3 position;
        public Quaternion rotation;
        public object data;

        public TimeEntityData(Vector3 position, Quaternion rotation, object data) {
            this.position = position;
            this.rotation = rotation;
            this.data = data;
        }
    }

    public bool isRecording = true;
    public object data;

    private Dictionary<int, TimeEntityData> m_data = new Dictionary<int, TimeEntityData>();

    [SerializeField]
    private Component[] m_recordingOnlyComponents;

    public float playbackSpeed => TimeManager.Instance.playbackSpeed;

    private bool m_wasKinematic = false; //dumb fix


    public Dictionary<int, TimeEntityData> GetData() => new Dictionary<int, TimeEntityData>(m_data);

    private void FixedUpdate() {
        if (isRecording) {
            Record(TimeManager.Instance.tick);
            SetEnabled(m_recordingOnlyComponents, true);
        } else {
            Play((int)(playbackSpeed * TimeManager.Instance.tick));
            SetEnabled(m_recordingOnlyComponents, false);
        }
    }

    public void Delay(int tick, int delay) {
        var duplicated = m_data[tick];

        var tmpDictionary = new Dictionary<int, TimeEntityData>();
        foreach (var kv in m_data) {
            if (kv.Key > tick) {
                tmpDictionary.Add(kv.Key + delay, kv.Value);
            } else {
                tmpDictionary.Add(kv.Key, kv.Value);
            }
        }

        m_data = tmpDictionary;
    }

    public void DelaySeconds(int tick, float delay) {
        Delay(tick, (int)(delay / Time.fixedDeltaTime));
    }

    public void ResetData() {
        m_data.Clear();
    }

    private void SetEnabled(Component[] c, bool v) {
        foreach (Component component in c) {
            if (component is MonoBehaviour mb) mb.enabled = v;
            if (component is Collider col) col.enabled = v;
            if (component is Rigidbody rb) rb.isKinematic = !v || m_wasKinematic;
        }
    }

    private void Record(int t) {
        if (m_data.ContainsKey(t)) {
            m_data[t] = new TimeEntityData(transform.position, transform.rotation, data);
        } else {
            m_data.Add(t, new TimeEntityData(transform.position, transform.rotation, data));
        }
    }

    private void Update() {
        var t = (int)(playbackSpeed * TimeManager.Instance.tick);
        if (!isRecording) {
            if (m_data.ContainsKey(t)) {
                transform.position = Vector3.Lerp(transform.position, m_data[t].position, Time.deltaTime * 8);
                transform.rotation = Quaternion.Lerp(transform.rotation, m_data[t].rotation, Time.deltaTime * 8);
            }
        }
    }

    private void Play(int t) {
        if (m_data.ContainsKey(t)) {
            //transform.position = m_data[t].position;
            //transform.rotation = m_data[t].rotation;
            data = m_data[t].data;
        } else {
            //Debug.LogWarning($"No frame for tick {t}");
        }
    }

    private void Start() {
        if (GetComponent<Rigidbody>())
            m_wasKinematic = GetComponent<Rigidbody>().isKinematic;
    }

    public void SetData(Dictionary<int, TimeEntityData> newData) {
        Debug.Log($"{gameObject.name} got {newData.Count} frames");
        m_data = new Dictionary<int, TimeEntityData>(newData);
    }
}
