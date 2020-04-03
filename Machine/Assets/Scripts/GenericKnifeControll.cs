using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericKnifeControll : MonoBehaviour
{
    [SerializeField] [Range(0f, 100f)] private float moveSpeed = 1f;

    private enum Controll { FollowTouch, MoveByDeltaTouch};
    [SerializeField] private Controll howToMove = Controll.FollowTouch;

    private int screenWidth;
    private int screenHight;

    private Vector2 startTouchPosition;
    private Vector2 actualTouchPosition;

    public bool constantMove = true;
    [Range(0f, 0.25f)] public float maxDelta = 0.05f;

    private void Awake()
    {
        screenWidth = Screen.width;
        screenHight = Screen.height;
    }

    private void Update()
    {
        switch (howToMove)
        {
            case Controll.FollowTouch:
                MoveKnife();
                break;
            case Controll.MoveByDeltaTouch:
                MoveKnifeByDeltaTouch();
                break;
            default:
                break;
        }
    }

    private void MoveKnife()
    {
        if (!constantMove) return;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);

        Vector3 wordPos;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        wordPos = Physics.Raycast(ray, out hit, 100f) ? hit.point : Camera.main.ScreenToWorldPoint(mousePos);
        wordPos.z = 0f;
        transform.position = Vector3.MoveTowards(transform.position, wordPos, maxDelta);
    }

    private void MoveKnifeByDeltaTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                actualTouchPosition = touch.position;
            }
            else
            if (touch.phase == TouchPhase.Moved)
            {
                actualTouchPosition = touch.position;
            }
            Vector2 deltaTouch = actualTouchPosition - startTouchPosition;
            deltaTouch.x /= screenWidth;
            deltaTouch.y /= screenHight;
            Vector3 moveVector = deltaTouch * moveSpeed * Time.deltaTime;

            transform.Translate(moveVector);
        }
        else if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                startTouchPosition = pos;
                actualTouchPosition = pos;
            }
            else
            if (Input.GetMouseButton(0))
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                actualTouchPosition = pos;
            }
            Vector2 deltaTouch = actualTouchPosition - startTouchPosition;
            deltaTouch.x /= screenWidth;
            deltaTouch.y /= screenHight;
            Vector3 moveVector = 1000f * deltaTouch * moveSpeed * Time.deltaTime;

            transform.Translate(moveVector);
        }
    }
}
