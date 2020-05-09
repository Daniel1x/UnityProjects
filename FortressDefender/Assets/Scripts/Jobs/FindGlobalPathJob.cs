using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public struct FindGlobalPathJob : IJob
{
    /// <summary>
    /// Heuristic cost of move in straight directions, used in pathfinding calculations.
    /// </summary>
    private const int STRAIGHT_MOVE_COST = 10;
    /// <summary>
    /// Heuristic cost of move in diagonal directions, used in pathfinding calculations.
    /// </summary>
    private const int DIAGONAL_MOVE_COST = 14;

    /// <summary>
    /// Start position in grid coordinates.
    /// </summary>
    public int2 startGridPosition;
    /// <summary>
    /// Target position in grid coordinates.
    /// </summary>
    public int2 endGridPosition;
    /// <summary>
    /// Size of waypoints grid.
    /// </summary>
    public int2 gridSize;
    /// <summary>
    /// Native array of waypoints. [ReadOnly]
    /// </summary>
    [DeallocateOnJobCompletion] public NativeArray<PathNode> pathNodes;
    /// <summary>
    /// Dynamic path position buffer, stores information about the world positions of path nodes.
    /// </summary>
    public NativeArray<PathPositionsBuffer> pathPositionsBuffer;

    public NativeArray<int> pathSize;
 
    public void Execute()
    {
        // Find indexes of start and end nodes.
        int startNodeIndex = CalculateIndexOfNode(startGridPosition, gridSize.x);
        int endNodeIndex = CalculateIndexOfNode(endGridPosition, gridSize.x);

        // Set start node values.
        PathNode startNode = pathNodes[startNodeIndex];
        startNode.gCost = 0;
        startNode.CalculateHCost(endGridPosition);
        startNode.CalculateFCost();
        pathNodes[startNode.index] = startNode;

        // Create the lists needed to calculate the path search using the A * algorithm.
        NativeList<int> openList = new NativeList<int>(Allocator.Temp);
        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

        // Create simple native array of offsets to neighbour nodes.
        NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
        neighbourOffsetArray[0] = new int2(0, 1);
        neighbourOffsetArray[1] = new int2(1, 1);
        neighbourOffsetArray[2] = new int2(1, 0);
        neighbourOffsetArray[3] = new int2(1, -1);
        neighbourOffsetArray[4] = new int2(0, -1);
        neighbourOffsetArray[5] = new int2(-1, -1);
        neighbourOffsetArray[6] = new int2(-1, 0);
        neighbourOffsetArray[7] = new int2(-1, 1);

        // Add start node to open list.
        openList.Add(startNode.index);

        // Start searching until there are no nodes in the open list.
        while (openList.Length > 0)
        {
            int thisNodeIndex = GetLowestFCostNodeIndex(openList, pathNodes);
            if (thisNodeIndex == endNodeIndex)
            {
                break; // End node reached!
            }

            // Get lowest fCost node from pathnodes array.
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

            // Add this node to the closed list as it has already been searched.
            closedList.Add(thisNodeIndex);

            // For every neighbour node
            for (int i = 0; i < neighbourOffsetArray.Length; i++)
            {
                // Get neighbour node position.
                int2 offset = neighbourOffsetArray[i];
                int2 neighbourPosition = offset + thisNode.XY;

                // If the neighbour node is not in the grid, skip it and continue.
                if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                {
                    continue;
                }

                // If the index of a neighbour node is in a closed list, so the node has been searched, skip it and continue.
                int neighbourNodeIndex = CalculateIndexOfNode(neighbourPosition, gridSize.x);
                if (closedList.Contains(neighbourNodeIndex))
                {
                    continue;
                }

                // If neighbour node is not walkable, skip it and continue.
                PathNode neighbourNode = pathNodes[neighbourNodeIndex];
                if (!neighbourNode.isWalkable)
                {
                    continue;
                }

                // Calculate the tentative GCost for the neighbour node, 
                // and if it is lower than the actual values ​​in the neighbour node, 
                // change its values ​​and add it to the open list if it does not already exist there.
                int2 thisNodePosition = thisNode.XY;
                int tentativeGCost = thisNode.gCost + CalculateDistanceCost(thisNodePosition, neighbourPosition);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNodeIndex = thisNodeIndex;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.CalculateHCost(endGridPosition);
                    neighbourNode.CalculateFCost();
                    pathNodes[neighbourNodeIndex] = neighbourNode;

                    if (!openList.Contains(neighbourNodeIndex))
                    {
                        openList.Add(neighbourNodeIndex);
                    }
                }
            }
        }

        // After searching through all the nodes in open list, get end node.
        PathNode endNode = pathNodes[endNodeIndex];

        // Clear dynamic path buffer.
        // pathPositionsBuffer.Clear();

        // If end node has no "previous" node, there is no path to it.
        if (endNode.cameFromNodeIndex == -1)
        {
            Debug.Log("There is a blocked global path!");
        }
        else // Otherwise, there is a path, so calculate it and fill in pathPositionsBuffer.
        {
            CalculatePath(pathNodes, endNode, ref pathPositionsBuffer, pathSize);
        }

        // Dispose all native lists and arrays.
        openList.Dispose();
        closedList.Dispose();
        neighbourOffsetArray.Dispose();
    }
    
    /// <summary>
    /// Function that calculates heuristic distance cost between two points on grid.
    /// </summary>
    /// <param name="from">First position on grid.</param>
    /// <param name="to">Second position on grid.</param>
    /// <returns></returns>
    private static int CalculateDistanceCost(int2 from, int2 to)
    {
        int xDistance = math.abs(from.x - to.x);
        int yDistance = math.abs(from.y - to.y);
        int difference = math.abs(xDistance - yDistance);
        int distance = DIAGONAL_MOVE_COST * math.min(xDistance, yDistance) + STRAIGHT_MOVE_COST * difference;
        return distance;
    }

    /// <summary>
    /// Static function that calculates the node index on a grid.
    /// </summary>
    /// <param name="gridPosition">Node position on grid.</param>
    /// <param name="gridWidth">Width of grid.</param>
    /// <returns></returns>
    public static int CalculateIndexOfNode(int2 gridPosition, int gridWidth)
    {
        return gridPosition.x + gridPosition.y * gridWidth;
    }

    /// <summary>
    /// Function that fills the dynamic buffer with the global position of nodes on the path.
    /// </summary>
    /// <param name="pathNodes">Native array of pathnodes.</param>
    /// <param name="endNode">Target node.</param>
    /// <param name="pathPositionsBuffer">Reference to dynamic buffer of path positions.</param>
    private static void CalculatePath(NativeArray<PathNode> pathNodes, PathNode endNode, ref NativeArray<PathPositionsBuffer> pathPositionsBuffer, NativeArray<int> pathSize)
    {
        if (endNode.cameFromNodeIndex == -1)
        {
            //There is no path!
        }
        else
        {
            PathNode node = endNode;
            pathPositionsBuffer[0] = new PathPositionsBuffer { worldPosition = node.worldPosition };
            int index = 1;
            while (node.cameFromNodeIndex != -1)
            {
                node = pathNodes[node.cameFromNodeIndex];
                pathPositionsBuffer[index] = new PathPositionsBuffer { worldPosition = node.worldPosition };
                index++;
            }
            pathSize[0] = index;
        }
    }

    /// <summary>
    /// Function to check if the position is in the grid.
    /// </summary>
    /// <param name="gridPosition">Position in grid coordinates.</param>
    /// <param name="gridSize">Size of grid.</param>
    /// <returns></returns>
    private static bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
    {
        return gridPosition.x >= 0 &&
               gridPosition.y >= 0 &&
               gridPosition.x < gridSize.x &&
               gridPosition.y < gridSize.y;
    }

    /// <summary>
    /// Function that returns index of pathnode from pathnodes list with the lowest fCost value.
    /// </summary>
    /// <param name="openList">List of indexes for pathnodes in the open list, available for search.</param>
    /// <param name="pathNodes">Native array of all the pathnodes.</param>
    /// <returns></returns>
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
