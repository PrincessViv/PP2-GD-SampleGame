using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    [SerializeField] bool invertY;

    float xRot;

    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //get Input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

        if (invertY)
        {
            xRot += mouseY;
        }
        else 
        {
            xRot -= mouseY;
        }
        

        //clamp Rotation -> X- Axis
        xRot = Mathf.Clamp(xRot, lockVertMin, lockVertMax);

        //Rotate cam -> X- Axis
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);

        //Rotate the player -> Y-Axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
