using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour {

    public Transform followTransform;
    public float yDistance;
    public float xRotation;
    public float yRotation;
    public float scrollSpeed;
    public static MyCamera theCamera;
    void FixedUpdate() {
        theCamera = this; //TODO: Optimize for the love of god
        transform.position = followTransform.position + new Vector3(0, yDistance + 10, 0);
        yDistance += Input.GetAxis("Mouse ScrollWheel") * -5;

        transform.eulerAngles = new Vector3(yRotation, xRotation, 0);

        if (Input.GetKey(KeyCode.LeftShift)) {
            xRotation += Input.GetAxis("Mouse X");
            yRotation -= Input.GetAxis("Mouse Y");
        }

    }
}