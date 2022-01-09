using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Belt : Building
{
    [SerializeField] private Rigidbody rb;
    public float speed = 1;
    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void FixedUpdate()
    {
        Vector3 pos = rb.position;
        rb.position -= transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(pos);
    }

    
}
