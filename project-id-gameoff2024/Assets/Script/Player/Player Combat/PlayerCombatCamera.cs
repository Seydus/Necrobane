using UnityEngine;
using System.Collections;

public class CameraCombatInfo
{
    public float duration;
    public float magnitude;
    public Vector3 direction;

    public CameraCombatInfo(float duration, float magnitude, Vector3 direction)
    {
        this.duration = duration;
        this.magnitude = magnitude;
        this.direction = direction;
    }
}

public class PlayerCombatCamera : MonoBehaviour
{
    public Camera cam;

    public IEnumerator CameraSwingShake(CameraCombatInfo cameraCombatInfo)
    {
        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < cameraCombatInfo.duration)
        {
            float swing = Mathf.Sin(elapsed * Mathf.PI / cameraCombatInfo.duration) * cameraCombatInfo.magnitude;
            cam.transform.localPosition = originalPos + cameraCombatInfo.direction * swing;

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }

    public IEnumerator CameraShake(CameraCombatInfo cameraCombatInfo)
    {
        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < cameraCombatInfo.duration)
        {
            float x = Random.Range(-1f, 1f) * cameraCombatInfo.magnitude;
            float y = Random.Range(-1f, 1f) * cameraCombatInfo.magnitude;

            cam.transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }

}
