using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TrajectoryFunctions {

    public static Vector3[] GetTrajectoryPath(Vector3 IniVelocity, Vector3 StartPos, int NumOfPoints, float TotalTime)
    {
        Vector3[] Points = new Vector3[NumOfPoints];

        for (int i = 0; i < NumOfPoints; i++)
        {
            float timeMarker = TotalTime * ((float)i / (NumOfPoints));

            float currentX = IniVelocity.x * timeMarker;
            float currentY = IniVelocity.y * timeMarker + (0.5f * Physics.gravity.y * (timeMarker * timeMarker));
            float currentZ = IniVelocity.z * timeMarker;

            Points[i] = (StartPos + new Vector3(currentX, currentY, currentZ));
        }

        return Points;
    }

    public static float TimeToReachTarget(Vector3 IniVelocity, float LaunchAngle, float StartYHeight, float TargetYHeight)
    {
        float heightDiff = StartYHeight - TargetYHeight;
        float initialYVelocity = IniVelocity.magnitude * Mathf.Sin(LaunchAngle * Mathf.Deg2Rad);
        float initialYVelocitySquared = initialYVelocity * initialYVelocity;
        float twoGravityHeight = 2 * (Physics.gravity.magnitude * heightDiff);
        float result = initialYVelocitySquared + twoGravityHeight;
        float finalYVelocity = Mathf.Sqrt(result);
        float time = (-finalYVelocity - initialYVelocity) / -Physics.gravity.magnitude;

        return time;
    }
}
