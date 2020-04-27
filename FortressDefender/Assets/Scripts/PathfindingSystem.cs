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
        NativeList<JobHandle> jobHandles = new NativeList<JobHandle>(Allocator.Temp);

        Entities.ForEach((Entity entity, DynamicBuffer<PathPositionsBuffer> pathPositionsBuffer, ref PathfindingParameters parameters) =>
        {
            if (WaypointsManager.Instance != null)
            {
                FindPathJob findPathJob = new FindPathJob
                {
                    startPosition = parameters.startGridPoint,
                    endPosition = parameters.endGridPoint,
                    mapBoundries = WaypointsManager.MapBoundries,
                    gridSize = WaypointsManager.GridSize,
                    waypoints = new NativeArray<Waypoint>(WaypointsManager.Instance.waypoints, Allocator.TempJob),
                    pathPositionsBuffer = pathPositionsBuffer,
                    entity = entity,
                    unitDataComponentFromEntity = GetComponentDataFromEntity<UnitData>()

                };
                jobHandles.Add(findPathJob.Schedule());
                
                PostUpdateCommands.RemoveComponent<PathfindingParameters>(entity);
            }
        });

        JobHandle.CompleteAll(jobHandles);
    }

    [BurstCompile]
    private struct FindPathJob : IJob
    {
        public int2 startPosition;
        public int2 endPosition;
        public MapBoundries mapBoundries;
        public int2 gridSize;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Waypoint> waypoints;
        public DynamicBuffer<PathPositionsBuffer> pathPositionsBuffer;
        public Entity entity;
        public ComponentDataFromEntity<UnitData> unitDataComponentFromEntity;
        
        public void Execute()
        {
            NativeArray<PathNode> pathNodes = GetPathNodesFromWaypoints(waypoints);

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

            PathNode startNode = pathNodes[startNodeIndex];
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

            pathPositionsBuffer.Clear();
            if (endNode.cameFromNodeIndex == -1)
            {
                Debug.Log("There is a entity with blocked path!");
                UnitData data = unitDataComponentFromEntity[entity];
                data.pathIndex = -1;
                unitDataComponentFromEntity[entity] = data;
            }
            else
            {
                CalculatePath(pathNodes, endNode, pathPositionsBuffer);
                UnitData data = unitDataComponentFromEntity[entity];
                data.pathIndex = pathPositionsBuffer.Length - 1;
                unitDataComponentFromEntity[entity] = data;
            }

            openList.Dispose();
            closedList.Dispose();
            neighbourOffsetArray.Dispose();
            pathNodes.Dispose();
        }
        
        private static NativeArray<PathNode> GetPathNodesFromWaypoints(NativeArray<Waypoint> waypoints)
        {
            int numWaypoints = waypoints.Length;
            NativeArray<PathNode> pathNodes = new NativeArray<PathNode>(numWaypoints, Allocator.Temp);
            for(int i = 0; i < numWaypoints; i++)
            {
                pathNodes[i] = new PathNode(waypoints[i]);
            }
            return pathNodes;
        }
        
        private static int CalculateDistanceCost(int2 from, int2 to)
        {
            int xDistance = math.abs(from.x - to.x);
            int yDistance = math.abs(from.y - to.y);
            int difference = math.abs(xDistance - yDistance);
            int distance = DIAGONAL_MOVE_COST * math.min(xDistance, yDistance) + STRAIGHT_MOVE_COST * difference;
            return distance;
        }

        private static void CalculatePath(NativeArray<PathNode> pathNodes, PathNode endNode, DynamicBuffer<PathPositionsBuffer> pathPositionsBuffer)
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
}
