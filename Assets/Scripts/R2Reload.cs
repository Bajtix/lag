using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R2Reload : MonoBehaviour {


    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
