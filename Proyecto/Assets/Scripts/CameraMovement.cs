using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float Followspeed = 2f;
    public float yOffset;
    public Transform target;


    void Update()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);
        transform.position = Vector3.Lerp(transform.position, newPos, Followspeed * Time.deltaTime);




    }

}
