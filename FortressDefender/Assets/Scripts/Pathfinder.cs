using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    /*
    private const int directions = 8;
    private const int roundDigits = 3;
    private const float rayHight = 10f;
    private const float rayLength = 20f;

    [Range(0.01f, 10f)] public float gridStep = 0.5f;
    [SerializeField] private Vector3 startPosition = new Vector3(), targetPosition = new Vector3();
    private Waypoint startWaypoint, targetWaypoint;
    [SerializeField] private Vector2 minMapBoundries = -Vector2.one;
    [SerializeField] private Vector2 maxMapBoundries = Vector2.one;
    
    private Vector2[] moveDirections = new Vector2[directions];
    private float[] moveDirectionsMagnitudes = new float[directions];
    private Dictionary<Vector2, Waypoint> grid = new Dictionary<Vector2, Waypoint>();
    private Queue<Waypoint> waypointsQueue = new Queue<Waypoint>();
    private List<Waypoint> path = new List<Waypoint>();

    private bool isRunning = true;
    private Waypoint actualWaypoint;
    private List<Waypoint> waypoints = new List<Waypoint>();

    private void CreateDirectionsArray()
    {
        moveDirections[0] = Vector2.up;
        moveDirections[1] = Vector2.one;
        moveDirections[2] = Vector2.right;
        moveDirections[3] = new Vector2(1f, -1f);
        moveDirections[4] = Vector2.down;
        moveDirections[5] = -Vector2.one;
        moveDirections[6] = Vector2.left;
        moveDirections[7] = new Vector2(-1f, 1f);

        for (int i = 0; i < directions; i++)
        {
            moveDirections[i] *= gridStep;
            moveDirectionsMagnitudes[i] = moveDirections[i].magnitude;
        }
    }

    private void CreateWaypoints()
    {
        for (float xPos = minMapBoundries.x; xPos < maxMapBoundries.x; xPos += gridStep)
        {
            for(float yPos = minMapBoundries.y; yPos < maxMapBoundries.y; yPos += gridStep)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(xPos, rayHight, yPos), Vector3.down, out hit, rayLength))
                {
                    if(hit.collider.tag != "Wall")
                    {
                        Vector3 hitPos = RoundValue(hit.point, roundDigits);
                        Waypoint thisHitWaypoint = new Waypoint(hitPos);
                        grid.Add(new Vector2(hitPos.x, hitPos.z), thisHitWaypoint);
                        waypoints.Add(thisHitWaypoint);
                    }
                }
            }
        }
        startWaypoint = grid[FindClosestWaypoint(startPosition, waypoints)];
        targetWaypoint = grid[FindClosestWaypoint(targetPosition, waypoints)];
    }

    private Vector3 RoundValue(Vector3 point, int digits)
    {
        float scaleFactor = Mathf.Pow(10f, digits);
        Vector3 v = point * scaleFactor;
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        v.z = Mathf.Round(v.z);
        v /= scaleFactor;
        return v;        
    }

    private void Start()
    {
        CreateDirectionsArray();
        CreateWaypoints();
        path = FindPath();
        DrawPath();
    }

    private Vector2 FindClosestWaypoint(Vector3 toThisPoint, List<Waypoint> waypoints)
    {
        Vector2 toThisPoint2D = new Vector2(toThisPoint.x, toThisPoint.z);
        float minDistance = float.MaxValue;
        Vector2 closestWaypoint = new Vector2();
        for (int i = 0; i < waypoints.Count; i++)
        {
            Vector2 thisWaypointPosition2D = waypoints[i].position2D;
            float distance = Vector2.Distance(thisWaypointPosition2D, toThisPoint2D);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestWaypoint = thisWaypointPosition2D;
            }
        }
        return closestWaypoint;
    }

    private List<Waypoint> FindPath()
    {
        isRunning = true;
        List<Waypoint> path = new List<Waypoint>();

        waypointsQueue.Enqueue(startWaypoint);

        while (waypointsQueue.Count > 0 && isRunning)
        {
            actualWaypoint = waypointsQueue.Dequeue();
            Debug.DrawLine(actualWaypoint.position, actualWaypoint.position + Vector3.up, Color.red, 30f);
            StopIfPathFound(actualWaypoint, targetWaypoint);
            actualWaypoint.isExplored = ExploreNeighbours();
        }
        
        path.Add(targetWaypoint);
        Waypoint previous = targetWaypoint.exploredFrom;
        while (previous != startWaypoint)
        {
            path.Add(previous);
            previous = previous.exploredFrom;
        }
        path.Add(startWaypoint);
        path.Reverse();
        
        return path;
    }

    private bool ExploreNeighbours()
    {
        if (!isRunning) { return true; }

        for(int i = 0; i < directions; i++)
        {
            Vector2 neighbourPos2D = actualWaypoint.position2D + moveDirections[i];
            if (grid.ContainsKey(neighbourPos2D))
            {
                Debug.Log("Trying to queue new neighbours.");
                QueueNewNeighbours(neighbourPos2D);
            }
        }

        return true;
    }

    private void QueueNewNeighbours(Vector2 neighbourPos2D)
    {
        Waypoint neighbour = grid[neighbourPos2D];
        if (neighbour.isExplored || waypointsQueue.Contains(neighbour))
        {
            // nothing
        }
        else
        {
            waypointsQueue.Enqueue(neighbour);
            neighbour.exploredFrom = actualWaypoint;
            Debug.Log("Next neighbour queued.");
        }
    }

    private void StopIfPathFound(Waypoint thisWaypoint, Waypoint targetWaypoint)
    {
        if (thisWaypoint == targetWaypoint)
        {
            isRunning = false;
            Debug.Log("Target has been found, pathfinding stopped.");
        }
    }

    private void DrawPath()
    {
        float debugTime = 10f;

        Debug.Log("Path consist " + path.Count + " nodes.");
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i].position + Vector3.up, path[i + 1].position + Vector3.up, Color.blue, debugTime);
        }
        Debug.DrawLine(path[0].position, path[0].position + (Vector3.up * 5f), Color.green, debugTime);
        Debug.DrawLine(path[path.Count - 1].position, path[path.Count - 1].position + (Vector3.up * 5f), Color.yellow, debugTime);
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.Clear();
            CreateWaypoints();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                targetPosition = hit.point;
            }
            //targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetWaypoint = grid[FindClosestWaypoint(targetPosition, waypoints)];
            path.Clear();
            path = FindPath();
            DrawPath();
        }
        if (Input.GetMouseButtonDown(1))
        {
            grid.Clear();
            CreateWaypoints();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                startPosition = hit.point;
                startWaypoint = grid[FindClosestWaypoint(startPosition, waypoints)];
            }
        }
    }

    */
}