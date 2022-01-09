using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    private Rigidbody rb;
    public float beltPositionAdjustment = 2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Belt belt))
        {
            Vector3 targetPosition = GetClosestPoint(belt.transform, transform.position);
            transform.position = Vector3.Lerp(transform.position, targetPosition, beltPositionAdjustment * Time.deltaTime);
        }
    }

    public static Vector3 GetClosestPoint(Transform transform, Vector3 point)
    {
        Vector2 A = new Vector2(transform.position.x, transform.position.z);
        Vector2 B = A + new Vector2(transform.forward.x, transform.forward.z);
        Vector2 P = new Vector2(point.x, point.z);

        Vector2 L = GetClosestPointOnLine(A, B, P);

        return new Vector3(L.x, point.y, L.y);
    }

    public static Vector2 GetClosestPointOnLine(Vector2 A, Vector2 B, Vector2 P)
    {
        Vector2 AP = P - A;       //Vector from A to P   
        Vector2 AB = B - A;       //Vector from A to B  

        float magnitudeAB = AB.sqrMagnitude;     //Magnitude of AB vector (it's length squared)     
        float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
        float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

        return A + AB * distance;
    }
}
