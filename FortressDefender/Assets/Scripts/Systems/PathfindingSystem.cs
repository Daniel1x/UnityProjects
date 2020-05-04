using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Transforms;
using System;

public class PathfindingSystem : ComponentSystem
{
    private const int STRAIGHT_MOVE_COST = 10;
    private const int DIAGONAL_MOVE_COST = 14;
    private const string FLOOR_TAG = "Floor";

    protected override void OnUpdate()
    {
        // List of jobs.
        NativeList<JobHandle> jobHandles = new NativeList<JobHandle>(Allocator.Temp);

        Entities.ForEach((Entity entity, DynamicBuffer<PathPositionsBuffer> pathPositionsBuffer, ref PathfindingParameters parameters) =>
        {
            if (WaypointsManager.Instance != null) // If there is WaypointManager.Instance
            {
                if (parameters.needNewPath) // If new path needs to be created
                {
                    // Create pathfinding job for current entity.
                    FindPathJob findPathJob = new FindPathJob
                    {
                        startPosition = parameters.startGridPoint,
                        endPosition = parameters.endGridPoint,
                        gridSize = WaypointsManager.GridSize,
                        waypoints = new NativeArray<Waypoint>(WaypointsManager.Instance.waypoints, Allocator.TempJob),
                        pathPositionsBuffer = pathPositionsBuffer,
                        entity = entity,
                        unitDataComponentFromEntity = GetComponentDataFromEntity<UnitData>()

                    };
                    // Schedule current pathfinding job.
                    jobHandles.Add(findPathJob.Schedule());
                    // Mark that there is no need to create new path.
                    parameters.needNewPath = false;
                }                
            }
        });

        // Complete all pathfinding jobs.
        JobHandle.CompleteAll(jobHandles);
    }

    [BurstCompile]
    private struct FindPathJob : IJob
    {
        /// <summary>
        /// Start position in grid coordinates.
        /// </summary>
        public int2 startPosition;
        /// <summary>
        /// Target position in grid coordinates.
        /// </summary>
        public int2 endPosition;
        /// <summary>
        /// Size of waypoints grid.
        /// </summary>
        public int2 gridSize;
        /// <summary>
        /// Native array of waypoints. [ReadOnly]
        /// </summary>
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Waypoint> waypoints;
        /// <summary>
        /// Dynamic path position buffer, stores information about the world positions of path nodes.
        /// </summary>
        public DynamicBuffer<PathPositionsBuffer> pathPositionsBuffer;
        /// <summary>
        /// Current entity.
        /// </summary>
        public Entity entity;
        /// <summary>
        /// Unit data components from entities.
        /// </summary>
        public ComponentDataFromEntity<UnitData> unitDataComponentFromEntity;
        
        public void Execute()
        {
            // Creating pathnodes native array.
            NativeArray<PathNode> pathNodes = GetPathNodesFromWaypoints(waypoints);

            // For every pathnode on grid, calculate heuristic distance cost.
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    int nodeIndex = PathNode.CalculateIndexOfNode(x, y, gridSize.x);
                    PathNode node = pathNodes[nodeIndex];
                    node.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                }
            }

            // Find indexes of start and end nodes.
            int startNodeIndex = PathNode.CalculateIndexOfNode(startPosition, gridSize.x);
            int endNodeIndex = PathNode.CalculateIndexOfNode(endPosition, gridSize.x);

            // Set start node values.
            PathNode startNode = pathNodes[startNodeIndex];
            startNode.gCost = 0;
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
                    int neighbourNodeIndex = PathNode.CalculateIndexOfNode(neighbourPosition, gridSize.x);
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
            pathPositionsBuffer.Clear();

            // If end node has no "previous" node, there is no path to it.
            UnitData data = unitDataComponentFromEntity[entity];
            if (endNode.cameFromNodeIndex == -1)
            {
                Debug.Log("There is a entity with blocked path!");
                data.pathIndex = -1;
            }
            else // Otherwise, there is a path, so calculate it and fill in pathPositionsBuffer.
            {
                CalculatePath(pathNodes, endNode, ref pathPositionsBuffer);
                data.pathIndex = pathPositionsBuffer.Length - 1;
            }
            // Update the entity unit data to follow the path.
            unitDataComponentFromEntity[entity] = data;

            // Dispose all native lists and arrays.
            openList.Dispose();
            closedList.Dispose();
            neighbourOffsetArray.Dispose();
            pathNodes.Dispose();
        }
        
        /// <summary>
        /// Creates native array of path nodes from native array of waypoints.
        /// </summary>
        /// <param name="waypoints">Native list of waypoints.</param>
        /// <returns></returns>
        private static NativeArray<PathNode> GetPathNodesFromWaypoints([ReadOnly] NativeArray<Waypoint> waypoints)
        {
            int numWaypoints = waypoints.Length;
            NativeArray<PathNode> pathNodes = new NativeArray<PathNode>(numWaypoints, Allocator.Temp);
            for(int i = 0; i < numWaypoints; i++)
            {
                pathNodes[i] = new PathNode(waypoints[i]);
            }
            return pathNodes;
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
        /// Function that fills the dynamic buffer with the global position of nodes on the path.
        /// </summary>
        /// <param name="pathNodes">Native array of pathnodes.</param>
        /// <param name="endNode">Target node.</param>
        /// <param name="pathPositionsBuffer">Reference to dynamic buffer of path positions.</param>
        private static void CalculatePath(NativeArray<PathNode> pathNodes, PathNode endNode, ref DynamicBuffer<PathPositionsBuffer> pathPositionsBuffer)
        {
            if (endNode.cameFromNodeIndex == -1)
            {
                //There is no path!
            }
            else
            {
                pathPositionsBuffer.Add(new PathPositionsBuffer { worldPosition = endNode.worldPosition });
                PathNode thisNode = endNode;

                while (thisNode.cameFromNodeIndex != -1)
                {
                    PathNode cameFromNode = pathNodes[thisNode.cameFromNodeIndex];
                    pathPositionsBuffer.Add(new PathPositionsBuffer { worldPosition = cameFromNode.worldPosition });
                    thisNode = cameFromNode;
                }
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
}
