using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovCam : MonoBehaviour {
    Vector3 position = new Vector3();
    Quaternion rotatnion = new Quaternion();
    [SerializeField]GameObject ship = new GameObject();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        SetPosition();
	}

    public void SetPosition()
    {
        rotatnion = ship.transform.rotation;
        position = ship.transform.position;
        if (Input.GetKey(KeyCode.W))
        {
            position.x = position.x + 10*Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            position.x = position.x - 10 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            position.z = position.z + 10 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            position.z = position.z - 10 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            position.y = position.y - 10 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.C))
        {
            position.y = position.y + 10 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rotatnion.y = rotatnion.y + 10 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rotatnion.y = rotatnion.y - 10 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotatnion.z = rotatnion.z + 10 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rotatnion.z = rotatnion.z - 10 * Time.deltaTime;
        }
        ship.transform.rotation = rotatnion;
        ship.transform.position = position;
        
    }
}
