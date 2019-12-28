using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipsScript : MonoBehaviour
{
    //private Camera mainCamera;
    public Camera shotCamera;
    public Camera shipCamera;
    public bool isPlayerOneShip;
    public float movePoints = 0;
    float healthPoints;
    float shieldPower;
    //public Missile projectile;

    //set enable of the ship cameras
    public void SpawnCameras()
    {
        shipCamera.enabled = true;
        shotCamera.enabled = false;
    }

    //function which makes the ship to move
    public void MoveCamera(float speedMove, float speedRotation, float fuel)
    {
        //go forward
        if (Input.GetKey(KeyCode.W))
        {
            shipCamera.transform.Translate(Vector3.forward * Time.deltaTime * speedMove);
            movePoints += 0.1f;
        }
        //go right
        if (Input.GetKey(KeyCode.A))
        {
            shipCamera.transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * speedRotation);
            movePoints += 0.1f;
        }
        //go backward
        if (Input.GetKey(KeyCode.S))
        {
            shipCamera.transform.Translate(Vector3.back * Time.deltaTime * speedMove);
            movePoints += 0.1f;
        }
        //go right
        if (Input.GetKey(KeyCode.D))
        {
            shipCamera.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * speedRotation);
            movePoints += 0.1f;
        }
        if (movePoints >= fuel)
        {
            //END OF THE GAME
            Debug.Log("END OF FUEL");
            movePoints = 0;
        }
    }

    //moving the camera and prepare the missile to shot
    public void AimShotCamera(float speedRotation)
    {
        //look up
        if (Input.GetKey(KeyCode.W))
            shotCamera.transform.Rotate(new Vector3(-1, 0, 0) * Time.deltaTime * speedRotation);
        //look left
        if (Input.GetKey(KeyCode.A))
            shotCamera.transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * speedRotation);
        //look down
        if (Input.GetKey(KeyCode.S))
            shotCamera.transform.Rotate(new Vector3(1, 0, 0) * Time.deltaTime * speedRotation);
        //look right
        if (Input.GetKey(KeyCode.D))
            shotCamera.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * speedRotation);
        if (Input.GetKey(KeyCode.X))
            Shot(1.0f,50.0f);
    }

    //functio which spawns missile and shot it into proper dirrection
    public void Shot(float speed, float dmg)
    {
        //GameObject go = GameObject.Find("Sphere");
        //Missile clone = (Missile)Instantiate(projectile, go.transform.position, go.transform.rotation);// as GameObject;
        
        //shotCamera.enabled = false;
        //shipCamera.enabled = false;
        //clone.missileCamera.enabled = true;

        //clone.MoveMissile(speed, clone);
    }

    //switching between cameras in the ship
    public void ChangeShipCamera()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(shipCamera.enabled == true)
            {
                Debug.Log("shot camera");
                shotCamera.enabled = true;
                shipCamera.enabled = false;
            }
            else if(shotCamera.enabled == true)
            {
                Debug.Log("ship camera");
                shotCamera.enabled = false;
                shipCamera.enabled = true;
            }
        }
    }
}