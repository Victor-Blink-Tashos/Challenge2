using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public float cameraHight;
    public Transform target;
    public Transform rTarget;
    public float distance;



    void Start()
    {

    }


    void Update()
    {
        //Set the camera position to player position with offser (distance) to get it to follow the player around
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y + cameraHight, target.transform.position.z - distance);
        transform.LookAt(rTarget);
    }
}
