﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    //todo get movement working
    //head and cannon will follow camera movement, body will move based on WASD input

    //todo, add a horizontal max and min angle

    [SerializeField] float sphereRadius = 2f;
    [SerializeField] [Range(0f, 5f)] float horizontalSensitivity = 1f;
    [SerializeField] [Range(0f, 5f)] float verticalSensitivity = 1f;
    [SerializeField] float maxVerticalAngle = 60f;
    [SerializeField] float minVerticalAngle = 60f;
    [SerializeField] GameObject player;
    [SerializeField] GameObject cannon;
    [SerializeField] GameObject tankHead;
    [SerializeField] GameObject tankBody;
    [SerializeField] float cannonOffset = 20f;
    float sphereAngleX = 60f;
    float sphereAngleY = 180f;
    Camera camera;
    Vector3 initCameraPos;
    bool onGround = false;
    bool againstWallForward = false;
    bool againstWallBackward = false;

    void Start()
    {
        camera = Camera.main;
        initCameraPos = camera.transform.position;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float rawVertical = Input.GetAxis("Vertical");
        float rawHorizontal = Input.GetAxis("Horizontal");
        CameraStuff();

        RotateHeadAndCannon();

    }

    private void RotateHeadAndCannon()
    {
        float headAngle = sphereAngleY + 180;
        tankHead.transform.rotation = Quaternion.Euler(90, headAngle, 0);

        float cannonAngle = -sphereAngleX - cannonOffset;
        Quaternion oldRotation = cannon.transform.rotation;
        Vector3 oldRotationVector = oldRotation.eulerAngles;
        oldRotationVector.x = cannonAngle;
        cannon.transform.rotation = Quaternion.Euler(oldRotationVector);
    }

    private void CameraStuff()
    {
        float rawMouseX = Input.GetAxis("Mouse X");
        float rawMouseY = Input.GetAxis("Mouse Y");

        if (onGround)
        {
            if (rawMouseY < 0)
            {
                rawMouseY = 0;
            }
        }

        if (againstWallForward)
        {
            if (sphereAngleY > 180)
            {
                if (rawMouseX > 0)
                {
                    rawMouseX = 0;
                }
            }
            else
            {
                if (rawMouseX < 0)
                {
                    rawMouseX = 0;
                }
            }
        }

        if (againstWallBackward)
        {
            if (sphereAngleY > 180)
            {
                if (rawMouseX < 0)
                {
                    rawMouseX = 0;
                }
            }
            else
            {
                if (rawMouseX > 0)
                {
                    rawMouseX = 0;
                }
            }
        }

        //using mouseX and mouseY, move the camera about a sphere around the player with radius sphereRadius
        Vector3 newPos = new Vector3();
        Vector3 oldPos = camera.transform.position;

        float oldAngleX = sphereAngleX;
        float oldAngleY = sphereAngleY;

        sphereAngleX -= rawMouseY * verticalSensitivity;
        sphereAngleY += rawMouseX * horizontalSensitivity;

        newPos.x = sphereRadius * Mathf.Sin(Mathf.Deg2Rad * sphereAngleX) * Mathf.Sin(Mathf.Deg2Rad * sphereAngleY);
        newPos.y = sphereRadius * Mathf.Cos(Mathf.Deg2Rad * sphereAngleX);
        newPos.z = sphereRadius * Mathf.Sin(Mathf.Deg2Rad * sphereAngleX) * Mathf.Cos(Mathf.Deg2Rad * sphereAngleY);

        camera.transform.position = newPos;
        camera.transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);

        Vector3 angles = camera.transform.rotation.eulerAngles;

        if (angles.x > minVerticalAngle && angles.x < 360 - maxVerticalAngle)
        {
            if (angles.x < 180f)
            {
                angles.x = minVerticalAngle;
                sphereAngleX = oldAngleX;
                newPos.x = sphereRadius * Mathf.Sin(Mathf.Deg2Rad * sphereAngleX) * Mathf.Sin(Mathf.Deg2Rad * sphereAngleY);
                newPos.y = sphereRadius * Mathf.Cos(Mathf.Deg2Rad * sphereAngleX);
                newPos.z = sphereRadius * Mathf.Sin(Mathf.Deg2Rad * sphereAngleX) * Mathf.Cos(Mathf.Deg2Rad * sphereAngleY);
            }
            else
            {
                angles.x = 360 - maxVerticalAngle;
                sphereAngleX = oldAngleX;
                newPos.x = sphereRadius * Mathf.Sin(Mathf.Deg2Rad * sphereAngleX) * Mathf.Sin(Mathf.Deg2Rad * sphereAngleY);
                newPos.y = sphereRadius * Mathf.Cos(Mathf.Deg2Rad * sphereAngleX);
                newPos.z = sphereRadius * Mathf.Sin(Mathf.Deg2Rad * sphereAngleX) * Mathf.Cos(Mathf.Deg2Rad * sphereAngleY);
            }
        }
        camera.transform.position = newPos;
        camera.transform.rotation = Quaternion.Euler(angles);

        RaycastHit hit = new RaycastHit();
        if (Physics.Linecast(oldPos, camera.transform.position - Vector3.up, out hit))
        {
            onGround = true;
        }
        else
        {
            onGround = false;
        }

        if (Physics.Linecast(oldPos, camera.transform.position + Vector3.forward, out hit))
        {
            againstWallForward = true;
        }
        else
        {
            againstWallForward = false;
        }

        if (Physics.Linecast(oldPos, camera.transform.position - Vector3.forward))
        {
            againstWallBackward = true;
        }
        else
        {
            againstWallBackward = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(Camera.main.transform.position, (Camera.main.transform.position - Vector3.up));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Vector3.forward);
    }
}
