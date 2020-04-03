using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MapCreator : MonoBehaviour {
    private const float CAM_Z_OFFSET = -10f;
    private const float DOT_Z_OFFSET = -0.5f;

    [SerializeField] private GameObject dotPrefab;
    [SerializeField] [Range(6, 20)] private int xSize;
    [SerializeField] [Range(6, 20)] private int ySize;

    public Vector2Int GetSize() { return new Vector2Int(xSize, ySize); }

    private List<Vector2Int> boundariesPoints = new List<Vector2Int>();
    private Vector2Int[] goalPoints = new Vector2Int[6];
    private List<Vector2Int> playablePoints = new List<Vector2Int>();

    /// <summary>
    /// Creating visible dots and lines of boundries.
    /// </summary>
    /// <param name="xSize">Width</param>
    /// <param name="ySize">Height</param>
    void CreateMap(int xSize, int ySize)
    {
        int xDots = xSize + 1; 
        int yDots = ySize + 1;
        // Spawning map dots
        for(int y = 0; y < yDots; y++)
        {
            for(int x = 0; x < xDots; x++)
            {
                SpawnOneDot(x, y);
            }
        }
        // Spawning goal dots
        int xPos;
        int yPos;
        for (int i = 0; i < 3; i++)
        {
            xPos = Mathf.RoundToInt(xSize / 2 - 1 + i);
            yPos = yDots;

            SpawnOneDot(xPos, yPos, i);

            xPos = Mathf.RoundToInt(xSize / 2 - 1 + i);
            yPos = -1;

            SpawnOneDot(xPos, yPos, i + 3);
        }

        MoveCameraToMiddle(xDots, yDots);
    }

    /// <summary>
    /// Spawning one dot on screen, in xy position.
    /// </summary>
    /// <param name="xPos"></param>
    /// <param name="yPos"></param>
    /// <param name="index">If defined, means adding a point to the goals table.</param>
    private void SpawnOneDot(int xPos, int yPos, int index = -1)
    {
        if (index != -1) AddToGoalPointsArray(xPos, yPos, index);
        AddToBoundriesList(xPos, yPos);
        AddToPlayablePointsList(xPos, yPos);
        Instantiate(dotPrefab, new Vector3(xPos, yPos, DOT_Z_OFFSET), Quaternion.identity, this.transform);
    }

    private void AddToPlayablePointsList(int xPos, int yPos)
    {
        playablePoints.Add(new Vector2Int(xPos, yPos));
    }

    private void AddToGoalPointsArray(int xPos, int yPos, int index)
    {
        goalPoints[index] = new Vector2Int(xPos, yPos);
    }

    private void AddToBoundriesList(int x, int y)
    {
        int xPosOfGoalMiddle = xSize / 2;
        if ((x == 0 || x == xSize || y <= 0 || y >= ySize)) // Checking if position is out of boundaries.
        {
            if (x == xPosOfGoalMiddle && (y == 0 || y == ySize)) return; // If it is middle of goal, return.

            Vector2Int spawnPosInt = new Vector2Int(x, y);
            if (!boundariesPoints.Contains(spawnPosInt))
            {
                boundariesPoints.Add(spawnPosInt);
            }
        }
    }

    /// <summary>
    /// Moves the camera to fit the board.
    /// </summary>
    /// <param name="xDots"></param>
    /// <param name="yDots"></param>
    private void MoveCameraToMiddle(int xDots, int yDots)
    {
        Camera cam = Camera.main;
        Vector3 middlePos = new Vector3(xDots / 2, yDots / 2, CAM_Z_OFFSET);
        cam.transform.position = middlePos;
    }

    private void Awake()
    {
        CreateMap(xSize, ySize);
        DrawBoundaries();
    }

    private void DrawBoundaries()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = boundariesPoints.Count;
        lineRenderer.SetPositions(GetBoundriesPositions());
        lineRenderer.loop = true;
    }

    private Vector3[] GetBoundriesPositions()
    {
        List<Vector2Int> sortedBoundariesPoints = new List<Vector2Int>();   // List of sorted boundaries points.
        sortedBoundariesPoints.Add(boundariesPoints[0]);                    // Setting start point.
        Vector2Int[] directions = Directions.directionsArray();             // Getting directions array.
        
        Vector2Int actualPoint = boundariesPoints[0];                       // Placeholder of moving point.
        for (int i = 0; i < boundariesPoints.Count; i++)
        {
            for (int dir = 0; dir < Directions.arraySize; dir++) 
            {
                Vector2Int neighborPoint = actualPoint + directions[dir];
                if (boundariesPoints.Contains(neighborPoint) && !sortedBoundariesPoints.Contains(neighborPoint))
                {
                    sortedBoundariesPoints.Add(neighborPoint);
                    actualPoint = neighborPoint;
                    break;
                }
            }
        }
        boundariesPoints.Clear();
        boundariesPoints = sortedBoundariesPoints;

        Vector3[] boundaries = new Vector3[boundariesPoints.Count];
        for(int i = 0; i < boundariesPoints.Count; i++)                     // Filling array form list.
        {
            Vector2Int thisPoint = sortedBoundariesPoints[i];
            boundaries[i] = new Vector3(thisPoint.x, thisPoint.y, -1f);
        }
        return boundaries;
    }

    public List<Vector2Int> GetBoundriesPositionsArray()
    {
        return boundariesPoints;
    }

    public List<Vector2Int> GetPlayablePoints()
    {
        return playablePoints;
    }

    public Vector2Int[] GetGoalPoints()
    {
        return goalPoints;
    }

    public Vector2Int GetStartPoint()
    {
        return new Vector2Int(xSize / 2, ySize / 2);
    }

    public void RestartGame()
    {
        boundariesPoints.Clear();
        goalPoints = new Vector2Int[6];
        playablePoints.Clear();
        DestroyOldMap();
        CreateMap(xSize, ySize);
        DrawBoundaries();
    }

    private void DestroyOldMap()
    {
        int numberOfChilds = transform.childCount;
        Transform[] childs = GetComponentsInChildren<Transform>();
        for (int index = 1; index < numberOfChilds + 1; index++)
        {
            Destroy(childs[index].gameObject);
        }
    }
}