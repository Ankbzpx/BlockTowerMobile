using UnityEngine;

public class CameraRestrict : MonoBehaviour {

	void Update ()
    {
        //can be simplified
        var factor = 1.0f - Mathf.Exp(-10f * Time.fixedDeltaTime);

        if (transform.rotation.x < 0)
            transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion (0, transform.rotation.y, transform.rotation.z, transform.rotation.w), factor);
        if (transform.rotation.x > 45)
            transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(45, transform.rotation.y, transform.rotation.z, transform.rotation.w), factor);

    }
}
