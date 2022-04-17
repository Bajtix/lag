using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificPlatform : MonoBehaviour {

    public LevelStage m_stage;

    private Collider[] m_colliders;

    private void Start() {
        m_colliders = GetComponentsInChildren<Collider>();
        var n = GetComponent<Renderer>().sharedMaterial.name;

        switch (n) {
            case "Sprinter Grid":
                m_stage = LevelStage.Sprinter;
                break;
            case "Vidmo Grid":
                m_stage = LevelStage.Vidmo;
                break;
            default:
                m_stage = LevelStage.None;
                break;
        }
    }

    private void EnableColliders() {
        foreach (Collider collider in m_colliders) {
            collider.enabled = true;
        }
    }

    private void DisableColliders() {
        foreach (Collider collider in m_colliders) {
            collider.enabled = false;
        }
    }

    public void SetType(LevelStage s) {
        if (m_colliders == null || m_colliders.Length == 0) return;


        if (s == m_stage || m_stage == LevelStage.None) {
            EnableColliders();
        } else {
            DisableColliders();
        }
    }

}
