using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using System;

public class WaypointsManager : MonoBehaviour
{
    [SerializeField] private MapBoundries maximumMapLimits = new MapBoundries();
    public Transform target = null;

    public static WaypointsManager Instance;
    private static MapBoundries mapBoundries;
    public static MapBoundries MapBoundries { get => mapBoundries; }
    private static int2 gridSize;
    public static int2 GridSize { get => gridSize; }

    [HideInInspector] public Waypoint[] waypoints;

    private void Awake()
    {
        if (FindObjectsOfType<WaypointsManager>().Length > 1 && WaypointsManager.Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            WaypointsManager.Instance = this;
        }
    }

    private void Start()
    {
        FindMapSize();
        CreateWaypoints();
    }

    private void FindMapSize()
    {
        MapBoundries limitSize = maximumMapLimits;
        MapBoundries boundries;
        boundries.min.y = FindBoundry(limitSize.max.y - limitSize.min.y + 1, limitSize.max.x - limitSize.min.x + 1,
                                      new Vector3(limitSize.min.x, 0f, limitSize.min.y), Vector3.right, Vector3.forward, false);
        boundries.max.y = FindBoundry(limitSize.max.y - boundries.min.y + 1, limitSize.max.x - limitSize.min.x + 1,
                                      new Vector3(limitSize.min.x, 0f, limitSize.max.y), Vector3.right, Vector3.back , false);
        boundries.min.x = FindBoundry(limitSize.max.x - limitSize.min.x + 1, boundries.max.y - boundries.min.y + 1,
                                      new Vector3(limitSize.min.x, 0f, boundries.min.y), Vector3.forward, Vector3.right, true);
        boundries.max.x = FindBoundry(limitSize.max.x - boundries.min.x + 1, boundries.max.y - boundries.min.y + 1,
                                      new Vector3(limitSize.max.x, 0f, boundries.min.x), Vector3.forward, Vector3.left, true);
        mapBoundries = boundries;
    }

    private int FindBoundry(int nRays, float rayLength, Vector3 startPos, Vector3 rayDirection, Vector3 stepDirection, bool findXorY)
    {
        Vector3 rayPos = startPos;
        RaycastHit hit;
        for (int i = 0; i < nRays; i++)
        {
            if (Physics.Raycast(rayPos, rayDirection, out hit, rayLength))
            {
                return findXorY ? Mathf.RoundToInt(hit.point.x) : Mathf.RoundToInt(hit.point.z);
            }
            rayPos += stepDirection;
        }
        return 0;
    }

    private void CreateWaypoints()
    {
        gridSize = mapBoundries.max - mapBoundries.min + new int2(1, 1);
        waypoints = new Waypoint[gridSize.x * gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                int2 gridPosition = new int2(x, y);
                int2 worldPosition = new int2(mapBoundries.min.x + x, mapBoundries.min.y + y);
                int index = Func.CalculateIndex(x, y, gridSize.x);
                bool isWalkable = CheckIfWaypointIsWalkable(x, y);
                waypoints[index] = new Waypoint
                {
                    gridPosition = gridPosition,
                    worldPosition = worldPosition,
                    index = index,
                    isWalkable = isWalkable
                };
            }
        }
    }

    private bool CheckIfWaypointIsWalkable(int x, int y)
    {
        float rayLength = 10f;
        Vector3 rayPosition = new Vector3(x + mapBoundries.min.x, rayLength / 2f, y + mapBoundries.min.y);
        RaycastHit hit;
        if (Physics.Raycast(rayPosition, Vector3.down, out hit, rayLength))
        {
            if (hit.collider.tag == "Floor")
            {
                return true;
            }
        }
        return false;
    }

    [BurstCompile]
    public struct Func
    {
        public static int CalculateIndex(int x, int y, int gridSizeX)
        {
            return x + (y * gridSizeX);
        }

        public static int2 GetGridPositionFromWorldPosition(int2 worldPosition)
        {
            if (worldPosition.x < mapBoundries.min.x || worldPosition.x > mapBoundries.max.x ||
                worldPosition.y < mapBoundries.min.y || worldPosition.y > mapBoundries.max.y)
                Debug.LogError("Invalid world position!");

            int xDistance = worldPosition.x - mapBoundries.min.x;
            int yDistance = worldPosition.y - mapBoundries.min.y;
            return new int2(xDistance, yDistance);
        }

        public static int GetIndexFromWorldPosition(int2 worldPosition)
        {
            int2 gridPosition = GetGridPositionFromWorldPosition(worldPosition);
            return CalculateIndex(gridPosition.x, gridPosition.y, gridSize.x);
        }

        public static int GetRandomWalkableWaypointIndex(Waypoint[] waypoints)
        {
            int maxID = gridSize.x * gridSize.y;
            while (true)
            {
                int id = UnityEngine.Random.Range(0, maxID);
                if (waypoints[id].isWalkable)
                {
                    return id;
                }
            }
        }

        public static int2 GetRandomWalkableWaypoint()
        {
            Waypoint[] waypoints = WaypointsManager.Instance.waypoints;
            int maxID = gridSize.x * gridSize.y;
            while (true)
            {
                int id = UnityEngine.Random.Range(0, maxID);
                if (waypoints[id].isWalkable)
                {
                    return waypoints[id].worldPosition;
                }
            }
        }

        public static int2 RoundToGrid(int2 worldPosition)
        {
            return new int2(math.round(worldPosition));
        }

        public static int2 RoundToGrid(float3 worldPosition)
        {
            float2 v = new float2(worldPosition.x, worldPosition.z);
            return new int2(math.round(v));
        }

        public static int2 GetTargetPosition()
        {
            return RoundToGrid(WaypointsManager.Instance.target.position);
        }
    }
}


/// <summary>
/// Structure defining the limits of the grid. 
/// The min and max values mean the plane values, 
/// where the XY plane for int2 means the XZ plane in global coordinates.
/// </summary>
[BurstCompile]
[Serializable]
public struct MapBoundries
{
    public int2 min;
    public int2 max;
}
