using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ElRaccoone.Tweens;




public class UIManager : Singleton<UIManager> {

    [SerializeField] private bool m_cursorAutoLock = true;
    [SerializeField] private Sprite[] m_badges;
    private float m_normalCountdownY;
    private bool m_isAnnouncing;
    [SerializeField] private UISounds m_sfx;




    public TextMeshProUGUI lblTimer, lblCountdown, lblAnnouncement;
    public Image imBadge;
    public CanvasGroup cgFader, cgCursor, cgClippedCamera;
    public RectTransform pnlAnnouncement, pnlLevelControl;
    public Image[] imLevelControlOptions;
    public Image imMenu, imNextLevel;


    private void Start() {
        Cursor.lockState = m_cursorAutoLock ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !m_cursorAutoLock;
        m_normalCountdownY = lblCountdown.rectTransform.anchoredPosition.y;

        //swap the order for visual quality. Just.. don't question it.
        switch (LevelManager.Instance.vidmoRuns) {
            case 2:
                var t1 = imLevelControlOptions[1].rectTransform.position;
                imLevelControlOptions[1].rectTransform.position = imLevelControlOptions[2].rectTransform.position;
                imLevelControlOptions[2].rectTransform.position = t1;
                break;
            case 3:
                var t2 = imLevelControlOptions[1].rectTransform.position;
                imLevelControlOptions[1].rectTransform.position = imLevelControlOptions[3].rectTransform.position;
                imLevelControlOptions[3].rectTransform.position = t2;
                break;

        }
    }

    private void Update() {
        if (TimeManager.Instance.Paused) {
            lblTimer.text = "";
            return;
        }

        if (LevelManager.Instance.currentStageType == LevelStage.Sprinter) {
            lblTimer.text = $"{TimeManager.Instance.time:00.00}s<size=50%> / {LevelManager.Instance.sprinterMaxTime:00.00}s</size>";
        }

        if (LevelManager.Instance.currentStageType == LevelStage.Vidmo) {
            lblTimer.text = $"{LevelManager.Instance.timeToComplete - TimeManager.Instance.time:00.00}s";
        }
    }

    public void SetFade(bool v) {
        if (!v) {
            cgFader.TweenCanvasGroupAlpha(0, 0.5f).SetFrom(1).SetOnComplete(() => cgFader.gameObject.SetActive(false));
        } else {
            cgFader.gameObject.SetActive(true);
            cgFader.TweenCanvasGroupAlpha(1, 0.5f).SetFrom(0);
        }
    }

    public void SetCursor(bool v) {
        if (!v) {
            cgCursor.TweenCanvasGroupAlpha(0, 0.5f).SetFrom(1).SetOnComplete(() => cgCursor.gameObject.SetActive(false));
        } else {
            cgCursor.gameObject.SetActive(true);
            cgCursor.TweenCanvasGroupAlpha(1, 0.5f).SetFrom(0);
        }
    }

    public void SetBadge(int i) {
        switch (i) {
            case 0 or 1:
                imBadge.gameObject.SetActive(true);
                imBadge.GetComponent<CanvasGroup>().alpha = 0;
                imBadge.sprite = m_badges[i];
                break;
            default:
                imBadge.gameObject.SetActive(false);
                break;
        }
    }

    public void Announce(string txt, float duration = 3f, float interp = 0.2f) {
        StartCoroutine(CAnnounce(txt, duration, interp));
    }

    public IEnumerator CAnnounce(string txt, float duration = 3f, float interp = 0.2f) {
        if (m_isAnnouncing) {
            pnlAnnouncement.TweenCancelAll();
            pnlAnnouncement.TweenLocalScaleX(0, interp).SetEaseCubicInOut();
            yield return new WaitForSeconds(interp);
            m_isAnnouncing = false;
            yield return CAnnounce(txt, duration);
        }

        lblAnnouncement.text = txt;
        pnlAnnouncement.TweenLocalScaleX(1, interp).SetEaseCubicInOut();
        m_isAnnouncing = true;
        yield return new WaitForSeconds(duration);
        m_isAnnouncing = false;

        pnlAnnouncement.TweenLocalScaleX(0, interp).SetEaseCubicInOut();
    }

    public void SetBadgeVisible(bool v) {
        imBadge.TweenCanvasGroupAlpha(v ? 1 : 0, 0.5f);
    }

    public IEnumerator CAnimatedCountdown(float dur) {
        lblCountdown.gameObject.SetActive(true);
        lblCountdown.TweenCanvasGroupAlpha(1, 0.1f);
        lblCountdown.rectTransform.anchoredPosition = new Vector2(lblCountdown.rectTransform.anchoredPosition.x, m_normalCountdownY);
        lblCountdown.text = "3";
        Announcer.Play("ann_three");
        yield return new WaitForSeconds(dur / 4f);
        lblCountdown.text = "2";
        Announcer.Play("ann_two");
        yield return new WaitForSeconds(dur / 4f);
        lblCountdown.text = "1";
        Announcer.Play("ann_one");
        yield return new WaitForSeconds(dur / 4f);
        lblCountdown.text = $"{LevelManager.Instance.currentStageType.ToString().ToUpper()} START!";
        Announcer.Play($"ann_{LevelManager.Instance.currentStageType.ToString().ToLower()}start");
        lblCountdown.TweenCanvasGroupAlpha(0, 0.5f).SetEase(ElRaccoone.Tweens.Core.EaseType.QuadIn).SetOnComplete(() => lblCountdown.gameObject.SetActive(false)).SetDelay(dur / 4f);
    }

    public void HighlightLevelControl(int a) {
        for (int i = 0; i < imLevelControlOptions.Length; i++) {
            imLevelControlOptions[i].gameObject.SetActive(true);
            imLevelControlOptions[i].color = Color.gray;
            imLevelControlOptions[i].GetComponentInChildren<TextMeshProUGUI>().text = "";

            if (i == LevelManager.Instance.currentStageId) {
                imLevelControlOptions[i].GetComponentInChildren<TextMeshProUGUI>().text = "[R] Restart";
            }
            if (i == a) {
                imLevelControlOptions[i].color = Color.white;
                imLevelControlOptions[i].GetComponentInChildren<TextMeshProUGUI>().text = "[F] Play Next";
            }
            if (i > LevelManager.Instance.currentStageId + 1) {
                imLevelControlOptions[i].color = Color.black;
            }

            if (i > LevelManager.Instance.vidmoRuns) {
                imLevelControlOptions[i].gameObject.SetActive(false);
            }

        }
    }

    public void SetLevelControl(bool v) {
        HighlightLevelControl(LevelManager.Instance.currentStageId + 1);
        pnlLevelControl.TweenCancelAll();
        pnlLevelControl.TweenAnchoredPositionY((v ? 1 : -1) * pnlLevelControl.rect.height / 2, 0.3f);
    }
}
