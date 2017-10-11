﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//FPS camera movement adapted from: http://wiki.unity3d.com/index.php/SmoothMouseLook
public class Movement : MonoBehaviour
{
    private Rigidbody rigidBody;

    private float jumpValue = 0;
    private float lerpTarget = 0;

    public uint jumpCount = 4;
    private uint currJumps = 0;

    public float jumpFactor = 2;
    public float jumpDamping = 0.2f;

    public float movementSpeed = 0.025f;

    private bool isPlaying = false;

    public float sensitivityX = 15f;
    public float sensitivityY = 15f;

    public float minX = -360f;
    public float maxX = 360f;

    public float minY = -60f;
    public float maxY = 60f;

    private float rotX = 0f;
    private float rotY = 0f;

    private List<float> rotListX = new List<float>();
    float avgRotX = 0f;

    private List<float> rotListY = new List<float>();
    float avgRotY = 0f;

    public uint frameCounter = 20;
    Quaternion startingRot;
    Vector3 startingPos;

    // Use this for initialization
    void Awake()
    {
        startingRot = transform.rotation;
        startingPos = transform.position;
    }

    void Start()
    {
#if !UNITY_IOS
        rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.mass = 1;
        rigidBody.drag = 10;
        rigidBody.useGravity = false;
        rigidBody.isKinematic = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
#endif
    }

    public void SwitchToPlaying()
    {
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPlaying = true;
    }

    public void SwitchOutOfPlaying()
    {
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
        Cursor.lockState = CursorLockMode.None;
        isPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (rigidBody)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                transform.position = startingPos;
                currJumps = 0;
            }

            if (!isPlaying)
                return;

            avgRotX = 0f;
            avgRotY = 0f;

            rotY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotX += Input.GetAxis("Mouse X") * sensitivityX;

            rotListY.Add(rotY);
            rotListX.Add(rotX);

            if (rotListX.Count > frameCounter)
            {
                rotListX.RemoveAt(0);
            }

            if (rotListY.Count > frameCounter)
            {
                rotListY.RemoveAt(0);
            }

            for (int i = 0; i < rotListX.Count; i++)
            {
                avgRotX += rotListX[i];
                avgRotY += rotListY[i];
            }

            avgRotX /= rotListX.Count;
            avgRotY /= rotListY.Count;

            avgRotY = ClampAngle(avgRotY, minY, maxY);
            avgRotX = ClampAngle(avgRotX, minX, maxX);

            Quaternion yRot = Quaternion.AngleAxis(avgRotY, Vector3.left);
            Quaternion xRot = Quaternion.AngleAxis(avgRotX, Vector3.up);

            transform.rotation = startingRot * xRot * yRot;

            transform.Translate(Vector3.ProjectOnPlane(transform.forward, Vector3.down) * Input.GetAxis("Vertical") * movementSpeed, Space.World);
            transform.Translate(transform.right * Input.GetAxis("Horizontal") * movementSpeed, Space.World);

            if (Input.GetKeyDown(KeyCode.Space) && currJumps <= jumpCount)
            {
                lerpTarget += jumpFactor;
                currJumps++;
            }

            jumpValue = Mathf.Lerp(jumpValue, lerpTarget, Time.deltaTime * 20);

            if (lerpTarget > 0)
                lerpTarget -= jumpDamping;
            else
                lerpTarget = 0;
        }
    }

    private void FixedUpdate()
    {
        if (rigidBody)
        {
            rigidBody.velocity += Vector3.up * jumpValue;
        }
    }

    float ClampAngle(float angle, float min, float max)
    {
        angle %= 360;
        if ((angle >= -360f) && (angle <= 360f))
        {
            if (angle < -360f)
            {
                angle += 360f;
            }
            if (angle > 360f)
            {
                angle -= 360f;
            }
        }

        return Mathf.Clamp(angle, min, max);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Platform") &&
            (transform.position.y > collision.transform.position.y + collision.transform.localScale.y / 2f))
        {
            currJumps = 0;
        }
    }
}