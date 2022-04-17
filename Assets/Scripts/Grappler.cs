using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : MonoBehaviour {


    [SerializeField] private float m_spring = 4.5f, m_damp = 7f;
    [SerializeField] private LayerMask m_mask;
    [SerializeField] private Rigidbody m_playerBody;
    [SerializeField] private LineRenderer m_lineRenderer;
    [SerializeField] private Transform m_lineStart, m_lineEnd;

    private SpringJoint m_joint;
    private Transform m_attachedTransform;
    private Vector3 m_ropePoint;

    public bool grappling = false;


    private void Update() {

        if (!gameObject.activeInHierarchy) return;

        if (Input.GetMouseButtonDown(0)) {
            Grapple();
        }
        if (Input.GetMouseButtonUp(0)) {
            Ungrapple();
        }

        if (grappling) {
            m_lineRenderer.enabled = true;
            m_lineRenderer.SetPosition(0, m_lineEnd.position);
            m_lineRenderer.SetPosition(1, m_lineStart.position);
            m_lineRenderer.SetPosition(2, m_joint.connectedAnchor);


            m_joint.connectedAnchor = m_attachedTransform.TransformPoint(m_ropePoint);
        } else {
            m_lineRenderer.enabled = false;
        }
    }

    public void Grapple() {
        if (grappling) return;

        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20, m_mask)) return;

        m_joint = m_playerBody.gameObject.AddComponent<SpringJoint>();
        m_joint.autoConfigureConnectedAnchor = false;

        m_attachedTransform = hit.transform;
        m_ropePoint = m_attachedTransform.InverseTransformPoint(hit.point);
        m_joint.connectedAnchor = hit.point;


        float v = Vector3.Distance(transform.position, hit.point);
        m_joint.spring = m_spring;
        m_joint.damper = m_damp;
        m_joint.maxDistance = v * 0.8f;
        m_joint.minDistance = v * 0.2f; // based on code by Dani

        PlayerController.Instance.sfx.Grapple();

        grappling = true;
    }



    public void Ungrapple() {
        if (!grappling) return;

        Destroy(m_joint);

        grappling = false;
        m_attachedTransform = null;
    }
}
