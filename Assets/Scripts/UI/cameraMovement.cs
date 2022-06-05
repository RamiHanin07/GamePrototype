using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{

    private Camera thisCamera;
    void Start(){

        thisCamera = GetComponent<Camera>();
        thisCamera.orthographic = false;
        print("setting camera to perspective" + thisCamera.orthographic);
        thisCamera.orthographic = true;
        print("setting camera to orthographic" + thisCamera.orthographic);
        
    }
}
