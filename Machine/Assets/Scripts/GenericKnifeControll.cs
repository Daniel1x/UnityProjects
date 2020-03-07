using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericKnifeControll : MonoBehaviour
{
    public bool constantMove = true;
    [Range(0f, 0.25f)] public float maxDelta = 0.05f;

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
