using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPathLine : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField, Min(3)]
    private int lineSegments = 60;

    [SerializeField, Min(1)]
    private float timeOfTheFlight = 5;

    public void ShowTrajectoryLine(Vector3 startpoint, Vector3 startVelocity)
    {
        float timestep = timeOfTheFlight / lineSegments;

        Vector3[] lineRendererPoints = CalculateTrajectoryLine(startpoint, startVelocity, timestep);

        lineRenderer.positionCount = lineSegments;
        lineRenderer.SetPositions(lineRendererPoints);
    }

    private Vector3[] CalculateTrajectoryLine(Vector3 startpoint, Vector3 startVelocity, float timeStep)
    {
        Vector3[] lineRendererPoints = new Vector3[lineSegments];

        lineRendererPoints[0] = startpoint;

        for (int i = 1; i < lineSegments; i++)
        {
            float timeOffset = timeStep * i;

            Vector3 progressBeforeGravity = startVelocity * timeOffset;
            //Arrow not affected by gravity
            Vector3 gravityOffset = Vector3.zero;
            Vector3 newPosition = startpoint + progressBeforeGravity - gravityOffset;
            lineRendererPoints[i] = newPosition;

        }

        return lineRendererPoints;
    }
}

