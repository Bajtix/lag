using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour {
    [SerializeField] private Transform m_carryPoint;
    private DelayArtifact m_carrying;

    public void Bind(DelayArtifact artifact) {
        if (m_carrying != null) return;

        m_carrying = artifact;
        m_carrying.GetComponent<Rigidbody>().isKinematic = true;
        artifact.transform.SetParent(m_carryPoint);
        artifact.transform.localPosition = Vector3.zero;
        artifact.transform.localRotation = Quaternion.identity;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Release();
        }
    }

    public void Release() {
        if (m_carrying == null) return;
        m_carrying.transform.SetParent(null);
        m_carrying.GetComponent<Rigidbody>().isKinematic = false;
        m_carrying.GetComponent<Rigidbody>().AddForce(transform.forward * 30, ForceMode.Impulse);
        m_carrying = null;

    }
}
