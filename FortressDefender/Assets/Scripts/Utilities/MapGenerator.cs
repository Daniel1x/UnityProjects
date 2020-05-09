using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private bool debugInEditorEnabled = false;

    private void Start()
    {
        debugInEditorEnabled = false;
    }

    private void Update()
    {
        if (!debugInEditorEnabled) return;
        Debug.Log("RESET!");
        ResetMap();
    }


    /// <summary>
    /// Constant value that defines texture ratio.
    /// </summary>
    private const float GRID_TILING = 0.03125f;
    /// <summary>
    /// Wall types.
    /// </summary>
    private enum Side { Front, Right, Back, Left };
    
    /// <summary>
    /// Wall fragment prefab (cube).
    /// </summary>
    [SerializeField] private GameObject cubePrefab = null;
    /// <summary>
    /// Floor gameobject tag.
    /// </summary>
    [SerializeField] private string floorTag = "Floor";
    /// <summary>
    /// Wall gameobject tag.
    /// </summary>
    [SerializeField] private string wallTag = "Wall";
    /// <summary>
    /// Floor material.
    /// </summary>
    [SerializeField] private Material floorMaterial = null;
    /// <summary>
    /// Wall material.
    /// </summary>
    [SerializeField] private Material wallMaterial = null;
    /// <summary>
    /// Width of created map fragment. (X axis)
    /// </summary>
    [SerializeField] [Range(2, 100)] private int width = 3;
    /// <summary>
    /// Hight of created map fragment. (Y axis)
    /// </summary>
    [SerializeField] [Range(2, 100)] private int hight = 3;
    /// <summary>
    /// Length of created map fragment. (Z axis)
    /// </summary>
    [SerializeField] [Range(2, 100)] private int length = 3;
    /// <summary>
    /// Offset of spawn position.
    /// </summary>
    [SerializeField] private Vector3 spawnOffset = new Vector3();
    /// <summary>
    /// Does the map fragment have a front entrance? 
    /// Front - looking along the Z axis.
    /// </summary>
    [SerializeField] bool hasFrontEntrance = false;
    /// <summary>
    /// Does the map fragment have a back entrance?
    /// Back - looking along the -Z axis.
    /// </summary>
    [SerializeField] bool hasBackEntrance = false;
    /// <summary>
    /// Does the map fragment have a right entrance?
    /// Right - looking along the -X axis.
    /// </summary>
    [SerializeField] bool hasRightEntrance = false;
    /// <summary>
    /// Does the map fragment have a left entrance?
    /// Left - looking along the X axis.
    /// </summary>
    [SerializeField] bool hasLeftEntrance = false;
    /// <summary>
    /// Size of entrances.
    /// </summary>
    [SerializeField] [Range(1, 10)] private int entranceSize = 1;

    /// <summary>
    /// Function that creates map fragment gameobject, based on passed values.
    /// </summary>
    /// <param name="width">Width of created map fragment.</param>
    /// <param name="hight">Hight of created map fragment.</param>
    /// <param name="length">Length of created map fragment.</param>
    /// <param name="position">Offset position of created map fragment.</param>
    /// <param name="hasFrontEntrance">Does this map fragment have a front entrance?</param>
    /// <param name="hasRightEntrance">Does this map fragment have a right entrance?</param>
    /// <param name="hasBackEntrance">Does this map fragment have a back entrance?</param>
    /// <param name="hasLeftEntrance">Does this map fragment have a left entrance?</param>
    /// <param name="entranceSize">Size of entrances.</param>
    private void CreateMap(float width, float hight, float length, Vector3 position, bool hasFrontEntrance, bool hasRightEntrance, bool hasBackEntrance, bool hasLeftEntrance, float entranceSize)
    {
        GameObject mapFragment = new GameObject("MapFragment") as GameObject;
        mapFragment.transform.parent = this.transform;

        CreateFloor(width, length, position, mapFragment.transform);
        CreateWall(Side.Front, width, hight, length, position, mapFragment.transform, hasFrontEntrance, entranceSize);
        CreateWall(Side.Right, width, hight, length, position, mapFragment.transform, hasRightEntrance, entranceSize);
        CreateWall(Side.Back, width, hight, length, position, mapFragment.transform, hasBackEntrance, entranceSize);
        CreateWall(Side.Left, width, hight, length, position, mapFragment.transform, hasLeftEntrance, entranceSize);
    }

    /// <summary>
    /// Function that creates wall fragment gameobject, based on passed values.
    /// </summary>
    /// <param name="side">Which side is this wall on?</param>
    /// <param name="width">Width of map fragment.</param>
    /// <param name="hight">Hight of map fragment.</param>
    /// <param name="length">Length of map fragment.</param>
    /// <param name="floorPosition">Position of floor gameobject.</param>
    /// <param name="parent">Parent gameobject.</param>
    /// <param name="hasEntrance">Does this wall have a entrance.</param>
    /// <param name="entranceSize">Size of entrance.</param>
    private void CreateWall(Side side, float width, float hight, float length, Vector3 floorPosition, Transform parent, bool hasEntrance = false, float entranceSize = 1f)
    {
        Vector3 wallPosition = CalculateWallPosition(side, width, hight, length, floorPosition);
        Quaternion wallRotation = CalculateWallRotation(side);

        GameObject wall = new GameObject();
        wall.transform.position = wallPosition;
        wall.transform.rotation = wallRotation;
        wall.transform.parent = parent;
        wall.name = side + "Wall";

        CreateWallGrid(wall, side, width, hight, length, hasEntrance, entranceSize);
    }

    /// <summary>
    /// Function that creates parts of wall, based on passed values.
    /// </summary>
    /// <param name="wallGO">Wall gameobject parent(wall pivot point).</param>
    /// <param name="side">Which side is this wall on?</param>
    /// <param name="width">Width of map fragment.</param>
    /// <param name="hight">Hight of map fragment.</param>
    /// <param name="length">Length of map fragment.</param>
    /// <param name="hasEntrance">Does this wall have a entrance.</param>
    /// <param name="entranceSize">Size of entrance.</param>
    private void CreateWallGrid(GameObject wallGO, Side side, float width, float hight, float length, bool hasEntrance = false, float entranceSize = 1f)
    {
        // If wall has entrance: create two parts of wall (wallLeft and wallRight).
        if (hasEntrance)
        {
            float halfEntranceSize = entranceSize / 2f;
            float halfWidth = width / 2f;
            float halfLength = length / 2f;
            float halfWallWidth = halfWidth - halfEntranceSize;
            float halfWallLength = halfLength - halfEntranceSize;

            // Instantiate wall fragment.
            GameObject wallRight;
            try
            {
                wallRight = Instantiate(cubePrefab, wallGO.transform);
            }
            catch
            {
                Debug.LogError("Cube prefab is NULL, find it!");
                return;
            }
            // Set wall fragment name and tag.
            wallRight.name = "RightWall";
            wallRight.tag = wallTag;

            // Set wall fragment size.
            Vector3 wallRightLocalScale = CalculateWallScale(side, halfWallWidth, hight, halfWallLength);
            wallRightLocalScale = ClampVector(wallRightLocalScale, 0.25f, float.MaxValue);
            wallRight.transform.localScale = wallRightLocalScale;

            // Set wall fragment position.
            Vector3 wall1Position = Vector3.right * ((wallRightLocalScale.x / 2f) + halfEntranceSize);
            wallRight.transform.localPosition = wall1Position;

            // Instantiate wall fragment.
            GameObject wallLeft = Instantiate(cubePrefab, wallGO.transform);

            // Set wall fragment name and tag.
            wallLeft.name = "LeftWall";
            wallLeft.tag = wallTag;

            // Set wall fragment size.
            Vector3 wallLeftLocalScale = CalculateWallScale(side, halfWallWidth, hight, halfWallLength);
            wallLeftLocalScale = ClampVector(wallLeftLocalScale, 0.25f, float.MaxValue);
            wallLeft.transform.localScale = wallLeftLocalScale;

            // Set wall fragment position and rotation.
            Vector3 wall2Position = Vector3.left * ((wallLeftLocalScale.x / 2f) + halfEntranceSize);
            wallLeft.transform.localPosition = wall2Position;
            wallLeft.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            // Set material on walls.
            Material wallMaterial;
            try
            {
                wallMaterial = new Material(this.wallMaterial);
            }
            catch
            {
                Debug.LogError("Wall Material is NULL, find it!");
                return;
            }
            wallMaterial.mainTextureScale = CalculateWallTextureScale(side, halfWallWidth, hight, halfWallLength);
            wallRight.GetComponent<MeshRenderer>().material = wallMaterial;
            wallLeft.GetComponent<MeshRenderer>().material = wallMaterial;
        }
        else
        {
            // Instantiare wall.
            GameObject wall;
            try
            {
                wall = Instantiate(cubePrefab, wallGO.transform);
            }
            catch
            {
                Debug.LogError("Cube prefab is NULL, fint it!");
                return;
            }
            // Set wall name, tag and position.
            wall.name = side + "Wall";
            wall.tag = wallTag;
            wall.transform.localScale = CalculateWallScale(side, width, hight, length);
            
            // Set material on wall.
            Material wallMaterial;
            try
            {
                wallMaterial = new Material(this.wallMaterial);
            }
            catch
            {
                Debug.LogError("Wall Material is NULL, find it!");
                return;
            }
            wallMaterial.mainTextureScale = CalculateWallTextureScale(side, width, hight, length);
            wall.GetComponent<MeshRenderer>().material = wallMaterial;
        }
    }

    /// <summary>
    /// Function that clamps values of Vector3.
    /// </summary>
    /// <param name="value">Vector3.</param>
    /// <param name="minValue">Minimum value.</param>
    /// <param name="maxValue">Maximum value.</param>
    /// <param name="clampX">Clamp X value of vector.</param>
    /// <param name="clampY">Clamp Y value of vector.</param>
    /// <param name="clampZ">Clamp Z value of vector.</param>
    /// <returns>Clamped vector.</returns>
    private Vector3 ClampVector(Vector3 value, float minValue, float maxValue, bool clampX = true, bool clampY = false, bool clampZ = false)
    {
        Vector3 v = value;
        if (clampX) v.x = Mathf.Clamp(v.x, minValue, maxValue);
        if (clampY) v.y = Mathf.Clamp(v.y, minValue, maxValue);
        if (clampZ) v.z = Mathf.Clamp(v.z, minValue, maxValue);
        return v;
    }
    
    /// <summary>
    /// Function that calculates wall rotation based on chosen side.
    /// </summary> 
    /// <param name="side">Chosen side.</param>
    private Quaternion CalculateWallRotation(Side side)
    {
        return ChoseBySide(side, Quaternion.Euler(0f, 0f, 0f),
                                 Quaternion.Euler(0f, 90f, 0f),
                                 Quaternion.Euler(0f, 0f, 0f),
                                 Quaternion.Euler(0f, 90f, 0f));
    }

    /// <summary>
    /// Function that calculates wall texture scale, based on chosen side and size of map fragment.
    /// </summary>
    /// <param name="side">Chosen side.</param>
    /// <param name="width">Width of map fragment.</param>
    /// <param name="hight">Hight of map fragment.</param>
    /// <param name="length">Length of map fragment.</param>
    /// <returns></returns>
    private Vector2 CalculateWallTextureScale(Side side, float width, float hight, float length)
    {
        return ChoseBySide(side, new Vector2(width, hight) * GRID_TILING,
                                 new Vector2(length, hight) * GRID_TILING,
                                 new Vector2(width, hight) * GRID_TILING,
                                 new Vector2(length, hight) * GRID_TILING);
    }

    /// <summary>
    /// Function that calculates scale of wall, based on chosen side and size of map fragment.
    /// </summary>
    /// <param name="side">Chosen side.</param>
    /// <param name="width">Width of map fragment.</param>
    /// <param name="hight">Hight of map fragment.</param>
    /// <param name="length">Length of map fragment.</param>
    /// <returns></returns>
    private Vector3 CalculateWallScale(Side side, float width, float hight, float length)
    {
        return ChoseBySide(side, new Vector3(width, hight, 1),
                                 new Vector3(length, hight, 1),
                                 new Vector3(width, hight, 1),
                                 new Vector3(length, hight, 1));
    }

    /// <summary>
    /// Function that calculates position of wall.
    /// </summary>
    /// <param name="side">Chosen side.</param>
    /// <param name="width">Width of map fragment.</param>
    /// <param name="hight">Hight of map fragment.</param>
    /// <param name="length">Length of map fragment.</param>
    /// <param name="floorPosition">Position of floor gameobject.</param>
    /// <returns></returns>
    private Vector3 CalculateWallPosition(Side side, float width, float hight, float length, Vector3 floorPosition)
    {
        return ChoseBySide(side, floorPosition + new Vector3(0f, hight / 2f, -0.5f - (length / 2f)),
                                 floorPosition + new Vector3(0.5f + (width / 2f), hight / 2f, 0f),
                                 floorPosition + new Vector3(0f, hight / 2f, 0.5f + (length / 2f)),
                                 floorPosition + new Vector3(-0.5f - (width / 2f), hight / 2f, 0f));
    }

    /// <summary>
    /// Function that creates floor gameobject, based on passed size.
    /// </summary>
    /// <param name="floorWidth">Width of floor.</param>
    /// <param name="floorLength">Length of floor.</param>
    /// <param name="position">Position of floor gameobject.</param>
    /// <param name="parent">Floor gameobject parent.</param>
    private void CreateFloor(float floorWidth, float floorLength, Vector3 position, Transform parent)
    {
        float width = floorWidth + 2;
        float length = floorLength + 2;

        // Instantiate floor gameobject.
        GameObject floor;
        try
        {
            floor = Instantiate(cubePrefab, position - (Vector3.up / 2f), Quaternion.identity, parent) as GameObject;
        }
        catch
        {
            Debug.LogError("Cube prefab is NULL, find it!");
            return;
        }

        // Set floor name, tag and scale.
        floor.name = "Floor";
        floor.tag = floorTag;
        floor.transform.localScale = new Vector3(width, 1, length);

        // Set floor material.
        Material floorMaterial;
        try
        {
            floorMaterial = new Material(this.floorMaterial);
        }
        catch
        {
            Debug.LogError("Floor Material is NULL, find it!");
            return;
        }
        Material material = floor.GetComponent<MeshRenderer>().material = floorMaterial;
        Vector2 tiling = material.mainTextureScale;
        material.mainTextureScale = new Vector2(width, length) * GRID_TILING;
    }
    
    /// <summary>
    /// Public function that creates map fragment, based on current values.
    /// </summary>
    public void CreateMap()
    {
        CreateMap(width, hight, length, transform.position + spawnOffset, hasFrontEntrance, hasRightEntrance, hasBackEntrance, hasLeftEntrance, entranceSize);
    }

    /// <summary>
    /// Public function that creates map fragment, based on random values.
    /// </summary>
    public void CreateRandomMapFragment()
    {
        width = UnityEngine.Random.Range(4, 20);
        hight = UnityEngine.Random.Range(1, 6);
        length = UnityEngine.Random.Range(4, 20);
        hasFrontEntrance = UnityEngine.Random.Range(0f, 1f) > 0.5f;
        hasBackEntrance = UnityEngine.Random.Range(0f, 1f) > 0.5f;
        hasRightEntrance = UnityEngine.Random.Range(0f, 1f) > 0.5f;
        hasLeftEntrance = UnityEngine.Random.Range(0f, 1f) > 0.5f;
        entranceSize = UnityEngine.Random.Range(1, 4);
        CreateMap(width, hight, length, Vector3.zero, hasFrontEntrance, hasRightEntrance, hasBackEntrance, hasLeftEntrance, entranceSize);
    }

    /// <summary>
    /// Function that destroys all the children gameobjects.
    /// </summary>
    /// <param name="parent">Parent gameobject.</param>
    /// <param name="inEditorMode">Enable destroy in editor mode.</param>
    private void DestroyAllChildrens(Transform parent, bool inEditorMode = false)
    {
        while (parent.childCount > 0)
        {
            foreach (Transform child in parent)
            {
                DestroyAllChildrens(child, inEditorMode);
                if (inEditorMode)
                {
                    DestroyImmediate(child.gameObject);
                }
                else
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Function that destroy every map fragment, in editor mode.
    /// </summary>
    public void DestroyMap()
    {
        DestroyAllChildrens(transform, true);
    }

    /// <summary>
    /// Function that destroy current map fragment and create new one.
    /// </summary>
    public void ResetMap()
    {
        DestroyAllChildrens(transform, true);
        CreateMap(width, hight, length, transform.position + spawnOffset, hasFrontEntrance, hasRightEntrance, hasBackEntrance, hasLeftEntrance, entranceSize);
    }

    /// <summary>
    /// Return values based on chosen side.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="side">Chosen side</param>
    /// <param name="front">Returned on front side</param>
    /// <param name="right">Returned on front right</param>
    /// <param name="back">Returned on front back</param>
    /// <param name="left">Returned on front left</param>
    /// <returns></returns>
    private T ChoseBySide<T>(Side side, T front, T right, T back, T left)
    {
        T t;
        switch (side)
        {
            case Side.Front:
                t = front;
                break;
            case Side.Right:
                t = right;
                break;
            case Side.Back:
                t = back;
                break;
            case Side.Left:
                t = left;
                break;
            default:
                t = front;
                break;
        }
        return t;
    }
}
