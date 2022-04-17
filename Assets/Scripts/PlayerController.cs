using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController> {
    public enum State {
        Air,
        Walking,
        Sliding
    }

    [SerializeField]
    private float m_movementForce = 5, m_desiredVelocity = 5, m_jumpForce = 200, m_airFactor = 0.2f, m_groundFriction = 0.8f, m_slidingFriction = 0.2f, m_correctionForce = 10, m_airCorrectionForce = 4;
    private float m_jumpDelay;
    private State m_currentState = State.Air;

    public Rigidbody playerBody;
    public Grappler grappler;
    public PlayerSFX sfx;

    public bool isGrounded, isSliding;

    public State CurrentState => m_currentState;



    private void Start() {

    }

    private void FixedUpdate() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.TransformDirection(new Vector3(horizontal, 0, vertical).normalized);
        Vector3 currentVelocity = playerBody.velocity.IgnoreY();
        float movementAngle = Vector3.Angle(movement.normalized, currentVelocity.normalized);

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.5f);

        FindState();

        switch (m_currentState) {
            case State.Walking:
                if (movement.magnitude > 0.2f) {
                    if (movementAngle < 90) {
                        playerBody.AddForce(movement * m_movementForce * (m_desiredVelocity - currentVelocity.magnitude));

                        playerBody.AddForce((movement - currentVelocity) * m_correctionForce); // crorection force
                    } else {
                        playerBody.AddForce(movement * m_movementForce * 8);
                    }
                } else {
                    playerBody.AddForce(-currentVelocity * m_movementForce * m_groundFriction);
                }
                break;
            case State.Air:
                playerBody.AddForce(movement * m_movementForce * m_airFactor);
                playerBody.AddForce((movement.normalized - currentVelocity.normalized) * currentVelocity.magnitude * m_airCorrectionForce); // crorection force
                break;
            case State.Sliding:
                playerBody.AddForce((movement.normalized - currentVelocity.normalized) * currentVelocity.magnitude * m_airCorrectionForce); // crorection force
                playerBody.AddForce(-currentVelocity * m_movementForce * m_slidingFriction);
                break;

        }
    }

    private void FindState() {
        if (isGrounded) {
            if (isSliding) {
                m_currentState = State.Sliding;
            } else {
                m_currentState = State.Walking;
            }
        } else {
            m_currentState = State.Air;
        }
    }

    private void Update() {
        if (m_jumpDelay <= 0 && Input.GetButtonDown("Jump")) {
            if (isGrounded) {
                playerBody.AddForce(Vector3.up * m_jumpForce, ForceMode.VelocityChange);
                sfx.Jump();

            } else {
                //wall jumping
            }
            m_jumpDelay = 0.3f;

        }

        isSliding = Input.GetKey(KeyCode.LeftShift);

        m_jumpDelay -= Time.deltaTime;
    }
}
