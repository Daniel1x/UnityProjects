using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;

public class Pathfinding : MonoBehaviour
{
    //TESTING
    [SerializeField] Transform startPos = null;
    [SerializeField] Transform endPos = null;
    public void FindPathToThePoint()
    {
        float startTime = Time.realtimeSinceStartup;
        int2 startPosOnGrid = WorldPositionToGrid(new int2(Mathf.RoundToInt(startPos.position.x), Mathf.RoundToInt(startPos.position.z)), minPositionOnMap, maxPositionOnMap, gridWidth);
        int2 endPosOnGrid = WorldPositionToGrid(new int2(Mathf.RoundToInt(endPos.position.x), Mathf.RoundToInt(endPos.position.z)), minPositionOnMap, maxPositionOnMap, gridWidth);
        //FindPath(startPosOnGrid, endPosOnGrid);

        int findPathJobCount = 8;
        NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(findPathJobCount, Allocator.Temp);
        for(int i = 0; i < findPathJobCount; i++)
        {
            FindPathJob findPathJob = new FindPathJob
            {
                pathNodeArray = pathNodeArray,
                gridSize = gridSize,
                gridWidth = gridWidth,
                startPosition = startPosOnGrid,
                endPosition = endPosOnGrid,
                minPositionOnMap = minPositionOnMap,
                maxPositionOnMap = maxPositionOnMap
            };
            jobHandleArray[i] = findPathJob.Schedule();
        }
        JobHandle.CompleteAll(jobHandleArray);
        jobHandleArray.Dispose();

        Debug.Log("Time: " + ((Time.realtimeSinceStartup - startTime) * 1000f) + " ms.");
    }

    private int2 WorldPositionToGrid(int2 worldPos, int2 minPositionOnMap, int2 maxPositionOnMap, float gridWidth)
    {
        int2 worldPosition = math.clamp(worldPos, minPositionOnMap, maxPositionOnMap);
        int2 gridPos = worldPosition - minPositionOnMap;
        gridPos.x = Mathf.RoundToInt(gridPos.x / gridWidth);
        gridPos.y = Mathf.RoundToInt(gridPos.y / gridWidth);
        return gridPos;
    }

    private void Start()
    {
        CreateNodes();
        FindPathToThePoint();
    }
    
    //TESTING

    private const int STRAIGHT_MOVE_COST = 10;
    private const int DIAGONAL_MOVE_COST = 14;
    private const float RAYCAST_LENGTH = 50f;
    private const string FLOOR_TAG = "Floor";

    [SerializeField] [Range(0.1f, 2f)] private float gridWidth = 1f;
    [SerializeField] private int2 minPositionOnMap = new int2();
    [SerializeField] private int2 maxPositionOnMap = new int2();

    private int2 gridSize;

    private NativeArray<PathNode> pathNodeArray;      //<------------------------------------------------------------------------------
    //private PathNode[] pathNodeArray;

    public void CreateNodes()
    {
        //pathNodeArray.Dispose();
        CreatePathNodeArray();
    }

    private void CreatePathNodeArray() //<------------------------------------------------------------------------------
    {
        gridSize.x = Mathf.RoundToInt((maxPositionOnMap.x - minPositionOnMap.x) / gridWidth);
        gridSize.y = Mathf.RoundToInt((maxPositionOnMap.y - minPositionOnMap.y) / gridWidth);

        pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);
        //pathNodeArray = new PathNode[gridSize.x * gridSize.y];

        for(int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                PathNode pathNode = new PathNode(x, y, gridSize.x, IsNodeWalkable(x, y, gridWidth));
                pathNodeArray[pathNode.index] = pathNode;
            }
        }
    }

    private bool IsNodeWalkable(int x, int y, float gridSize)
    {
        Vector3 rayPosition = (new Vector3(x, 0f, y) * gridSize) + new Vector3(minPositionOnMap.x, RAYCAST_LENGTH / 2f, minPositionOnMap.y);
        RaycastHit hit;
        if (Physics.Raycast(rayPosition, Vector3.down, out hit, RAYCAST_LENGTH))
        {
            if (hit.collider.tag == FLOOR_TAG)
            {
                return true;
            }
        }
        return false;
    }

    private static int CalculateDistanceCost(int2 from, int2 to)
    {
        int xDistance = math.abs(from.x - to.x);
        int yDistance = math.abs(from.y - to.y);
        int difference = math.abs(xDistance - yDistance);
        int distance = DIAGONAL_MOVE_COST * math.min(xDistance, yDistance) + STRAIGHT_MOVE_COST * difference;
        return distance;
    }

    [BurstCompile]
    private struct FindPathJob : IJob
    {
        //public PathNode[] pathNodeArray; //<-------------------------------------------------------
        public NativeArray<PathNode> pathNodeArray;
        public int2 gridSize;
        public float gridWidth;
        public int2 startPosition;
        public int2 endPosition;
        public int2 minPositionOnMap;
        public int2 maxPositionOnMap;

        public void Execute()
        {
            NativeArray<PathNode> pathNodes = new NativeArray<PathNode>(pathNodeArray, Allocator.Temp);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    int nodeIndex = PathNode.CalculateIndexOfNode(x, y, gridSize.x);
                    PathNode node = pathNodes[nodeIndex];
                    node.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                }
            }

            int startNodeIndex = PathNode.CalculateIndexOfNode(startPosition, gridSize.x);
            int endNodeIndex = PathNode.CalculateIndexOfNode(endPosition, gridSize.x);

            PathNode startNode = pathNodeArray[startNodeIndex];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodes[startNode.index] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);
            NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
            neighbourOffsetArray[0] = new int2(0, 1);
            neighbourOffsetArray[1] = new int2(1, 1);
            neighbourOffsetArray[2] = new int2(1, 0);
            neighbourOffsetArray[3] = new int2(1, -1);
            neighbourOffsetArray[4] = new int2(0, -1);
            neighbourOffsetArray[5] = new int2(-1, -1);
            neighbourOffsetArray[6] = new int2(-1, 0);
            neighbourOffsetArray[7] = new int2(-1, 1);

            openList.Add(startNode.index);

            while (openList.Length > 0)
            {
                int thisNodeIndex = GetLowestFCostNodeIndex(openList, pathNodes);
                if (thisNodeIndex == endNodeIndex)
                {
                    break; // End node reached!
                }

                PathNode thisNode = pathNodes[thisNodeIndex];

                // Remove this node from open list.
                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == thisNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(thisNodeIndex);

                for (int i = 0; i < neighbourOffsetArray.Length; i++)
                {
                    int2 offset = neighbourOffsetArray[i];
                    int2 neighbourPosition = offset + thisNode.XY;

                    if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                    {
                        continue;
                    }

                    int neighbourNodeIndex = PathNode.CalculateIndexOfNode(neighbourPosition, gridSize.x);
                    if (closedList.Contains(neighbourNodeIndex))
                    {
                        continue;
                    }

                    PathNode neighbourNode = pathNodes[neighbourNodeIndex];
                    if (!neighbourNode.isWalkable)
                    {
                        continue;
                    }

                    int2 thisNodePosition = thisNode.XY;

                    int tentativeGCost = thisNode.gCost + CalculateDistanceCost(thisNodePosition, neighbourPosition);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNodeIndex = thisNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.CalculateFCost();
                        pathNodes[neighbourNodeIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNodeIndex))
                        {
                            openList.Add(neighbourNodeIndex);
                        }
                    }
                }
            }

            PathNode endNode = pathNodes[endNodeIndex];

            if (endNode.cameFromNodeIndex == -1)
            {
                //No path!
            }
            else
            {
                NativeList<int2> path = CalculatePath(pathNodes, endNode);
                Debug.Log("Path!");
                path.Dispose();

            }

            openList.Dispose();
            closedList.Dispose();
            neighbourOffsetArray.Dispose();
            pathNodes.Dispose();
        }
    }

    private void FindPath(int2 startPosition, int2 endPosition)
    {
        NativeArray<PathNode> pathNodes = new NativeArray<PathNode>(pathNodeArray, Allocator.Temp);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                int nodeIndex = PathNode.CalculateIndexOfNode(x, y, gridSize.x);
                PathNode node = pathNodes[nodeIndex];
                node.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
            }
        }

        int startNodeIndex = PathNode.CalculateIndexOfNode(startPosition, gridSize.x);
        int endNodeIndex = PathNode.CalculateIndexOfNode(endPosition, gridSize.x);

        PathNode startNode = pathNodeArray[startNodeIndex];
        startNode.gCost = 0;
        startNode.CalculateFCost();
        pathNodes[startNode.index] = startNode;

        NativeList<int> openList = new NativeList<int>(Allocator.Temp);
        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);
        NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
        neighbourOffsetArray[0] = new int2(0, 1);
        neighbourOffsetArray[1] = new int2(1, 1);
        neighbourOffsetArray[2] = new int2(1, 0);
        neighbourOffsetArray[3] = new int2(1, -1);
        neighbourOffsetArray[4] = new int2(0, -1);
        neighbourOffsetArray[5] = new int2(-1, -1);
        neighbourOffsetArray[6] = new int2(-1, 0);
        neighbourOffsetArray[7] = new int2(-1, 1);

        openList.Add(startNode.index);

        while (openList.Length > 0)
        {
            int thisNodeIndex = GetLowestFCostNodeIndex(openList, pathNodes);
            if (thisNodeIndex == endNodeIndex)
            {
                break; // End node reached!
            }

            PathNode thisNode = pathNodes[thisNodeIndex];

            // Remove this node from open list.
            for(int i = 0; i < openList.Length; i++)
            {
                if (openList[i] == thisNodeIndex)
                {
                    openList.RemoveAtSwapBack(i);
                    break;
                }
            }

            closedList.Add(thisNodeIndex);

            for(int i = 0; i < neighbourOffsetArray.Length; i++)
            {
                int2 offset = neighbourOffsetArray[i];
                int2 neighbourPosition = offset + thisNode.XY;

                if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                {
                    continue;
                }

                int neighbourNodeIndex = PathNode.CalculateIndexOfNode(neighbourPosition, gridSize.x);
                if (closedList.Contains(neighbourNodeIndex))
                {
                    continue;
                }

                PathNode neighbourNode = pathNodes[neighbourNodeIndex];
                if (!neighbourNode.isWalkable)
                {
                    continue;
                }

                int2 thisNodePosition = thisNode.XY;

                int tentativeGCost = thisNode.gCost + CalculateDistanceCost(thisNodePosition, neighbourPosition);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNodeIndex = thisNodeIndex;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.CalculateFCost();
                    pathNodes[neighbourNodeIndex] = neighbourNode;
                    
                    if (!openList.Contains(neighbourNodeIndex))
                    {
                        openList.Add(neighbourNodeIndex);
                    }
                }
            }
        }

        PathNode endNode = pathNodes[endNodeIndex];

        if (endNode.cameFromNodeIndex == -1)
        {
            //No path!
        }
        else
        {
            NativeList<int2> path = CalculatePath(pathNodes, endNode);
            DrawPath(path, gridWidth, minPositionOnMap, maxPositionOnMap);
            path.Dispose();

        }

        openList.Dispose();
        closedList.Dispose();
        neighbourOffsetArray.Dispose();
        pathNodes.Dispose();
    }

    private static void DrawPath(NativeList<int2> path, float gridWidth, int2 minPositionOnMap, int2 maxPositionOnMap)
    {
        for(int i = 1; i < path.Length; i++)
        {
            int2 p1 = path[i - 1];
            int2 p2 = path[i];
            Vector3 minPos = new Vector3(minPositionOnMap.x, 0f, minPositionOnMap.y);
            Vector3 start = (new Vector3(p1.x, 0f, p1.y) * gridWidth) + minPos;
            Vector3 end = (new Vector3(p2.x, 0f, p2.y) * gridWidth) + minPos;
            Debug.DrawLine(start, end, Color.red, 30f);
        }
    }

    private static NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodes, PathNode endNode)
    {
        if (endNode.cameFromNodeIndex == -1)
        {
            return new NativeList<int2>(Allocator.Temp);
        }
        else
        {
            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
            path.Add(endNode.XY);

            PathNode thisNode = endNode;
            while (thisNode.cameFromNodeIndex != -1)
            {
                PathNode cameFromNode = pathNodes[thisNode.cameFromNodeIndex];
                path.Add(cameFromNode.XY);
                thisNode = cameFromNode;
            }
            return path;
        }
    }

    private static bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
    {
        return gridPosition.x >= 0 &&
               gridPosition.y >= 0 &&
               gridPosition.x < gridSize.x &&
               gridPosition.y < gridSize.y;
    }

    private static int GetLowestFCostNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodes)
    {
        PathNode lowestFCostPathNode = pathNodes[openList[0]];
        for (int i = 1; i < openList.Length; i++)
        {
            PathNode thisNode = pathNodes[openList[i]];
            if (thisNode.fCost < lowestFCostPathNode.fCost)
            {
                lowestFCostPathNode = thisNode;
            }
        }
        return lowestFCostPathNode.index;
    }
}





/*

public class Pathfinding : MonoBehaviour
{
    private const int STRAIGHT_MOVE_COST = 10;
    private const int DIAGONAL_MOVE_COST = 14;
    private const float RAYCAST_HIGHT_OFFSET = 10f;
    private const string FLOOR_TAG = "Floor";
    private const string WALL_TAG = "Wall";

    private List<PathNode> openList;
    private List<PathNode> closedList;

    private List<PathNode> FindPath(Vector3 startPosition, Vector3 endPosition, float gridStep)
    {
        PathNode startNode = new PathNode(startPosition);
        PathNode endNode = new PathNode(endPosition);

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        Directions.SetGridStep(gridStep);
        
        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                Debug.Log("END!");
                return CalculatePath(startNode, currentNode);
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            List<PathNode> neighbourList = GetNeighbourList(currentNode);

            foreach (PathNode neighbourNode in neighbourList)
            {
                if (closedList.Contains(neighbourNode)) continue;

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.previousNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        for (int i = 0; i < Directions.nDirections; i++)
        {
            PathNode neighbourNode = GetNeighbourNode(currentNode, Directions.directions[i], RAYCAST_HIGHT_OFFSET);
            if (neighbourNode != null)
            {
                neighbourList.Add(neighbourNode);
            }
        }

        return neighbourList;
    }

    private PathNode GetNeighbourNode(PathNode currentNode, Vector2 direction, float rayOffsetY)
    {
        Vector3 rayPosition = currentNode.position + new Vector3(direction.x, rayOffsetY / 2f, direction.y);
        RaycastHit hit;
        if (Physics.Raycast(rayPosition, Vector3.down, out hit, rayOffsetY))
        {
            if (hit.collider.tag == FLOOR_TAG)
            {
                return new PathNode(hit.point);
            }
        }
        return null;
    }

    private List<PathNode> CalculatePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.previousNode != null)
        {
            path.Add(currentNode.previousNode);
            currentNode = currentNode.previousNode;
        }
        path.Reverse();
        return path;
    }
    
    private int CalculateDistanceCost(PathNode p1, PathNode p2)
    {
        //float xDifference = Mathf.Abs(p1.position.x - p2.position.x);
        //float yDifference = Mathf.Abs(p1.position.y - p2.position.y);
        //float remaining = Mathf.Abs(xDifference - yDifference);
        //float distance = DIAGONAL_MOVE_COST * Mathf.Min(xDifference, yDifference) + STRAIGHT_MOVE_COST * remaining;
        //return Mathf.RoundToInt(distance);
        
        return Mathf.RoundToInt(100 * Vector3.Distance(p1.position, p2.position));
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for(int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                lowestFCostNode = pathNodeList[i];
        }
        return lowestFCostNode;
    }

    [SerializeField] bool followPlayer = false;
    Vector3Int lastPos = Vector3Int.zero;
    [SerializeField] Transform player = null;
    [SerializeField] Transform startPoint = null;
    [SerializeField] Transform endPoint = null;
    [SerializeField] float gridStep = 1f;
    [SerializeField] List<PathNode> path;

    public void ShowWalkableNodes()
    {
        List<PathNode> walkableNodes = new List<PathNode>();

        PathNode currentNode = new PathNode(startPoint.position);

        walkableNodes.Add(currentNode);

        Directions.SetGridStep(gridStep);

        List<PathNode> openList = GetNeighbourList(currentNode);
        
        while (openList.Count > 0)
        {
            PathNode node = openList[0];

            if (!walkableNodes.Contains(node)) walkableNodes.Add(node);

            openList.Remove(node);

            List<PathNode> neighbours = GetNeighbourList(node);

            foreach (PathNode pathNode in neighbours)
            {
                if(!walkableNodes.Contains(pathNode) && !openList.Contains(pathNode))
                {
                    openList.Add(pathNode);
                }
            }
        }

        foreach(PathNode node in walkableNodes)
        {
            Debug.DrawLine(node.position, node.position + Vector3.up, Color.blue, 5f);
        }
    }

    public void FindPath()
    {
        path = FindPath(startPoint.position, endPoint.position, gridStep);
        if (path != null)
        {
            Debug.Log("Path!");
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i].position + Vector3.up, path[i + 1].position + Vector3.up, Color.yellow, 10f);
            }
        }
        else
        {
            Debug.Log("No Path :( ");
        }
    }

    private void Update()
    {
        if (followPlayer)
        {
            Vector3Int thisPos = new Vector3Int(Mathf.RoundToInt(player.position.x), 0, Mathf.RoundToInt(player.position.z));
            if (lastPos != thisPos)
            {
                lastPos = thisPos;

                path = FindPath(player.position, endPoint.position, gridStep);
            }
            
            if (path != null)
            {
                Debug.Log("Path!");
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(path[i].position + Vector3.up, path[i + 1].position + Vector3.up, Color.yellow);
                }
            }
            else
            {
                Debug.Log("No Path :( ");
            }
        }
    }
}

*/