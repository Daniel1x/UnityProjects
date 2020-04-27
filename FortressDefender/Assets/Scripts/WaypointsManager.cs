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
    [SerializeField] private MapBoundries maximumMapLimits;

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
                int index = CalculateIndex(x, y, gridSize.x);
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

    private static int CalculateIndex(int x,int y,int gridSizeX)
    {
        return x + (y * gridSizeX);
    }

    private static int2 GetGridPositionFromWorldPosition(int2 worldPosition)
    {
        if (worldPosition.x < mapBoundries.min.x || worldPosition.x > mapBoundries.max.x || 
            worldPosition.y < mapBoundries.min.y || worldPosition.y > mapBoundries.max.y)
            Debug.LogError("Invalid world position!");

        int xDistance = worldPosition.x - mapBoundries.min.x;
        int yDistance = worldPosition.y - mapBoundries.min.y;
        return new int2(xDistance, yDistance);
    }

    private static int GetIndexFromWorldPosition(int2 worldPosition)
    {
        int2 gridPosition = GetGridPositionFromWorldPosition(worldPosition);
        return CalculateIndex(gridPosition.x, gridPosition.y, gridSize.x);
    }
}

[Serializable]
public struct MapBoundries
{
    public int2 min;
    public int2 max;
}
