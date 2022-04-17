using UnityEngine;

public class PlayerSFX : MonoBehaviour {
    public SoundEffect footstep,
    jump,
    land,
    grapple;

    private float m_stepStep;

    [SerializeField]
    private PlayerController m_playerController;
    private bool m_lastGroundCheck = false;

    [SerializeField]
    private AudioSource m_airNoise, m_slideNoise;

    private void Start() {
        if (m_playerController == null) m_playerController = PlayerController.Instance;
    }

    private void Update() {
        float playerSpeed = m_playerController.playerBody.velocity.IgnoreY().magnitude;


        if (m_playerController.isGrounded && !m_lastGroundCheck && !m_playerController.isSliding) {
            Land();
        }

        m_slideNoise.volume = Mathf.Clamp01(Mathf.Lerp(m_slideNoise.volume, (m_playerController.CurrentState == PlayerController.State.Sliding ? 1 : 0) * (playerSpeed * 0.3f), Time.deltaTime * 12));
        m_slideNoise.pitch = Mathf.Clamp((playerSpeed * 0.05f) + 0.9f, 0.8f, 1.3f);

        m_airNoise.volume = Mathf.Clamp01(Mathf.Lerp(m_airNoise.volume, (m_playerController.isGrounded ? 0.5f : 2f) * (playerSpeed * 0.02f), Time.deltaTime * 12));

        m_lastGroundCheck = m_playerController.isGrounded;


        if (m_playerController.CurrentState == PlayerController.State.Walking)
            m_stepStep += Time.deltaTime * 0.5f * playerSpeed * 1.5f;

        if (m_stepStep >= 1) {
            m_stepStep = 0;
            Step();
        }
    }

    public void Step() {
        footstep.Play(transform.position - Vector3.up * 0.2f);
    }

    public void Jump() {
        jump.Play(transform.position - Vector3.up * 0.2f);
    }

    public void Land() {
        land.Play(transform.position - Vector3.up * 0.2f);
    }

    public void Grapple() {
        grapple.Play(transform.position + Vector3.up);
    }
}