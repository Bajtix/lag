using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElRaccoone.Tweens;

public partial class LevelManager : Singleton<LevelManager> {

    [SerializeField] private string m_levelName = "I forgor";
    [SerializeField] private GameObject m_sprinterPrefab, m_vidmoPrefab;
    [SerializeField] private LevelCameraController m_cameraController;
    [SerializeField] private Material m_allPlatforms, m_vidmoPlatforms, m_sprinterPlatforms;

    private Dictionary<int, TimeEntity.TimeEntityData>[] m_runs, m_sprinterDelayRuns;
    private Vector3 m_levelStartPos;
    private Quaternion m_levelStartRot;
    private TimeEntity m_controllerTimeEntity;


    public LevelStage currentStageType = LevelStage.None;
    public int currentStageId = 0;
    public int vidmoRuns = 1;
    public GameObject[] ghosts;
    [HideInInspector] public GameObject replaySprinter => ghosts[0];



    public void Start() {
        ghosts = new GameObject[vidmoRuns + 1];
        m_controllerTimeEntity = PlayerController.Instance.GetComponent<TimeEntity>();
        m_runs = new Dictionary<int, TimeEntity.TimeEntityData>[vidmoRuns + 1];
        m_sprinterDelayRuns = new Dictionary<int, TimeEntity.TimeEntityData>[vidmoRuns];

        m_levelStartPos = m_controllerTimeEntity.transform.position;
        m_levelStartRot = m_controllerTimeEntity.transform.rotation;
        StartCoroutine(CLevelIntro());
    }

    private void SetStageType(LevelStage s) {
        currentStageType = s;

        foreach (var w in GameObject.FindObjectsOfType<SpecificPlatform>()) {
            w.SetType(s);
        }

        switch (s) {
            case LevelStage.Sprinter:
                m_sprinterPlatforms.SetFloat("_Solid", 1);
                m_vidmoPlatforms.SetFloat("_Solid", 0);
                break;
            case LevelStage.Vidmo:
                m_sprinterPlatforms.SetFloat("_Solid", 0);
                m_vidmoPlatforms.SetFloat("_Solid", 1);
                break;
            default:
                m_sprinterPlatforms.SetFloat("_Solid", 0);
                m_vidmoPlatforms.SetFloat("_Solid", 0);
                break;
        }

        UIManager.Instance.SetBadge((int)s);
    }

    private void SummonGhost(int i) {
        var vidmo = i != 0;
        var ghost = Instantiate(vidmo ? m_vidmoPrefab : m_sprinterPrefab, m_levelStartPos, m_levelStartRot);
        var te = ghost.GetComponent<TimeEntity>();

        if (i == 0 && currentStageId > 1) {
            Debug.Log($"Setting data from previous vidmo (run {currentStageId - 1})"); // override player runs with data from vidmo interrupts
            te.SetData(m_sprinterDelayRuns[currentStageId - 2]);
        } else te.SetData(m_runs[i]);

        te.isRecording = false;

        if (ghosts[i] != null) {
            Destroy(ghosts[i]);
        }
        ghosts[i] = ghost;
    }

    private void ResetPlayer(bool vidmo) {

        TimeManager.Instance.Reset();

        m_controllerTimeEntity.transform.SetPositionAndRotation(m_levelStartPos, m_levelStartRot);
        m_cameraController.playerCamera.localRotation = Quaternion.Euler(0, 0, 0);
        PlayerController.Instance.playerBody.velocity = Vector3.zero;
        PlayerController.Instance.grappler.Ungrapple();
        m_controllerTimeEntity.ResetData();

        if (vidmo)
            PlayerController.Instance.GetComponent<PlayerAnimator>().SetVidmo();
        else
            PlayerController.Instance.GetComponent<PlayerAnimator>().SetSprinter();
    }

    private void BroadcastAll(string msg, object data = null) {
        foreach (var w in GameObject.FindObjectsOfType<MonoBehaviour>()) {
            w.SendMessage(msg, data, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void RestartLevel(bool voluntary = false) {
        StartCoroutine(CReplayCurrentStage());
    }

    private void Update() {
        if (!TimeManager.Instance.Paused) {
            if (Input.GetKeyDown(KeyCode.R)) {
                RestartLevel(true);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Return)) {
                VidmoFinish();
                return;
            }

            if (PlayerController.Instance.transform.position.y < -20f) {
                RestartLevel(false);
                UIManager.Instance.Announce("You fell down!", 0.8f, 0.09f);
                Announcer.PlayRandom("death");
                return;
            }

        }
    }

    public void SprinterFinish() {
        Debug.Log("Sprinter finished!");
        TimeManager.Instance.Pause();
        StartCoroutine(CSprinterFinish());
    }

    public void VidmoFinish() {
        Debug.Log("Vidmo finished!");
        TimeManager.Instance.Pause();
        StartCoroutine(CVidmoFinish());
    }

    private IEnumerator CLevelIntro() {
        yield return null;
        TimeManager.Instance.Pause();

        m_cameraController.LevelCamera();
        UIManager.Instance.SetBadgeVisible(false);
        SetStageType(LevelStage.None);
        UIManager.Instance.SetFade(false);

        yield return m_cameraController.CFlyBack(20f, 4f, 0.25f);
        UIManager.Instance.Announce($"<b>{m_levelName}</b>", 4, 0.6f);
        yield return new WaitForSeconds(2);
        yield return CStartNewStage(0);
    }

    private IEnumerator CStartNewStage(int stage) {
        TimeManager.Instance.Pause();

        currentStageId = stage;
        if (stage == 0) SetStageType(LevelStage.Sprinter);
        else SetStageType(LevelStage.Vidmo);

        //clear all time data after the current stage
        for (int i = stage; i <= vidmoRuns; i++) if (m_runs[i] == null) m_runs[i] = new(); else m_runs[i].Clear();


        ResetPlayer(stage != 0);
        yield return m_cameraController.CFlyLevelToPlayer(5f);
        yield return CStartRun();
    }

    private IEnumerator CReplayCurrentStage() {
        TimeManager.Instance.Pause();
        m_cameraController.LevelToPlayer();
        m_cameraController.LevelCamera();
        ResetPlayer(currentStageId != 0);
        yield return m_cameraController.CFlyLevelToPlayer(1f);
        yield return CStartRun();
    }

    private IEnumerator CStartRun() {
        m_controllerTimeEntity.ResetData();

        for (int i = 0; i < currentStageId; i++) SummonGhost(i);

        yield return UIManager.Instance.CAnimatedCountdown(3);
        TimeManager.Instance.Unpause();
        BroadcastAll("StartRun");
        m_cameraController.PlayerCamera();
    }

    private IEnumerator CSprinterFinish() {
        BroadcastAll("FinishRun");
        m_cameraController.LevelToPlayer();
        m_cameraController.LevelCamera();

        yield return m_cameraController.CFlyBack(-1f, 1f, 0f);

        UIManager.Instance.Announce($"Sprinter reached the goal in {TimeManager.Instance.time:0.000}s", 5, 0.6f);
        Announcer.Play("ann_sprintergoal");
        yield return new WaitForSeconds(1);
        yield return CLevelControl();
    }

    private IEnumerator CVidmoFinish() {
        BroadcastAll("FinishRun");
        m_cameraController.LevelToPlayer();
        m_cameraController.LevelCamera();

        UIManager.Instance.Announce($"Vidmo {currentStageId} finished the run", 5, 0.6f);

        //Announcer.Play("ann_sprintergoal");
        yield return new WaitForSeconds(1);
        yield return CLevelControl();
    }

    private IEnumerator CLevelControl() {
        m_runs[currentStageId] = m_controllerTimeEntity.GetData();
        if (currentStageId != 0) {
            m_sprinterDelayRuns[currentStageId - 1] = replaySprinter.GetComponent<TimeEntity>().GetData();
            Debug.Log($"Getting data from sprinter ghost. Got {m_sprinterDelayRuns[currentStageId - 1].Count} from vidmo (run {currentStageId})");
        }


        yield return null;
        UIManager.Instance.SetLevelControl(true);
        while (true) {
            if (Input.GetKeyDown(KeyCode.R)) {
                UIManager.Instance.SetLevelControl(false);
                yield return new WaitForSeconds(0.5f);
                yield return CReplayCurrentStage();
                yield break;
            }
            if (Input.GetKeyDown(KeyCode.F)) {
                UIManager.Instance.SetLevelControl(false);
                yield return new WaitForSeconds(0.5f);

                yield return CStartNewStage(currentStageId + 1);
                yield break;
            }
            yield return null;
        }

    }
}
