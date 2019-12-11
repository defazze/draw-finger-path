using System;
using System.Collections.Generic;
using UnityEngine;

public static class TrackExtension
{
    public static void Interpolate(this List<Vector3> track, Vector3 newPoint, float step, Action<Vector3> onAddPoint)
    {
        if (track.Count > 0)
        {
            var lastPoint = track[track.Count - 1];
            var currentPoint = lastPoint;
            var distance = Vector3.Distance(currentPoint, newPoint);

            while (distance > step)
            {
                var percent = step / distance;
                currentPoint = Vector3.Lerp(currentPoint, newPoint, percent);
                distance = Vector3.Distance(currentPoint, newPoint);

                if (distance >= step)
                {
                    onAddPoint(currentPoint);
                };
            }

            if (Vector3.Distance(lastPoint, newPoint) >= step)
            {
                onAddPoint(newPoint);
            }
        }
        else
        {
            onAddPoint(newPoint);
        }
    }
}