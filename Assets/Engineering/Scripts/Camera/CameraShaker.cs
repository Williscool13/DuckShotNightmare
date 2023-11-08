using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraShaker : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] AudioSource cameraShakeSource;
    [SerializeField] Image red;
    IEnumerator shakeRoutine;
    public void StartShake(float intensity, float duration, int delayCount) {
        if (shakeRoutine != null) { Debug.Log("Already shaking");  return;  }
        shakeRoutine = ScreenShake(intensity, duration, delayCount);
        StartCoroutine(shakeRoutine);
    }

    public void RedShake(float intensity, float duration, int delayCount) {
        if (shakeRoutine != null) { Debug.Log("Already shaking"); return; }

        shakeRoutine = RedScreenShake(intensity, duration, delayCount);
        StartCoroutine(shakeRoutine);
    }

    public void StopShake() {
        if (shakeRoutine == null) { return; }
        StopCoroutine(shakeRoutine);
        shakeRoutine = null;
        mainCam.transform.position = basePosition;
        cameraShakeSource.Stop();
        currShakeTime = cameraShakeSource.time;
        // disable red
        red.enabled = false;
    }
    Vector3 basePosition;
    float currShakeTime;

    WaitForFixedUpdate fixedUpdateWait = new();
    IEnumerator ScreenShake(float intensity, float duration, int delayCount) {
        cameraShakeSource.time = currShakeTime;
        cameraShakeSource.Play();
        float timer = duration;
        basePosition = mainCam.transform.position;

        while (timer > 0) {
            // screen shakefloat FX = Random.Range (-1.0f, 1.0f);
            float xDelta = Random.Range(-intensity, intensity);
            float yDelta = Random.Range(-intensity, intensity);

            mainCam.transform.position = new Vector3(basePosition.x + xDelta, basePosition.y + yDelta, basePosition.z);

            for (int i = 0; i < delayCount; i++) {
                yield return fixedUpdateWait;
            }
            timer -= Time.fixedDeltaTime * delayCount;
        }

        mainCam.transform.position = basePosition;

        cameraShakeSource.Stop();
        currShakeTime = cameraShakeSource.time;
        shakeRoutine = null;
    }

    IEnumerator RedScreenShake(float intensity, float duration, int delayCount) {
        cameraShakeSource.time = currShakeTime;
        cameraShakeSource.Play();
        float timer = duration;
        basePosition = mainCam.transform.position;

        red.enabled = true;
        red.color = new Color(1, 0, 0, 0.1f + intensity);
        while (timer > 0) {
            // screen shakefloat FX = Random.Range (-1.0f, 1.0f);
            float xDelta = Random.Range(-intensity, intensity);
            float yDelta = Random.Range(-intensity, intensity);

            mainCam.transform.position = new Vector3(basePosition.x + xDelta, basePosition.y + yDelta, basePosition.z);

            for (int i = 0; i < delayCount; i++) {
                yield return fixedUpdateWait;
            }
            timer -= Time.fixedDeltaTime * delayCount;
        }

        mainCam.transform.position = basePosition;
        red.enabled = false;

        cameraShakeSource.Stop();
        currShakeTime = cameraShakeSource.time;
        shakeRoutine = null;
    }

    public bool IsShaking() {
        return shakeRoutine != null;
    }
}
