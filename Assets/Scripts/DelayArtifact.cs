using UnityEngine;

public class DelayArtifact : MonoBehaviour {

    public float delay = 1;

    private bool m_reachedPlayer = false;

    private struct ArtifactState {
        public Vector3 position;
        public Quaternion rotation;
        public bool reachedPlayer;

        public ArtifactState(Vector3 position, Quaternion rotation, bool reachedPlayer) {
            this.position = position;
            this.rotation = rotation;
            this.reachedPlayer = reachedPlayer;
        }
    }

    private ArtifactState[] m_states;

    private void Start() {
        m_states = new ArtifactState[4];
        m_states[0] = GetState();
    }

    private void StartRun() {
        if (LevelManager.Instance.currentStageId == 0) return; // dont care
        SetState(m_states[LevelManager.Instance.currentStageId - 1]);
    }

    private void FinishRun() {
        if (LevelManager.Instance.currentStageId == 0) return; // dont care
        m_states[LevelManager.Instance.currentStageId] = GetState();
    }

    private ArtifactState GetState() {
        return new ArtifactState(transform.position, transform.rotation, m_reachedPlayer);
    }

    private void SetState(ArtifactState state) {
        transform.position = state.position;
        transform.rotation = state.rotation;
        m_reachedPlayer = state.reachedPlayer;

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Renderer>().enabled = !m_reachedPlayer;
    }

    private void OnTriggerEnter(Collider other) {
        if (m_reachedPlayer) return;

        if (other.gameObject.CompareTag("Player") && LevelManager.Instance.currentStageType == LevelStage.Vidmo) {
            other.transform.GetComponentInChildren<Grabber>().Bind(this);
        }

        if (other.gameObject.CompareTag("SprinterGhost") && LevelManager.Instance.currentStageType == LevelStage.Vidmo) {
            var player = LevelManager.Instance.replaySprinter.GetComponent<TimeEntity>();
            player.DelaySeconds((int)(TimeManager.Instance.tick * player.playbackSpeed), delay * player.playbackSpeed);
            m_reachedPlayer = true;
        }
    }
}
