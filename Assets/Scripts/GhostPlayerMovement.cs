using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Rigidbody rb;
    private Vector3 dir;
    private float horizontal;
    private float vertical;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        dir = transform.forward * vertical + transform.right * horizontal;

        if (Input.GetKey(KeyCode.Space))
        {
            dir += new Vector3(0, 3, 0);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            dir += new Vector3(0, -3, 0);
        }




        rb.AddForce(dir.normalized * moveSpeed, ForceMode.Force);
    }
}
