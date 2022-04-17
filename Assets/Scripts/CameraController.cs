using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float m_sensX, m_sensY, m_lookLimit = 90;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private PlayerController m_playerController;

    [SerializeField]
    private Transform m_normalPosition, m_slidingPosition;

    [SerializeField]
    private AnimationCurve m_cameraBob, m_fovBoost;
    [SerializeField]
    private float m_bobStrength = 0.01f;
    private float m_bobStep;


    [SerializeField]
    private SoundEffect m_stepSound;




    private void Start() {

        if (m_playerController == null) m_playerController = PlayerController.Instance;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        if (!gameObject.activeInHierarchy) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        player.Rotate(0, mouseX * m_sensX, 0);

        var cXr = transform.localEulerAngles.x > 180 ? transform.localEulerAngles.x - 360 : transform.localEulerAngles.x; // a hack without using a variable to store the angle
        cXr = Mathf.Clamp(cXr - mouseY * m_sensY, -m_lookLimit, m_lookLimit);
        transform.localRotation = Quaternion.Euler(cXr, 0, 0);




        // special stuff ✨

        float playerSpeed = m_playerController.playerBody.velocity.IgnoreY().magnitude;
        GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, m_fovBoost.Evaluate(playerSpeed) + 60, Time.deltaTime * 5);


        Vector3 desiredPos = m_playerController.CurrentState == PlayerController.State.Sliding ? m_slidingPosition.localPosition : m_normalPosition.localPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPos, Time.deltaTime * 5) + GetBobOffset() * Mathf.Clamp01(playerSpeed);

        if (m_playerController.CurrentState == PlayerController.State.Walking) {
            m_bobStep += Time.deltaTime * 0.5f * playerSpeed;
        } else {
            m_bobStep = 0;
        }
    }



    private Vector3 GetBobOffset() {
        return Vector3.up * m_cameraBob.Evaluate(m_bobStep) * m_bobStrength + Vector3.right * (m_cameraBob.Evaluate(m_bobStep + 13.6312f) - 0.3f) * m_bobStrength * 0.3f;
    }
}
