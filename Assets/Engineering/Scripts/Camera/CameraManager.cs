using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CameraVariable mainCam;
    [SerializeField] Camera thisCam;
    // Start is called before the first frame update
    void Start()
    {
        mainCam.Value = thisCam;
    }

}
