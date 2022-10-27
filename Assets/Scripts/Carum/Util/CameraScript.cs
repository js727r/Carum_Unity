using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float speed = 3.5f;
    private float X;
    private float Y;

    void Update()
    {
        // if (Input.GetMouseButton(0))
        // {
        //     transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * speed, -Input.GetAxis("Mouse X") * speed, 0));
        //     X = transform.rotation.eulerAngles.x;
        //     Y = transform.rotation.eulerAngles.y;
        //     transform.rotation = Quaternion.Euler(X, Y, 0);
        // }
        if (Input.GetMouseButton(0))
        {
            var xAxis = Input.GetAxis("Mouse X")*speed;
            transform.Rotate(new Vector3(0,xAxis,0),Space.World);
            
        }
    }
}