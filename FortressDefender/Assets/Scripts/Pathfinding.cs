using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        /*
        float xDifference = Mathf.Abs(p1.position.x - p2.position.x);
        float yDifference = Mathf.Abs(p1.position.y - p2.position.y);
        float remaining = Mathf.Abs(xDifference - yDifference);
        float distance = DIAGONAL_MOVE_COST * Mathf.Min(xDifference, yDifference) + STRAIGHT_MOVE_COST * remaining;
        return Mathf.RoundToInt(distance);
        */
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

