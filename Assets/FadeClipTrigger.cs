using UnityEngine;

public class FadeClipTrigger : MonoBehaviour {

    private bool show = false;

    private void OnTriggerEnter(Collider other) {
        show = true;
    }
    private void OnTriggerExit(Collider other) {
        show = false;
    }

    private void Update() {
        UIManager.Instance.cgClippedCamera.alpha = Mathf.Lerp(UIManager.Instance.cgClippedCamera.alpha, show ? 1 : 0, Time.deltaTime * 10);
    }
}
