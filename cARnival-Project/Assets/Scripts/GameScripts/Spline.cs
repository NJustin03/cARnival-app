
using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour
{
    private List<Vector3> points = new List<Vector3>();

    public void GenerateSpline(Vector3 start, Vector3 end, Vector3 control)
    {
        points.Clear();
        points.Add(start);
        points.Add(control);
        points.Add(end);

        Debug.Log($"Spline generated with points: {start}, {control}, {end}");
    }

    public Vector3 GetPoint(float t)
    {
        if (points.Count < 3)
        {
            Debug.LogWarning("Not enough points to form a spline");
            return Vector3.zero;
        }

        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * points[0];
        p += 2 * u * t * points[1];
        p += tt * points[2];

        return p;
    }
}
