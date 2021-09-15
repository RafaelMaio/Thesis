// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the camera movement, scaling and rotation logic.
// SPECIAL NOTES: 
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    /// <summary>
    /// Camera moviment speed.
    /// </summary>
    [SerializeField] float navigationSpeed = 2.4f;

    /// <summary>
    /// Camera scaling speed.
    /// </summary>
    [SerializeField] float navigationScaleSpeed = 4f;

    /// <summary>
    /// Shift Multiplier (when click the left shift key).
    /// </summary>
    [SerializeField] float shiftMultiplier = 2f;

    /// <summary>
    /// Rotation sensitivity.
    /// </summary>
    [SerializeField] float sensitivity = 0.5f;

    private Camera cam;
    private Vector3 anchorPoint;
    private Quaternion anchorRot;

    /// <summary>
    /// Unity Awake function.
    /// </summary>
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    /// <summary>
    /// Unity Update function.
    /// Handles the camera movement, scaling and rotation logic.
    /// </summary>
    void Update()
    {
        bool up = false;
        Vector3 move = Vector3.zero;
        Vector3 initialPos = transform.position;
        float scaleSpeed = navigationScaleSpeed * (Input.GetKey(KeyCode.LeftShift) ? shiftMultiplier : 1f) * Time.deltaTime * 9.1f;
        float speed = navigationSpeed * (Input.GetKey(KeyCode.LeftShift) ? shiftMultiplier : 1f) * Time.deltaTime * 9.1f;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            move += Vector3.forward * scaleSpeed;
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            move -= Vector3.forward * scaleSpeed;
        if (Input.GetKey(KeyCode.D))
            move += Vector3.right * speed;
        if (Input.GetKey(KeyCode.A))
            move -= Vector3.right * speed;
        if (Input.GetKey(KeyCode.W))
        {
            move += Vector3.up * speed;
            up = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            move -= Vector3.up * speed;
            up = true;
        }
        transform.Translate(move);
        if (up)
        {
            Vector3 medPos = transform.position;
            Vector3 finalPos = medPos - initialPos;
            transform.Translate(new Vector3(0, -finalPos.y, 0), Space.World);
        }

        if (Input.GetMouseButtonDown(1))
        {
            anchorPoint = new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
            anchorRot = transform.rotation;
        }
        if (Input.GetMouseButton(1))
        {
            Quaternion rot = anchorRot;

            Vector3 dif = anchorPoint - new Vector3(Input.mousePosition.y, -Input.mousePosition.x);
            rot.eulerAngles += dif * sensitivity;
            transform.rotation = rot;
        }
    }
}