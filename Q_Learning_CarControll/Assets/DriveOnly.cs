using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveOnly : MonoBehaviour
{
    /*
    // Konrtola dotykiem (telefon)
    public bool controlledByTouch = false;
    public Joystick joystick;
    */

    // Wartości kontrolne
    float vInput = 0;
    float hInput = 0;

    // Prędkość jazdy.
    public float driveSpeed = 50.0f;
    // Prędkość skrętu.
    public float rotationSpeed = 100.0f;
    // Funkcja manipulująca obiektem na podstawie wejść z klawiatury.
    void Drive()
    {
        /*
        if (!controlledByTouch)
        {
            vInput = Input.GetAxis("Vertical");
            hInput = Input.GetAxis("Horizontal");
        }
        else
        {
            vInput = joystick.Vertical;
            hInput = joystick.Horizontal;
        }
        */

        vInput = Input.GetAxis("Vertical");
        hInput = Input.GetAxis("Horizontal");

        float translation = vInput * driveSpeed * Time.deltaTime;
        float rotation = hInput * rotationSpeed * Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }
    private void Update()
    {
        Drive();
    }
}
