using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimator : MonoBehaviour {

    public struct PlayerAnimatorData {
        public bool grounded, sliding, grapple;
        public float speed;
    }

    [SerializeField]
    private GameObject m_vidmo, m_sprinter;

    private Animator m_animator;

    private PlayerController m_playerController;

    [SerializeField] private Grappler m_grappler;

    [SerializeField] private TimeEntity m_timeEntity;

    public bool playbackOnly = false;

    public LevelStage type = LevelStage.None;



    private void Start() {
        m_playerController = GetComponent<PlayerController>();

        if (type == LevelStage.Vidmo) SetVidmo();
        else if (type == LevelStage.Sprinter) SetSprinter();
    }

    public void SetVidmo() {
        m_vidmo.SetActive(true);
        m_sprinter.SetActive(false);
        m_animator = m_vidmo.GetComponent<Animator>();
    }

    public void SetSprinter() {
        m_vidmo.SetActive(false);
        m_sprinter.SetActive(true);
        m_animator = m_sprinter.GetComponent<Animator>();
    }

    private void Update() {

        if (m_animator == null) return;

        if (playbackOnly) {
            if (m_timeEntity.data == null) return;
            var d = (PlayerAnimatorData)m_timeEntity.data;
            m_animator.SetBool("Grounded", d.grounded);
            m_animator.SetBool("Sliding", d.sliding);
            m_animator.SetBool("Grapple", d.grapple);
            m_animator.SetFloat("Speed", d.speed);
            return;
        }

        m_animator.SetBool("Grounded", m_playerController.isGrounded);
        m_animator.SetBool("Sliding", m_playerController.isSliding);
        m_animator.SetBool("Grapple", m_grappler.grappling);
        m_animator.SetFloat("Speed", m_playerController.playerBody.velocity.IgnoreY().magnitude / 4);

        m_timeEntity.data = new PlayerAnimatorData()
        {
            grounded = m_playerController.isGrounded,
            sliding = m_playerController.isSliding,
            grapple = m_grappler.grappling,
            speed = m_playerController.playerBody.velocity.IgnoreY().magnitude
        };

    }
}
