using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElRaccoone.Tweens;


public class LevelGoal : MonoBehaviour {


    private void StartRun() {
        if (LevelManager.Instance.currentStageType == LevelStage.Sprinter)
            gameObject.TweenLocalScale(Vector3.one, 0.4f).SetEaseCircIn();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") && LevelManager.Instance.currentStageType == LevelStage.Sprinter) {
            LevelManager.Instance.SprinterFinish();
            gameObject.TweenLocalScale(Vector3.zero, 0.4f).SetEaseCircIn();
        }
    }
}
