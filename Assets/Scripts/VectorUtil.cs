using UnityEngine;

public static class VectorUtil {
    public static Vector3 IgnoreY(this Vector3 vector) {
        return new Vector3(vector.x, 0, vector.z);
    }

    public static Vector3 IgnoreX(this Vector3 vector) {
        return new Vector3(0, vector.y, vector.z);
    }

    public static Vector3 IgnoreZ(this Vector3 vector) {
        return new Vector3(vector.x, vector.y, 0);
    }
}