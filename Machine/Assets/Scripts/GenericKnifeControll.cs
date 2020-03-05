using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericKnifeControll : MonoBehaviour
{
    [SerializeField] private bool constantMove;
    [SerializeField] [Range(0f, 0.25f)] float maxDelta = 0.25f;

    private void Update()
    {
        MoveKnife();
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
}
