using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControllRotation : MonoBehaviour
{
    public float rotationSpeed = 45f;
    Wing wing;
    bool isFront;
    bool isRight;

    public bool controlledByANN = false;

    private void Start()
    {
        wing = GetComponent<Wing>();
        isFront = wing.isFront;
        isRight = wing.isRight;
    }

    void Update()
    {
        if (!controlledByANN)
        {
            // Rotacja sterowana
            transform.Rotate(Input.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed * (isFront ? 1 : 1),
                                0f/* Input.GetAxis("Horizontal") * Time.deltaTime * m_Speed * (isRight ? 1 : -1)*/,
                                Input.GetAxis("Vertical") * Time.deltaTime * rotationSpeed * (isRight ? 1 : -1));
        }
    }

    public void Rotate(int axis, float value, float tiltSpeed)
    {
        switch (axis)
        {
            case 0:
                transform.Rotate(value * tiltSpeed * Time.deltaTime, 0f, 0f);
                break;
            case 1:
                transform.Rotate(value * -tiltSpeed * Time.deltaTime, 0f, 0f);
                break;
            case 2:
                transform.Rotate(0f, value * tiltSpeed * Time.deltaTime * (isRight ? 1 : -1), 0f);
                break;
            case 3:
                transform.Rotate(0f, value * -tiltSpeed * Time.deltaTime * (isRight ? 1 : -1), 0f);
                break;
            case 4:
                transform.Rotate(0f, 0f, value * tiltSpeed * Time.deltaTime * (isRight ? 1 : -1));
                break;
            case 5:
                transform.Rotate(0f, 0f, value * -tiltSpeed * Time.deltaTime * (isRight ? 1 : -1));
                break;
            default:
                Debug.Log("Wrong Axis! ANN Controll.");
                break;
        }
    }

    public void ANNControll(List<double> inputs, int wingNr)
    {
        if(controlledByANN)
        {
            transform.Rotate((float)inputs[wingNr * 3 - 3] * Time.deltaTime * rotationSpeed,
                             (float)inputs[wingNr * 3 - 2] * Time.deltaTime * rotationSpeed,
                             (float)inputs[wingNr * 3 - 1] * Time.deltaTime * rotationSpeed);
        }
    }
}
