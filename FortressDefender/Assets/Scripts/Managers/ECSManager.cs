using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;

public class ECSManager : MonoBehaviour
{
    /// <summary>
    /// Prefab of unit gameobject.
    /// </summary>
    [SerializeField] private GameObject unitPrefab = null;
    /// <summary>
    /// Prefab of tower gameobject.
    /// </summary>
    [SerializeField] private GameObject towerPrefab = null;
    /// <summary>
    /// Text field to display entity count.
    /// </summary>
    [SerializeField] private Text textBox = null;
    /// <summary>
    /// Number of entities spawned at once.
    /// </summary>
    [SerializeField] private int entitiesToSpawnAtOnce = 250;
    /// <summary>
    /// Number of entities spawned per every second.
    /// </summary>
    [Range(0, 100)] [SerializeField] private float entitiesToSpawnPerSecond = 1f;
    /// <summary>
    /// Desired frame rate.
    /// </summary>
    [Range(0f, 100f)] [SerializeField] private float targetFrameRate = 30;
    /// <summary>
    /// LayerMask of floor gameobjects.
    /// </summary>
    [SerializeField] private LayerMask floorLayerMask = 0;
    
    /// <summary>
    /// Static entity manager reference.
    /// </summary>
    public static EntityManager entityManager;
    /// <summary>
    /// Unit prefab as an entity.
    /// </summary>
    public static Entity unitEntity;
    /// <summary>
    /// Tower prefab as an entity.
    /// </summary>
    public static Entity towerEntity;
    /// <summary>
    /// BlobAssetStore reference.
    /// </summary>
    private BlobAssetStore store;
    /// <summary>
    /// Constant text displayed with entities count.
    /// </summary>
    private const string ENTITIES_TEXT = "Entities: ";
    /// <summary>
    /// Entity query placeholder.
    /// </summary>
    private EntityQuery entityQuery;

    /// <summary>
    /// List of tower entities.
    /// </summary>
    private List<Entity> towers = new List<Entity>();
    
    private void Start()
    {
        store = new BlobAssetStore();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, store);
        unitEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(unitPrefab, settings);
        towerEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(towerPrefab, settings);
    }

    /// <summary>
    /// Instantiates unit entities.
    /// </summary>
    /// <param name="entitiesToSpawn">Number of entities to spawn</param>
    private void SpawnUnitEntities(int entitiesToSpawn)
    {
        float3 spawnPoint = WaypointsManager.Instance.spawnPoint.position;
        for (int i = 0; i < entitiesToSpawn; i++)
        {
            float3 position = spawnPoint + new float3(UnityEngine.Random.Range(-3f, 3f), 0f, UnityEngine.Random.Range(-3f, 3f));
            int2 roundedToGridPosition = WaypointsManager.Func.RoundToGrid(position);
            position = new float3(roundedToGridPosition.x, 0f, roundedToGridPosition.y);

            // If waypoint at spawn position is walkable, instantiate entity.
            if (WaypointsManager.waypoints[WaypointsManager.Func.GetIndexFromWorldPosition(roundedToGridPosition)].isWalkable)
            {
                Entity entity = entityManager.Instantiate(unitEntity);
                entityManager.SetComponentData(entity, new Translation { Value = position });
            }
        }
    }

    /// <summary>
    /// Instantiates tower entity.
    /// </summary>
    /// <param name="positionInWorldCoordinates">Position of tower in world coordinates.</param>
    private void SpawnTowerEntityAtPosition(float3 positionInWorldCoordinates)
    {
        Entity entity = entityManager.Instantiate(towerEntity);
        float3 position = WaypointsManager.Func.RoundToGridInWorld(positionInWorldCoordinates);
        entityManager.SetComponentData(entity, new Translation { Value = position });
        entityManager.SetComponentData(entity, new TowerPositionOnGridData { positionOnGrid = WaypointsManager.Func.GetGridPositionFromWorldPosition(position, WaypointsManager.MapBoundaries) });

        towers.Add(entity);
    }

    /// <summary>
    /// Destroys tower entity at position.
    /// </summary>
    /// <param name="towerPosition">Tower position in world coordinates.</param>
    private void DestroyTowerAtPosition(float3 towerPosition)
    {
        int2 towerPositionOnGrid = WaypointsManager.Func.GetGridPositionFromWorldPosition(WaypointsManager.Func.RoundToGrid(towerPosition));
        foreach (Entity tower in towers)
        {
            int2 currentTowerPosition = entityManager.GetComponentData<TowerPositionOnGridData>(tower).positionOnGrid;
            if (towerPositionOnGrid.x == currentTowerPosition.x && towerPositionOnGrid.y == currentTowerPosition.y)
            {
                towers.Remove(tower);
                entityManager.DestroyEntity(tower);
                return;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnUnitEntities(entitiesToSpawnAtOnce);
        }

        UpdateTextBox();
        ControllFrameRate();
        
        // TESTING BLOCKING CHOSEN WAYPOINTS AND SPAWNING TURRETS
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, floorLayerMask))
            {
                int2 hitOnGridPosition = WaypointsManager.Func.GetGridPositionFromWorldPosition(WaypointsManager.Func.RoundToGrid(hit.point),WaypointsManager.MapBoundaries);
                int hitOnGridIndex = WaypointsManager.Func.CalculateIndex(hitOnGridPosition, WaypointsManager.GridSize.x);
                if (!WaypointsManager.waypoints[hitOnGridIndex].isBlockedByWall)
                {
                    bool waypointState = !WaypointsManager.waypoints[hitOnGridIndex].isWalkable;
                    WaypointsManager.waypoints[hitOnGridIndex].isWalkable = waypointState;
                    WaypointsManager.waypointHasBeenModified = true;
                    string state = waypointState ? " Unlocked" : " Blocked";

                    WaypointsManager.lastBlockedWaypoints.Add(WaypointsManager.Func.RoundToGridInWorld(hit.point));

                    Debug.Log("Clicked at: " + hit.point + ", Hitted grid at: " + hitOnGridPosition.ToString() + " with index: " + hitOnGridIndex.ToString() + state);

                    if (state.Equals(" Blocked"))
                    {
                        SpawnTowerEntityAtPosition(hit.point);
                    }
                    else if (state.Equals(" Unlocked"))
                    {
                        WaypointsManager.waypointHasBeenUnlocked = true;
                        DestroyTowerAtPosition(hit.point);
                    }
                }
            }
        }
    }
    
    private float passedTime = 0f;
    private float deltaTime = 0f;
    private void ControllFrameRate()
    {
        passedTime += Time.deltaTime;

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float currentFrameRate = 1f / deltaTime;

        float deltaFrame = currentFrameRate - targetFrameRate;
        
        entitiesToSpawnPerSecond += deltaFrame * 0.001f;
        if (entitiesToSpawnPerSecond < 0f) entitiesToSpawnPerSecond = 0.0001f;

        int numEntities = (int)(passedTime * entitiesToSpawnPerSecond);
        passedTime -= numEntities / entitiesToSpawnPerSecond;

        SpawnUnitEntities(numEntities);
    }

    /// <summary>
    /// Updates the display of the number of units.
    /// </summary>
    private void UpdateTextBox()
    {
        entityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<LifetimeData>());
        int numEntities = entityQuery.CalculateEntityCount();

        textBox.text = ENTITIES_TEXT + numEntities.ToString();
    }

    /// <summary>
    /// Clears the memory.
    /// </summary>
    private void OnDestroy()
    {
        store.Dispose();
    }
}