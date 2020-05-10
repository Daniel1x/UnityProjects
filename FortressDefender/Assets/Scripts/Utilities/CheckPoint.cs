using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Vector3 lastPosition = new Vector3();

    private void Update()
    {
        if (!transform.position.Equals(lastPosition))
        {
            lastPosition = transform.position;
            WaypointsManager.waypointHasBeenModified  = true;
            WaypointsManager.waypointHasBeenUnlocked = true;
        }
    }
}
