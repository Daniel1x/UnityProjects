using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class GameManager : MonoBehaviour {
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private LineRenderer linePrefab;
    private bool isPlayerOneTurn = true;
    [SerializeField] private Color playerOneColor;
    [SerializeField] private Color playerTwoColor;
    [SerializeField] private bool playWithComputer = false;
    
    public void EnableAI() { playWithComputer = !playWithComputer; }
    
    private Camera camera = null;
    [SerializeField] private Text playerTextBox;

    private GameObject movementDot = null;
    private MapCreator map = null;
    private List<Vector2Int> playablePoints = new List<Vector2Int>();
    private List<Vector2Int> possibleMovePoints = new List<Vector2Int>();
    private List<Vector2Int> visitedPoints = new List<Vector2Int>();
    private List<Vector2Int> boundriesPoints = new List<Vector2Int>();
    private List<Move> moves = new List<Move>();
    private Vector2Int currentPoint;
    private Vector2Int[] goalPoints = new Vector2Int[6];
    private Vector2Int startPoint;
    
    public UnityEvent OnWinGame;
    public UnityEvent OnDrawGame;
    public event IsPlayerOneWinDelegate OnPlayerWin;
    public delegate void IsPlayerOneWinDelegate(bool isPlayerOneTurn);

    private void Start()
    {
        camera = Camera.main;
        map = FindObjectOfType<MapCreator>();
        if (map == null)
        {
            Debug.LogError("Map Creator has not been found!");
            return;
        }

        movementDot = Instantiate(dotPrefab, Vector3.one * 100f, Quaternion.identity) as GameObject;
        movementDot.transform.localScale = Vector3.one * 3f;
        movementDot.GetComponent<SpriteRenderer>().color = Color.yellow;

        goalPoints = map.GetGoalPoints();
        startPoint = map.GetStartPoint();
        possibleMovePoints = map.GetPlayablePoints();
        boundriesPoints = map.GetBoundriesPositionsList();
        currentPoint = startPoint;

        BlockMovesOnBoundaries();
        
        DrawPointAt(startPoint, Color.red);
        visitedPoints.Add(startPoint);
    }

    private void BlockMovesOnBoundaries()
    {
        List<Vector2Int> boundaries = map.GetBoundriesPositionsList();
        int boundariesCount = boundaries.Count;
        for(int i = 0; i < boundariesCount; i++)
        {
            Move move = new Move(boundaries[i], boundaries[(i + 1) % boundariesCount]);
            AddNewMove(move);
        }
    }

    private void DrawPointAt(Vector2Int spawnPosition, Color color, float scaleFactor = 2f)
    {
        Vector3 spawnPos = new Vector3(spawnPosition.x, spawnPosition.y);
        GameObject dot = Instantiate(dotPrefab, spawnPos, Quaternion.identity, this.transform);
        dot.GetComponent<SpriteRenderer>().color = Color.red;
        dot.transform.localScale = scaleFactor * Vector3.one;
    }

    private void OnMouseDown()
    {
        CheckForBlockedBall();

        Vector2 mousePos = Input.mousePosition;
        Vector3 worldPos = camera.ScreenToWorldPoint(mousePos);
        Vector2Int clickOnGridPosition = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
        if (currentPoint != clickOnGridPosition && IsPossibleToMoveToThisPoint(clickOnGridPosition))
        {
            MoveToNewPoint(clickOnGridPosition);
            DrawPointAt(clickOnGridPosition, Color.red);
        }
    }

    private void CheckForBlockedBall()
    {
        bool isAnyMovePossible = false;
        Vector2Int actualPoint = currentPoint;
        for(int i = 0; i < Directions.arraySize; i++)
        {
            Vector2Int thisPoint = actualPoint + Directions.directions[i];
            if (!possibleMovePoints.Contains(thisPoint))
            {
                continue;
            }

            Move move = new Move(actualPoint, thisPoint);
            if (!moves.Contains(move))
            {
                isAnyMovePossible = true;
            }
        }
        if (!isAnyMovePossible)
        {
            OnDrawGame.Invoke();
        }
    }

    private void MoveToNewPoint(Vector2Int clickOnGridPosition)
    {
        MoveYellowDot(clickOnGridPosition);
        DrawPathTo(clickOnGridPosition);
        ChangePlayerTurn(currentPoint, clickOnGridPosition);
        visitedPoints.Add(clickOnGridPosition);
        currentPoint = clickOnGridPosition;
        CheckForWin(currentPoint);
    }

    private void MoveYellowDot(Vector2Int clickOnGridPosition)
    {
        movementDot.transform.position = new Vector3(clickOnGridPosition.x, clickOnGridPosition.y, 0f);
    }

    private void CheckForWin(Vector2Int currentPoint)
    {
        for(int i = 0; i < 3; i++)
        {
            if (currentPoint == goalPoints[i])
            {
                OnWinGame.Invoke();
                OnPlayerWin.Invoke(true);
                MoveYellowDot(new Vector2Int(100, 100));
                return;
            }
            if (currentPoint == goalPoints[i+3])
            {
                OnWinGame.Invoke();
                OnPlayerWin.Invoke(false);
                MoveYellowDot(new Vector2Int(100, 100));
                return;
            }
        }
    }

    private void ChangePlayerTurn(Vector2Int currentPoint, Vector2Int clickOnGridPosition)
    {
        if (PointHasBeenVisited(clickOnGridPosition) || MoveCrossedOtherMove(currentPoint, clickOnGridPosition) || PointBelongsToBorders(clickOnGridPosition)) 
        {
            // Do not change turn;
        }
        else
        {
            isPlayerOneTurn = !isPlayerOneTurn;
            playerTextBox.text = isPlayerOneTurn ? "Player 1" : "Player 2";
        }
    }

    private bool PointBelongsToBorders(Vector2Int clickOnGridPosition)
    {
        return boundriesPoints.Contains(clickOnGridPosition);
    }

    private bool MoveCrossedOtherMove(Vector2Int currentPoint, Vector2Int clickOnGridPosition)
    {
        if (currentPoint.x == clickOnGridPosition.x || currentPoint.y == clickOnGridPosition.y) // Points has the same x or y value.
        {
            return false;
        }

        Vector2Int crossPoint1 = new Vector2Int(currentPoint.x, clickOnGridPosition.y);
        Vector2Int crossPoint2 = new Vector2Int(clickOnGridPosition.x, currentPoint.y);
        Move crossedMove = new Move(crossPoint1, crossPoint2);
        
        return moves.Contains(crossedMove) || moves.Contains(crossedMove.Rotated());
    }

    private bool PointHasBeenVisited(Vector2Int clickOnGridPosition)
    {
        return visitedPoints.Contains(clickOnGridPosition);
    }

    private void DrawPathTo(Vector2Int clickOnGridPosition)
    {
        LineRenderer line = Instantiate(linePrefab, this.transform);
        line.positionCount = 2;
        line.SetPosition(0, Vector2IntToVector3(currentPoint));
        line.SetPosition(1, Vector2IntToVector3(clickOnGridPosition));
        line.material.color = isPlayerOneTurn ? playerOneColor : playerTwoColor;
    }

    private bool IsPossibleToMoveToThisPoint(Vector2Int clickOnGridPosition)
    {
        if (possibleMovePoints.Contains(clickOnGridPosition))
        {
            if (IsNeighbor(clickOnGridPosition) && MoveHasNotBeenMade(currentPoint,clickOnGridPosition))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool MoveHasNotBeenMade(Vector2Int currentPoint, Vector2Int clickOnGridPosition)
    {
        Move thisMove = new Move(currentPoint, clickOnGridPosition);
        if (moves.Contains(thisMove))
        {
            return false;
        }
        else
        {
            AddNewMove(thisMove);
            return true;
        }
    }

    private void AddNewMove(Move thisMove)
    {
        moves.Add(thisMove);
        Move thisMoveBackward = new Move(thisMove.to, thisMove.from);
        moves.Add(thisMoveBackward);
    }

    private bool IsNeighbor(Vector2Int clickOnGridPosition)
    {
        float distance = Vector2Int.Distance(currentPoint, clickOnGridPosition);
        return distance < 1.5f;         // No need to compare. If distance <= sqrt(2), point is a neighbor. 
    }

    private Vector3 Vector2IntToVector3( Vector2Int v2I)
    {
        return new Vector3(v2I.x, v2I.y);
    }

    public void RestartGameManager()
    {
        MoveYellowDot(new Vector2Int(100, 100));
        DestroyOldMap();
        playablePoints.Clear();
        possibleMovePoints.Clear();
        visitedPoints.Clear();
        moves.Clear();
        map = FindObjectOfType<MapCreator>();
        if (map == null)
        {
            Debug.LogError("Map Creator has not been found!");
            return;
        }

        goalPoints = map.GetGoalPoints();
        startPoint = map.GetStartPoint();
        possibleMovePoints = map.GetPlayablePoints();
        currentPoint = startPoint;

        BlockMovesOnBoundaries();

        DrawPointAt(startPoint, Color.red);
        visitedPoints.Add(startPoint);
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

    private void AutoPlay()
    {
        if (isPlayerOneTurn) return;
        if (!playWithComputer) return;
        
        CheckForBlockedBall();

        Vector2 mousePos = Input.mousePosition;
        Vector3 worldPos = camera.ScreenToWorldPoint(mousePos);
        Vector2Int clickOnGridPosition = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));

        clickOnGridPosition = RandomDirection();
        if (currentPoint != clickOnGridPosition && IsPossibleToMoveToThisPoint(clickOnGridPosition))
        {
            MoveToNewPoint(clickOnGridPosition);
            DrawPointAt(clickOnGridPosition, Color.red);
        }
    }

    private Vector2Int RandomDirection()
    {
        Vector2Int clickPoint;
        Vector2Int[] preferedDirections = new Vector2Int[3] { Directions.down, Directions.downLeft, Directions.downRight };

        if (UnityEngine.Random.Range(0f, 100f) > 20f)
        {
            clickPoint = currentPoint + preferedDirections[UnityEngine.Random.Range(0, 3)];
        }
        else
        {
            clickPoint = currentPoint + Directions.directions[UnityEngine.Random.Range(0, 8)];
        }

        return clickPoint;
    }

    private void Update()
    {
        AutoPlay();
    }
}