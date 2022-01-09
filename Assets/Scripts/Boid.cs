using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Transform tether;
    public float tetherRadius;
    public float tetherAmount = 0.01f;
    public static List<Boid> boids = new List<Boid>();
    public float alignment = 1;
    public float separation = 1;
    public float cohesion = 1;
    public float detect = 1;
    public float range = 5;
    public float maxForce = .2f;
    public float maxSpeed = 4;
    public float minSpeed = 2;
    private WallDetector detector;
    public float reflect;
    public float project;

    private Vector3 velocity;
    private Vector3 acceleration;
    private Vector3 cachedVelocity;

    private Vector3 rayForward;
    private Vector3 rayUp;
    private Vector3 rayDown;
    private Vector3 rayLeft;
    private Vector3 rayRight;

    private Vector3 averagePosition;
    private Vector3 averageVelocity;
    private Vector3 averageDifference;

    public float rayDistance;
    public Vector2 rayAngle;
    public LayerMask rayLayerMask;
    

    private void Awake()
    {
        detector = GetComponent<WallDetector>();
        velocity = transform.forward * 50;
    }

    private void OnDrawGizmos()
    {
        Quaternion up = Quaternion.AngleAxis(-rayAngle.y, transform.right);
        Quaternion down = Quaternion.AngleAxis(rayAngle.y, transform.right);
        Quaternion left = Quaternion.AngleAxis(-rayAngle.x, transform.up);
        Quaternion right = Quaternion.AngleAxis(rayAngle.x, transform.up);
        Quaternion roll = Quaternion.AngleAxis(-transform.rotation.eulerAngles.z, transform.forward);

        Vector3 rayForward = transform.forward;

        Vector3 rayUp = transform.forward;
        rayUp = up * rayUp;
        rayUp = roll * rayUp;

        Vector3 rayDown = transform.forward;
        rayDown = down * rayDown;
        rayDown = roll * rayDown;

        Vector3 rayLeft = transform.forward;
        rayLeft = left * rayLeft;
        rayLeft = roll * rayLeft;

        Vector3 rayRight = transform.forward;
        rayRight = right * rayRight;
        rayRight = roll * rayRight;

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + rayForward * rayDistance);
        Gizmos.DrawLine(transform.position, transform.position + rayUp * rayDistance);
        Gizmos.DrawLine(transform.position, transform.position + rayDown * rayDistance);
        Gizmos.DrawLine(transform.position, transform.position + rayLeft * rayDistance);
        Gizmos.DrawLine(transform.position, transform.position + rayRight * rayDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Detect());
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, averagePosition);
        Gizmos.DrawSphere(averagePosition, .25f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + averageVelocity);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + averageDifference);
    }

    void Start()
    {
        boids.Add(this);
    }

    private void OnDestroy()
    {
        boids.Remove(this);
    }

    private Vector3 Detect()
    {
        Quaternion up = Quaternion.AngleAxis(-rayAngle.y, transform.right);
        Quaternion down = Quaternion.AngleAxis(rayAngle.y, transform.right);
        Quaternion left = Quaternion.AngleAxis(-rayAngle.x, transform.up);
        Quaternion right = Quaternion.AngleAxis(rayAngle.x, transform.up);
        Quaternion roll = Quaternion.AngleAxis(-transform.rotation.eulerAngles.z, transform.forward);

        rayForward = transform.forward;

        rayUp = transform.forward;
        rayUp = up * rayUp;
        //rayUp = roll * rayUp;

        rayDown = transform.forward;
        rayDown = down * rayDown;
        //rayDown = roll * rayDown;

        rayLeft = transform.forward;
        rayLeft = left * rayLeft;
        //rayLeft = roll * rayLeft;

        rayRight = transform.forward;
        rayRight = right * rayRight;
        //rayRight = roll * rayRight;

        Vector3 direction = Vector3.zero;
        int count = 0;

        Vector3 Cast(Vector3 dir)
        {
            if (Physics.Raycast(transform.position, dir, out RaycastHit hitInfo, rayDistance, rayLayerMask, QueryTriggerInteraction.Ignore))
            {
                count++;
                return -dir * rayDistance / (hitInfo.distance * hitInfo.distance);
            }
            else return Vector3.zero;
        }

        direction += Cast(rayForward);
        direction += Cast(rayUp);
        direction += Cast(rayDown);
        direction += Cast(rayLeft);
        direction += Cast(rayRight);

        if (count <= 0)
            return Vector3.zero;

        //direction /= count;

        direction *= detect;

        return direction;
    }

    // Update is called once per frame
    void Update()
    {
        acceleration = Vector3.zero;

        acceleration += Detect();

        //velocity += Alignment() * alignment * Time.fixedDeltaTime;
        //velocity += Separation() * separation * Time.fixedDeltaTime;
        //velocity += Cohesion() * cohesion * Time.fixedDeltaTime;
        acceleration += Tether();

        acceleration += Flock();

        cachedVelocity = velocity + acceleration * Time.deltaTime;

        float mag = cachedVelocity.magnitude;

        cachedVelocity /= mag;
        cachedVelocity *= Mathf.Min(mag, maxSpeed);

        //float speed = Mathf.Min(velocity.magnitude, maxSpeed);
        //speed = Mathf.Max(speed, minSpeed);

        //if (tether)
        //{
        //    float sqDistToTether = Vector3.SqrMagnitude(tether.position - transform.position);
        //    if (sqDistToTether > tetherRadius * tetherRadius)
        //    {
        //        float speed = velocity.magnitude;
        //        velocity = (tether.position + Random.insideUnitSphere * tetherRadius) - transform.position;
        //        velocity = velocity.normalized * speed;
        //    }
        //}

        //if (detector.hit)
        //{
        //    velocity = detector.project * project + detector.reflect * reflect;
        //}

        //velocity = velocity.normalized * speed;
    }

    public void LateUpdate()
    {
        velocity = cachedVelocity;
        transform.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.0000001f)
            transform.forward = velocity;
    }

    public Vector3 Tether()
    {
        Vector3 direction = tether.position - transform.position;
        //float distance = Vector3.Magnitude(tether.position - transform.position);

        direction *= tetherAmount;

        Debug.Log(direction.magnitude);

        return direction;
    }


    private Vector3 Flock()
    {
        averagePosition = new Vector3();
        averageVelocity = new Vector3();
        averageDifference = new Vector3();
        int count = 0;

        foreach (Boid boid in boids)
        {
            if (boid == this)
                continue;

            float sqDistance = Vector3.SqrMagnitude(boid.transform.position - transform.position);

            if (sqDistance > range * range)
                continue;

            count++;

            averagePosition += boid.transform.position;
            averageVelocity += boid.velocity;
            averageDifference += (transform.position - boid.transform.position);

        }

        if (count <= 0)
            return Vector3.zero;

        averagePosition /= count;
        averageVelocity /= count;
        averageDifference /= count;

        Vector3 cohesion = (averagePosition - transform.position) * this.cohesion;
        Vector3 separation = (averageDifference) * this.separation;
        Vector3 alignment = (averageVelocity - velocity) * this.alignment;

        return cohesion + separation + alignment;
    }
}
