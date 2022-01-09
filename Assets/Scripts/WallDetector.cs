using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetector : MonoBehaviour
{
    public float range;
    public LayerMask layerMask;
    public QueryTriggerInteraction queryTriggerInteraction;

    public bool hit;
    public RaycastHit hitInfo;
    public Vector3 reflect;
    public Vector3 project;
    public float radius;

    public void OnDrawGizmos()
    {
        return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * range);
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.DrawSphere(transform.position + transform.forward * range, radius);

        if (hit)
        {
            reflect = Vector3.Reflect(transform.forward, hitInfo.normal);
            project = Vector3.ProjectOnPlane(transform.forward, hitInfo.normal);
            
            Gizmos.DrawSphere(hitInfo.point, .1f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(hitInfo.point, hitInfo.point + reflect);
            Gizmos.color = Color.black;
            Gizmos.DrawLine(hitInfo.point, hitInfo.point + project);
        }
    }

    public void Update()
    {
        if (hit = Physics.SphereCast(transform.position, radius, transform.forward, out hitInfo, range, layerMask, queryTriggerInteraction)){

        }
        //if (hit = Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask, queryTriggerInteraction))
        //{
        //
        //}
    }
}
