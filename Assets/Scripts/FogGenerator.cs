using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogGenerator : MonoBehaviour {
    [SerializeField]
    private int m_layers = 100;

    [SerializeField]
    private Material m_material;


    [SerializeField]
    private Mesh m_mesh;

    [ContextMenu("Generate")]
    public void Create() {

        for (int i = 0; i < m_layers; i++) {
            GameObject layer = new GameObject("Layer " + i);
            layer.transform.parent = transform;
            layer.transform.localPosition = new Vector3(0, i * (-1f / m_layers), 0);
            layer.transform.localScale = new Vector3(1, 1, 1);
            layer.AddComponent<MeshFilter>().mesh = m_mesh;
            layer.AddComponent<MeshRenderer>();
            layer.GetComponent<MeshRenderer>().material = m_material;
            layer.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            layer.GetComponent<MeshRenderer>().receiveShadows = false;
        }
    }
}
