using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbLift : MonoBehaviour {
    public Vector3 startPosition, endPosition;

    [SerializeField]
    private float m_speed = 1;

    private void Update() {
        transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.PingPong(TimeManager.Instance.time * m_speed, 1));
    }


}
