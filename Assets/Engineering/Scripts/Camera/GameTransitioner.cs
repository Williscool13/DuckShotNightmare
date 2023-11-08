using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameTransitioner : MonoBehaviour
{
    [SerializeField] CameraReference cam;
    [SerializeField] CameraData[] cameraData;

    [SerializeField] Animator curtainAnimator;
    [SerializeField] GameMusicManager musicManager;
    [SerializeField] GameObject clickToContinueText;
    [SerializeField] GameObject gameTitleText;

    [SerializeField] UnityEvent startGameEvent;

    [SerializeField] IntegerVariable gameStage;
    [SerializeField] UnityEvent gameStageChangeEvent;

    [SerializeField] GameObject[] blinders;
    public void StartGameTransition() {
        StartCoroutine(StartGameTransitionCoroutine());
    }

    IEnumerator StartGameTransitionCoroutine() {
        clickToContinueText.SetActive(false);
        yield return StartCoroutine(LerpMotion(cameraData[0].position, cameraData[0].eulerRotation, cameraData[0].perspective, cameraData[0].viewValue, cameraData[0].transitionSpeed));

        musicManager.FadeoutClip(1.0f);
        curtainAnimator.SetTrigger("OpenCurtains");
        

        yield return StartCoroutine(LerpMotion(cameraData[1].position, cameraData[1].eulerRotation, cameraData[1].perspective, cameraData[1].viewValue, cameraData[1].transitionSpeed));
        gameTitleText.SetActive(false);
        gameStage.Value = 0;
        gameStageChangeEvent.Invoke();

        cam.Value.transform.position = cameraData[2].position;
        cam.Value.transform.eulerAngles = cameraData[2].eulerRotation;
        cam.Value.orthographic = true;
        cam.Value.orthographicSize = cameraData[2].viewValue;

        foreach (GameObject gob in blinders) {
            gob.SetActive(true);
        }

        yield return new WaitForSeconds(7.0f);
    }


    IEnumerator LerpMotion(Vector3 position, Vector3 eulerRotation, bool perspective, float viewValue, float speed) {
        Vector3 basePos = cam.Value.transform.position;
        Vector3 baseEulerRot = cam.Value.transform.eulerAngles;
        float baseFov = cam.Value.fieldOfView;


        float lerpVal = 0;
        while (lerpVal != 1) {
            lerpVal = Mathf.Min(1, lerpVal + Time.fixedDeltaTime * speed);

            Vector3 pos = Vector3.Lerp(basePos, position, lerpVal);
            Vector3 eulderRot = Vector3.Lerp(baseEulerRot, eulerRotation, lerpVal);

            cam.Value.transform.position = pos;
            cam.Value.transform.eulerAngles = eulderRot;
            cam.Value.fieldOfView = Mathf.Lerp(baseFov, viewValue, lerpVal);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    public void EndGameTransitionWin() {
        StartCoroutine(EndGameTransitionWinCoroutine());
    }

    [SerializeField] Image transition;
    IEnumerator EndGameTransitionWinCoroutine() {
        CameraShaker shaker = cam.Value.GetComponent<CameraShaker>();
        yield return new WaitForSeconds(7.0f);
        shaker.StartShake(0.2f, 20.0f, 1);
        musicManager.FadeoutClip(0.1f);
        yield return new WaitForSeconds(5.0f);

        // fade to white and enter postgame
        float alpha = 0.0f;
        while (alpha < 1) {
            alpha += Time.deltaTime / 4;
            transition.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        // swap scene
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadSceneAsync("WinGameScene");
    }

    public void EndGameTransitionLose() {
        StartCoroutine(EndGameTransitionLoseCoroutine());
    }

    IEnumerator EndGameTransitionLoseCoroutine() {
        CameraShaker shaker = cam.Value.GetComponent<CameraShaker>();
        musicManager.FadeoutClip(0.1f);
        shaker.RedShake(1.0f, 20.0f, 1);
        yield return new WaitForSeconds(5.0f);

        // fade to red and enter postgame


        float alpha = 0f;
        while (alpha < 1) {
            alpha += Time.deltaTime / 4;
            transition.color = new Color(1, 0, 0, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadSceneAsync("LoseGameScene");
    }

    [Serializable]
    public class CameraData
    {
        public Vector3 position;
        public Vector3 eulerRotation;
        public bool perspective;
        public float viewValue;
        public float transitionSpeed;

        public CameraData(bool perspective, float value, float speed, Vector3 position, Vector3 eulerRotation) {
            this.perspective = perspective;
            this.viewValue = value;
            this.transitionSpeed = speed;
            this.position = position;
            this.eulerRotation = eulerRotation;
        }

    }
}
