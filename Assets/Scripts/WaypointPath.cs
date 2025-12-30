using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public List<Transform> waypoints = new();

    private void Reset()
    {
        waypoints.Clear();
        foreach (Transform child in transform)
            waypoints.Add(child);
    }

    public int Count => waypoints.Count;
    public Transform Get(int i) => waypoints[i];
}
