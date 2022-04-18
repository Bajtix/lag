using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {
    private struct Part {
        public Rigidbody part;
        public Vector3 position;
        public Quaternion rotation;

        public Part(Rigidbody part, Vector3 position, Quaternion rotation) {
            this.part = part;
            this.position = position;
            this.rotation = rotation;
        }
    }

    private List<Part> m_parts = new List<Part>();
    public GameObject destroyedVersion, normalVersion;

    private void Start() {
        foreach (var g in destroyedVersion.transform.GetComponentsInChildren<Rigidbody>()) {
            g.isKinematic = true;
            m_parts.Add(new Part(g, g.transform.localPosition, g.transform.localRotation));
        }
    }

    public void Destroy() {
        destroyedVersion.SetActive(true);
        normalVersion.SetActive(false);
        foreach (var dp in m_parts) {
            dp.part.isKinematic = false;
            dp.part.AddExplosionForce(300, transform.position, 10);
        }
    }

    public void Reset() {

        foreach (var dp in m_parts) {
            dp.part.isKinematic = true;
            dp.part.transform.localPosition = dp.position;
            dp.part.transform.localRotation = dp.rotation;
        }
        destroyedVersion.SetActive(false);
        normalVersion.SetActive(true);
    }
}
