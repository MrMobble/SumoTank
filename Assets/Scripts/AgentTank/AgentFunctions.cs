using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AgentFunctions
{
    public static bool CheckTarget(Transform Target)
    {
        if (Target == null)
        {
            Debug.Log("TargetNull");
            return true;
        }
        else if (!Target.gameObject.activeInHierarchy)
        {
            Debug.Log("TargetNotActive");
            return true;
        }
        return false;
    }

    public static bool CheckTargetLocation(Vector3 Target)
    {
        if (Target == new Vector3(0.0f, 0.0f, 0.0f))
        {
            return true;
        }
        return false;
    }

    public static bool CheckDistanceToLocation(Transform transform, Vector3 Target, float MinDistance)
    {
        if ((transform.position - Target).magnitude < MinDistance)
        {
            return true;
        }
        return false;
    }

    public static bool CheckForDeath(Transform ObjTransform, float MinYLocation)
    {
        if (ObjTransform.position.y < MinYLocation)
        {
            return true;
        }
        return false;
    }

    public static Quaternion GetTargetRotation(Transform target, Transform transform)
    {
        Rigidbody targetBody = target.GetComponent<Rigidbody>();

        Vector3 LookDir = target.position + targetBody.velocity - transform.position;
        LookDir.y = 0;

        return Quaternion.LookRotation(LookDir);
    }

    public static bool GetTargetAngle(Component Turret, Transform target, Transform transform, float ShootRange)
    {
        Rigidbody targetBody = target.GetComponent<Rigidbody>();

        Vector3 LookDir = target.position + targetBody.velocity - transform.position;
        LookDir.y = 0;

        if (Vector3.Angle(Turret.transform.forward, LookDir) < ShootRange)
        {
            return true;
        }

        return false;
    }

    public static float AngleDir(Vector3 Forward, Vector3 TargetDir, Vector3 Up)
    {
        Vector3 Perp = Vector3.Cross(Forward, TargetDir);
        float Dir = Vector3.Dot(Perp, Up);

        if (Dir > 0.0f) return 1.0f;
        else if (Dir < 0.0f) return -1.0f;
        else return 0.0f;
    }

    public static Vector3 BallisticVel(Transform transform, Transform target, float angle)
    {
        var Direction = target.position - transform.position;   //Get target direction
        var Height = Direction.y;                               //Get height difference
        Direction.y = 0;                                        //Retain only the horizontal direction
        var Distance = Direction.magnitude;                     //Get horizontal distance
        var AToRadians = angle * Mathf.Deg2Rad;                 //Convert angle to radians
        Direction.y = Distance * Mathf.Tan(AToRadians);         //Set dir to the elevation angle
        Distance += Height / Mathf.Tan(AToRadians);             //Correct for small height differences

        //Calculate the velocity magnitude
        var Velocity = Mathf.Sqrt(Distance * Physics.gravity.magnitude / Mathf.Sin(2 * AToRadians));
        return Velocity * Direction.normalized;
    }

}
