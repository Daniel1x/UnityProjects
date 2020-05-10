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
    [SerializeField] private MapBoundaries maximumMapLimits = new MapBoundaries();
    /// <summary>
    /// Transform of the gameobject that represents the spawn point.
    /// </summary>
    public Transform spawnPoint = null;
    /// <summary>
    /// Transform of the gameobject that represents the target point.
    /// </summary>
    public Transform targetPoint = null;
    /// <summary>
    /// Waypoint Manager Instance. (Singleton pattern)
    /// </summary>
    public static WaypointsManager Instance { get => instance; }
    private static WaypointsManager instance;
    /// <summary>
    /// Map boundaries after grid matching.
    /// </summary>
    private static MapBoundaries mapBoundaries;
    /// <summary>
    /// Map boundaries after grid matching.
    /// </summary>
    public static MapBoundaries MapBoundaries { get => mapBoundaries; }
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
    public static bool waypointHasBeenModified = true;
    /// <summary>
    /// If the waypoints have been unlocked, then new shortest path could be available.
    /// </summary>
    public static bool waypointHasBeenUnlocked = false;
    /// <summary>
    /// Native array of pathnodes, made from waypoints data.
    /// </summary>
    public static NativeArray<PathNode> pathNodes;
    /// <summary>
    /// Native array of path positions.
    /// </summary>
    public static NativeArray<PathPositionsBuffer> globalPath;
    /// <summary>
    /// Length of global path.
    /// </summary>
    public static int globalPathLength { get => globalPathLengthPlaceholder[0]; }
    /// <summary>
    /// NativeArray that holds value of global path length.
    /// </summary>
    private static NativeArray<int> globalPathLengthPlaceholder;
    /// <summary>
    /// NativeArray that holds path values.
    /// </summary>
    private static NativeArray<PathPositionsBuffer> globalPathPlaceholder;
    /// <summary>
    /// List of waypoints that was modified.
    /// </summary>
    public static List<float3> lastBlockedWaypoints;

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
            globalPathLengthPlaceholder = new NativeArray<int>(1, Allocator.Persistent);
            globalPathPlaceholder = new NativeArray<PathPositionsBuffer>(1024, Allocator.Persistent);
            lastBlockedWaypoints = new List<float3>();
        }
    }

    /// <summary>
    /// Boundaries matching and creating waypoints grid.
    /// </summary>
    private void Start()
    {
        mapBoundaries = FindMapSize(maximumMapLimits);
        CreateWaypoints();
        Func.UpdatePathNodesNativeArray();
    }

    /// <summary>
    /// Release of allocated memory.
    /// </summary>
    private void OnDestroy()
    {
        if (pathNodes.IsCreated) pathNodes.Dispose();
        if (globalPath.IsCreated) globalPath.Dispose();
        if (globalPathLengthPlaceholder.IsCreated) globalPathLengthPlaceholder.Dispose();
        if (globalPathPlaceholder.IsCreated) globalPathPlaceholder.Dispose();
    }

    /// <summary>
    /// Adjusting limits to the size of the map.
    /// </summary>
    /// <param name="maximumMapLimits">Maximum size of map.</param>
    /// <returns></returns>
    private static MapBoundaries FindMapSize(MapBoundaries maximumMapLimits)
    {
        MapBoundaries limitSize = maximumMapLimits;
        MapBoundaries boundaries;
        boundaries.min.y = FindBoundry(limitSize.max.y - limitSize.min.y + 1, limitSize.max.x - limitSize.min.x + 1,
                                      new Vector3(limitSize.min.x, 0f, limitSize.min.y), Vector3.right, Vector3.forward, false);
        boundaries.max.y = FindBoundry(limitSize.max.y - boundaries.min.y + 1, limitSize.max.x - limitSize.min.x + 1,
                                      new Vector3(limitSize.min.x, 0f, limitSize.max.y), Vector3.right, Vector3.back , false);
        boundaries.min.x = FindBoundry(limitSize.max.x - limitSize.min.x + 1, boundaries.max.y - boundaries.min.y + 1,
                                      new Vector3(limitSize.min.x, 0f, boundaries.min.y), Vector3.forward, Vector3.right, true);
        boundaries.max.x = FindBoundry(limitSize.max.x - boundaries.min.x + 1, boundaries.max.y - boundaries.min.y + 1,
                                      new Vector3(limitSize.max.x, 0f, boundaries.min.x), Vector3.forward, Vector3.left, true);
        return boundaries;
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
    private static int FindBoundry(int nRays, float rayLength, Vector3 startPos, Vector3 rayDirection, Vector3 stepDirection, bool findXnotY)
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
    private static void CreateWaypoints()
    {
        gridSize = mapBoundaries.max - mapBoundaries.min + new int2(1, 1);
        waypoints = new Waypoint[gridSize.x * gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                int2 gridPosition = new int2(x, y);
                int2 worldPosition = new int2(mapBoundaries.min.x + x, mapBoundaries.min.y + y);
                int index = Func.CalculateIndex(x, y, gridSize.x);
                bool isWalkable = CheckIfWaypointIsWalkable(x, y);
                waypoints[index] = new Waypoint
                {
                    gridPosition = gridPosition,
                    worldPosition = worldPosition,
                    index = index,
                    isBlockedByWall = !isWalkable,
                    isWalkable = isWalkable
                };
            }
        }

        Func.UpdatePathNodesNativeArray();
    }
    
    /// <summary>
    /// Function that checks if waypoint on passed position is walkable.
    /// </summary>
    /// <param name="x">X position on grid.</param>
    /// <param name="y">Y position on grid.</param>
    /// <returns></returns>
    public static bool CheckIfWaypointIsWalkable(int x, int y)
    {
        float rayLength = 10f;
        Vector3 rayPosition = new Vector3(x + mapBoundaries.min.x, rayLength / 2f, y + mapBoundaries.min.y);
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
        /// <param name="worldPosition"></param>
        /// <param name="boundaries"></param>
        /// <returns></returns>
        public static int2 GetGridPositionFromWorldPosition(float3 worldPosition, MapBoundaries boundaries)
        {
            return GetGridPositionFromWorldPosition(RoundToGrid(worldPosition), boundaries);
        }

        /// <summary>
        /// Function that calculates position on grid, from world coordinates.
        /// </summary>
        /// <param name="worldPosition">Position in world coordinates.</param>
        /// <param name="boundaries">Global map boundaries.</param>
        /// <returns></returns>
        public static int2 GetGridPositionFromWorldPosition(int2 worldPosition, MapBoundaries boundaries)
        {
            if (worldPosition.x < boundaries.min.x || worldPosition.x > boundaries.max.x ||
                worldPosition.y < boundaries.min.y || worldPosition.y > boundaries.max.y)
            {
                Debug.Log("Invalid target world position!");
                worldPosition = ClampToGrid(worldPosition, boundaries);
            }

            int xDistance = worldPosition.x - boundaries.min.x;
            int yDistance = worldPosition.y - boundaries.min.y;
            return new int2(xDistance, yDistance);
        }

        /// <summary>
        /// Function that calculates position on grid, from world coordinates.
        /// </summary>
        /// <param name="worldPosition">Position in world coordinates.</param>
        /// <returns></returns>
        public static int2 GetGridPositionFromWorldPosition(int2 worldPosition)
        {
            MapBoundaries boundaries = WaypointsManager.MapBoundaries;
            if (worldPosition.x < boundaries.min.x || worldPosition.x > boundaries.max.x ||
                worldPosition.y < boundaries.min.y || worldPosition.y > boundaries.max.y)
            {
                Debug.Log("Invalid target world position!");
                worldPosition = ClampToGrid(worldPosition, boundaries);
            }

            int xDistance = worldPosition.x - boundaries.min.x;
            int yDistance = worldPosition.y - boundaries.min.y;
            return new int2(xDistance, yDistance);
        }

        /// <summary>
        /// Function that clamps world position of unit inside global map boundaries.
        /// </summary>
        /// <param name="worldPosition">Position of unit in world coordinates.</param>
        /// <returns></returns>
        public static int2 ClampToGrid(int2 worldPosition, MapBoundaries boundaries)
        {
            int2 clampedWorldPosition = worldPosition;

            if      (clampedWorldPosition.x < boundaries.min.x) clampedWorldPosition.x = boundaries.min.x;
            else if (clampedWorldPosition.x > boundaries.max.x) clampedWorldPosition.x = boundaries.max.x;
            if      (clampedWorldPosition.y < boundaries.min.y) clampedWorldPosition.y = boundaries.min.y;
            else if (clampedWorldPosition.y > boundaries.max.y) clampedWorldPosition.y = boundaries.max.y;

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
        /// Function that round world position to int2 grid, but returns float3 value.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static float3 RoundToGridInWorld([ReadOnly] float3 worldPosition)
        {
            return new float3(math.round(worldPosition));
        }

        /// <summary>
        /// Returns rounded to grid world position of target gameobject.
        /// </summary>
        /// <returns></returns>
        public static int2 GetTargetPosition()
        {
            return RoundToGrid(WaypointsManager.Instance.targetPoint.position);
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
            return WaypointsManager.Func.GetTargetWaypoint().isWalkable;
        }

        /// <summary>
        /// Return target waypoint.
        /// </summary>
        /// <returns></returns>
        public static Waypoint GetTargetWaypoint()
        {
            return WaypointsManager.waypoints[GetIndexFromWorldPosition(GetTargetPosition())];
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

            UpdateGlobalPathToTargetNativeArray();

            WaypointsManager.waypointHasBeenModified = false;
        }

        /// <summary>
        /// Calculates new global path to target.
        /// </summary>
        public static void UpdateGlobalPathToTargetNativeArray()
        {
            if (!WaypointsManager.waypointHasBeenModified) return;
            
            if (!WaypointsManager.Func.IsGlobalPathBlocked() && !WaypointsManager.waypointHasBeenUnlocked) return;

            new FindGlobalPathJob
            {
                startGridPosition = Func.GetGridPositionFromWorldPosition(WaypointsManager.instance.spawnPoint.position, mapBoundaries),
                endGridPosition = Func.GetGridPositionFromWorldPosition(WaypointsManager.instance.targetPoint.position, mapBoundaries),
                gridSize = gridSize,
                pathNodes = new NativeArray<PathNode>(pathNodes, Allocator.TempJob),
                pathPositionsBuffer = globalPathPlaceholder,
                pathSize = WaypointsManager.globalPathLengthPlaceholder
            }.Run();

            if (globalPath.IsCreated) globalPath.Dispose();

            globalPath = new NativeArray<PathPositionsBuffer>(globalPathLength, Allocator.Persistent);
            NativeArray<PathPositionsBuffer>.Copy(globalPathPlaceholder, globalPath, globalPathLength);

            WaypointsManager.waypointHasBeenUnlocked = false;

            Debug.Log("A global path has been determined.");
            for (int i = 0; i < globalPath.Length - 1; i++)
            {
                Debug.DrawLine(math.up() + globalPath[i].worldPosition, math.up() + globalPath[i + 1].worldPosition, Color.yellow, 5f);
            }
        }

        /// <summary>
        /// Function that checks if global path was blocked.
        /// </summary>
        /// <param name="globalPath">Global path.</param>
        /// <param name="blockedWaypoints">Blocked waypoints positions.</param>
        /// <returns></returns>
        private static bool IsGlobalPathBlocked()
        {
            if (!globalPath.IsCreated) return true;

            for(int waypointIndex = 0; waypointIndex < lastBlockedWaypoints.Count; waypointIndex++)
            {
                for (int pathPointIndex = 0; pathPointIndex < globalPath.Length; pathPointIndex++)
                {
                    if (globalPath[pathPointIndex].worldPosition.Equals(lastBlockedWaypoints[waypointIndex]))
                    {
                        lastBlockedWaypoints.Clear();
                        return true;
                    }
                }
            }
            lastBlockedWaypoints.Clear();
            return false;
        }
    }
}

