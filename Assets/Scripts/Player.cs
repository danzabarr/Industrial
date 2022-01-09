using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private Rigidbody rb;
    public float movementSpeed;
    private float movementInputSpeed;
    private float angle;
    private Vector3 direction;
    public float blendSpeed;
    public float directionalBias;
    public float rotationSpeed;
    public float movementInputSensitivity;
    public bool strafing;
    private Vector3 movement;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }

    void FixedUpdate()
    {
        CameraController cam = Camera.main.GetComponent<CameraController>();

        Vector2 forward = cam.transform.forward.xz().normalized;
        Vector2 right = cam.transform.right.xz().normalized;
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (/*Unit.CurrentAction == Action.Idle && */input.sqrMagnitude > .00001f)
        {
            movementInputSpeed = Mathf.Lerp(movementInputSpeed, 1, movementInputSensitivity * Time.fixedDeltaTime);
            input = input.normalized;

            float a = Mathf.Atan2(input.y, input.x);
            a = Mathf.Lerp(angle, a, blendSpeed * Time.fixedDeltaTime);
            angle = a;
            float runX = 0;
            float runY = 1;
            directionalBias = 1;

            if (strafing)
            {
                runX = Mathf.Cos(a);
                runY = Mathf.Sin(a);
                directionalBias = Vector2.Dot(new Vector2(runX, runY), Vector2.up) * .25f + .75f;
                if (input.y < -0.01f)
                    runX *= -1;
            }

            //animator.SetFloat("runX", runX);
            //animator.SetFloat("runY", runY);

            float runSpeed = movementSpeed * movementInputSpeed * directionalBias;

            right *= input.x;
            forward *= input.y;

            Vector3 move = (right + forward).x0y();

            float rotation = 90 - Mathf.Atan2(move.z, move.x) * Mathf.Rad2Deg;
            if (strafing)
                rotation = cam.yaw;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(rotation, Vector3.up), Time.fixedDeltaTime * rotationSpeed);
            move = move.normalized;

            direction = move;

            move *= runSpeed * Time.fixedDeltaTime;

            movement += move;

            rb.MovePosition(transform.position + move);

        }
        else
        {
            movementInputSpeed = 0;
        }
    }

}
