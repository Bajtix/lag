using System.Collections;
using System.Collections.Generic;
using ElRaccoone.Tweens;
using UnityEngine;

public class LevelCameraController : MonoBehaviour {
    public Transform playerCamera, levelCamera;
    private Camera m_playerCamCam, m_levelCamCam;

    private void Start() {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        levelCamera = GameObject.FindGameObjectWithTag("LevelCamera").transform;

        m_playerCamCam = playerCamera.GetComponent<Camera>();
        m_levelCamCam = levelCamera.GetComponent<Camera>();
    }

    public void LevelCamera() {
        levelCamera.gameObject.SetActive(true);
        playerCamera.transform.parent.gameObject.SetActive(false);

        UIManager.Instance.SetCursor(false);
    }

    public void PlayerCamera() {
        levelCamera.gameObject.SetActive(false);
        playerCamera.transform.parent.gameObject.SetActive(true);

        UIManager.Instance.SetCursor(true);

    }

    public void LevelToPlayer() {
        levelCamera.position = playerCamera.position;
        levelCamera.rotation = playerCamera.rotation;
        m_levelCamCam.fieldOfView = m_playerCamCam.fieldOfView;
    }

    public IEnumerator CFlyLevelToPlayer(float duration, float wait = 1f) {
        levelCamera.TweenCancelAll();
        levelCamera.TweenPosition(playerCamera.position, duration).SetEaseCubicInOut();
        levelCamera.TweenRotation(playerCamera.rotation.eulerAngles, duration).SetEaseCubicInOut();
        m_levelCamCam.TweenCameraFieldOfView(m_playerCamCam.fieldOfView, duration).SetEaseCubicInOut();
        yield return new WaitForSeconds(duration * wait);
    }

    public IEnumerator CFlyBack(float dst, float duration, float wait = 1f) {
        levelCamera.TweenPosition(levelCamera.position + levelCamera.forward * -dst, duration).SetEaseCircOut();
        m_levelCamCam.TweenCameraFieldOfView(70, duration).SetEaseCircOut().SetFrom(60);
        yield return new WaitForSeconds(duration * wait);
    }
}
