using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using System;

/// <summary>
/// Class used to manage waypoints.
/// </summary>
public class WaypointsManager : MonoBehaviour
{
    /// <summary>
    /// Maximum limits from which grid matching begins.
    /// </summary>
    [SerializeField] private MapBoundries maximumMapLimits = new MapBoundries();
    /// <summary>
    /// Transform of the gameobject that represents the target point.
    /// </summary>
    public Transform target = null;
    /// <summary>
    /// Waypoint Manager Instance. (Singleton pattern)
    /// </summary>
    public static WaypointsManager Instance { get => instance; }
    private static WaypointsManager instance;
    /// <summary>
    /// Map boundaries after grid matching.
    /// </summary>
    private static MapBoundries mapBoundries;
    /// <summary>
    /// Map boundaries after grid matching.
    /// </summary>
    public static MapBoundries MapBoundries { get => mapBoundries; }
    /// <summary>
    /// The size of the waypoints grid.
    /// </summary>
    private static int2 gridSize;
    /// <summary>
    /// The size of the waypoints grid.
    /// </summary>
    public static int2 GridSize { get => gridSize; }

    /// <summary>
    /// Grid of waypoints, stored as array.
    /// </summary>
    public static Waypoint[] waypoints;

    /// <summary>
    /// If the waypoints have been modified, the native array of pathnodes should be updated.
    /// </summary>
    public static bool waypointHasBeenModified = false;
    
    /// <summary>
    /// Native array of pathnodes, made from waypoints data.
    /// </summary>
    public static NativeArray<PathNode> pathNodes;

    /// <summary>
    /// Singleton pattern.
    /// </summary>
    private void Awake()
    {
        if (FindObjectsOfType<WaypointsManager>().Length > 1 && WaypointsManager.Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            WaypointsManager.instance = this;
        }
    }

    /// <summary>
    /// Boundaries matching and creating waypoints grid.
    /// </summary>
    private void Start()
    {
        FindMapSize();
        CreateWaypoints();
    }

    /// <summary>
    /// Adjusting limits to the size of the map.
    /// </summary>
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

    /// <summary>
    /// Function that finds limits of current map by casting rays with passed values.
    /// </summary>
    /// <param name="nRays">Maximum number of casted rays.</param>
    /// <param name="rayLength">Length of ray.</param>
    /// <param name="startPos">The starting position from which the rays are cast.</param>
    /// <param name="rayDirection">The direction of casted rays.</param>
    /// <param name="stepDirection">Vector that specifies the direction of the ray projection step.</param>
    /// <param name="findXnotY">True if you are looking for limits on X axis, or false when looking for limits on Y axis (Z axis in world coords).</param>
    /// <returns></returns>
    private int FindBoundry(int nRays, float rayLength, Vector3 startPos, Vector3 rayDirection, Vector3 stepDirection, bool findXnotY)
    {
        Vector3 rayPos = startPos;
        RaycastHit hit;
        for (int i = 0; i < nRays; i++)
        {
            if (Physics.Raycast(rayPos, rayDirection, out hit, rayLength))
            {
                return findXnotY ? Mathf.RoundToInt(hit.point.x) : Mathf.RoundToInt(hit.point.z);
            }
            rayPos += stepDirection;
        }
        return 0;
    }

    /// <summary>
    /// Function that creates waypoints grid as array structure. 
    /// Waypoints are created with all values ​​and tested to see if they are walkable.
    /// </summary>
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

        UpdatePathNodesNativeArray();
    }

    /// <summary>
    /// Recreated native array of pathnodes from waypoints data.
    /// </summary>
    public static void UpdatePathNodesNativeArray()
    {
        if (pathNodes.IsCreated) pathNodes.Dispose();

        int waypointsArrayLength = WaypointsManager.waypoints.Length;
        pathNodes = new NativeArray<PathNode>(waypointsArrayLength, Allocator.Persistent);
        CreatePathNodesJob createPathNodesJob = new CreatePathNodesJob { pathNodes = pathNodes, waypoints = new NativeArray<Waypoint>(WaypointsManager.waypoints, Allocator.Temp) };
        JobHandle createPathNodesJobHandle = createPathNodesJob.Schedule(waypointsArrayLength, 32);
        createPathNodesJobHandle.Complete();

        WaypointsManager.waypointHasBeenModified = false;
    }

    private void OnDestroy()
    {
        pathNodes.Dispose();
    }

    /// <summary>
    /// Function that checks if waypoint on passed position is walkable.
    /// </summary>
    /// <param name="x">X position on grid.</param>
    /// <param name="y">Y position on grid.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Struct used by Burst Compiler to improve performance.
    /// </summary>
    [BurstCompile]
    public struct Func
    {
        /// <summary>
        /// Function that calculates the index with XY position on a grid in a flat array.
        /// </summary>
        /// <param name="x">X position on grid.</param>
        /// <param name="y">Y position on grid.</param>
        /// <param name="gridSizeX">Width of grid (value of int2.x)</param>
        /// <returns></returns>
        public static int CalculateIndex(int x, int y, int gridSizeX)
        {
            return x + (y * gridSizeX);
        }

        /// <summary>
        /// Function that calculates the index with XY position on a grid in a flat array.
        /// </summary>
        /// <param name="gridXYCoordinates">XY position on grid.</param>
        /// <param name="gridSizeX">Width of grid (value of int2.x)</param>
        /// <returns></returns>
        public static int CalculateIndex(int2 gridXYCoordinates, int gridSizeX)
        {
            return gridXYCoordinates.x + (gridXYCoordinates.y * gridSizeX);
        }

        /// <summary>
        /// Function that calculates position on grid, from world coordinates.
        /// </summary>
        /// <param name="worldPosition">Position in world coordinates.</param>
        /// <returns></returns>
        public static int2 GetGridPositionFromWorldPosition(int2 worldPosition, MapBoundries boundries)
        {
            if (worldPosition.x < boundries.min.x || worldPosition.x > boundries.max.x ||
                worldPosition.y < boundries.min.y || worldPosition.y > boundries.max.y)
            {
                Debug.Log("Invalid target world position!");
                worldPosition = ClampToGrid(worldPosition, boundries);
            }

            int xDistance = worldPosition.x - boundries.min.x;
            int yDistance = worldPosition.y - boundries.min.y;
            return new int2(xDistance, yDistance);
        }

        /// <summary>
        /// Function that calculates position on grid, from world coordinates.
        /// </summary>
        /// <param name="worldPosition">Position in world coordinates.</param>
        /// <returns></returns>
        public static int2 GetGridPositionFromWorldPosition(int2 worldPosition)
        {
            MapBoundries boundries = WaypointsManager.MapBoundries;
            if (worldPosition.x < boundries.min.x || worldPosition.x > boundries.max.x ||
                worldPosition.y < boundries.min.y || worldPosition.y > boundries.max.y)
            {
                Debug.Log("Invalid target world position!");
                worldPosition = ClampToGrid(worldPosition, boundries);
            }

            int xDistance = worldPosition.x - boundries.min.x;
            int yDistance = worldPosition.y - boundries.min.y;
            return new int2(xDistance, yDistance);
        }

        /// <summary>
        /// Function that clamps world position of unit inside global map boundaries.
        /// </summary>
        /// <param name="worldPosition">Position of unit in world coordinates.</param>
        /// <returns></returns>
        public static int2 ClampToGrid(int2 worldPosition, MapBoundries boundries)
        {
            int2 clampedWorldPosition = worldPosition;

            if      (clampedWorldPosition.x < boundries.min.x) clampedWorldPosition.x = boundries.min.x;
            else if (clampedWorldPosition.x > boundries.max.x) clampedWorldPosition.x = boundries.max.x;
            if      (clampedWorldPosition.y < boundries.min.y) clampedWorldPosition.y = boundries.min.y;
            else if (clampedWorldPosition.y > boundries.max.y) clampedWorldPosition.y = boundries.max.y;

            return clampedWorldPosition;
        }

        /// <summary>
        /// Function that calculates index of waypoint on grid, from world coordinates.
        /// </summary>
        /// <param name="worldPosition">Position in world coordinates.</param>
        /// <returns></returns>
        public static int GetIndexFromWorldPosition(int2 worldPosition)
        {
            int2 gridPosition = GetGridPositionFromWorldPosition(worldPosition);
            return CalculateIndex(gridPosition.x, gridPosition.y, gridSize.x);
        }

        /// <summary>
        /// Function that returns random index on array of walkable waypoint.
        /// </summary>
        /// <param name="waypoints">Array of waypoints.</param>
        /// <returns></returns>
        public static int GetRandomWalkableWaypointIndex([ReadOnly] Waypoint[] waypoints)
        {
            int maxID = gridSize.x * gridSize.y;
            int numTries = 1000;
            while (numTries>0)
            {
                int id = UnityEngine.Random.Range(0, maxID);
                if (waypoints[id].isWalkable)
                {
                    return id;
                }
                else
                {
                    numTries--;
                }
            }
            Debug.LogError("Could not find walkable navigation point!");
            return 0;
        }

        /// <summary>
        /// Function that returns world position of random walkable waypoint.
        /// </summary>
        /// <returns></returns>
        public static int2 GetRandomWalkableWaypoint([ReadOnly] Waypoint[] waypoints)
        {
            int maxID = gridSize.x * gridSize.y;
            int numTries = 1000;
            while (numTries > 0)
            {
                int id = UnityEngine.Random.Range(0, maxID);
                if (waypoints[id].isWalkable)
                {
                    return waypoints[id].worldPosition;
                }
                else
                {
                    numTries--;
                }
            }
            Debug.LogError("Could not find walkable navigation point!");
            return 0;
        }

        /// <summary>
        /// Function that rounds world position to int2 grid.
        /// </summary>
        /// <param name="worldPosition">Position in world coordinates where float2.y = v.z in world XZ plane.</param>
        /// <returns></returns>
        public static int2 RoundToGrid([ReadOnly] float2 worldPosition)
        {
            return new int2(math.round(worldPosition));
        }

        /// <summary>
        /// Function that rounds world position to int2 grid.
        /// </summary>
        /// <param name="worldPosition">Position in world coordinates.</param>
        /// <returns></returns>
        public static int2 RoundToGrid([ReadOnly] float3 worldPosition)
        {
            float2 v = new float2(worldPosition.x, worldPosition.z);
            return new int2(math.round(v));
        }

        /// <summary>
        /// Returns rounded to grid world position of target gameobject.
        /// </summary>
        /// <returns></returns>
        public static int2 GetTargetPosition()
        {
            return RoundToGrid(WaypointsManager.Instance.target.position);
        }

        /// <summary>
        /// Returns position of targer waypoint.
        /// </summary>
        /// <returns></returns>
        public static int2 GetTargetPositionFromWaypoint()
        {
            return GetTargetWaypoint().worldPosition;
        }

        /// <summary>
        /// Returns from WaypointManager.Instance an array of waypoints.
        /// </summary>
        /// <returns></returns>
        public static Waypoint[] GetWaypointsArray()
        {
            return WaypointsManager.waypoints;
        }

        /// <summary>
        /// Returns from WaypointManager.Instance an native array of waypoints.
        /// </summary>
        /// <returns></returns>
        public static NativeArray<Waypoint> GetWaypointsNativeArray()
        {
            return new NativeArray<Waypoint>(WaypointsManager.waypoints, Allocator.Persistent);
        }

        /// <summary>
        /// Checks if the target waypoint is walkable.
        /// </summary>
        /// <returns></returns>
        public static bool IsTargetWaypointWalkable()
        {
            return WaypointsManager.waypoints[GetIndexFromWorldPosition(GetTargetPosition())].isWalkable;
        }

        /// <summary>
        /// Return target waypoint.
        /// </summary>
        /// <returns></returns>
        public static Waypoint GetTargetWaypoint()
        {
            return WaypointsManager.waypoints[GetIndexFromWorldPosition(GetTargetPosition())];
        }
    }
}

