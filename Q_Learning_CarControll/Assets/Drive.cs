using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Drive : MonoBehaviour
{
    // Prędkość jazdy.
    public float speed = 50.0f;
    // Prędkość skrędu.
    public float rotationSpeed = 100.0f;
    // Zasięg widzenia (Raycast length).
    public float visibleDistance = 50.0f;
    // Liczba miejsc po przecinku.
    public int decimalPlaces = 1;
    // Zebrane dane wejściowe (Odległości od ścian + wejścia z klawiatury).
    List<string> collectedTrainingData = new List<string>();
    // StreamWriter do manipulacji danymi na plikach.
    StreamWriter trainingDataFile;

    // Wejścia sterujące.
    float translationInput = 0f;
    float rotationInput = 0f;

    // Konrtola dotykiem (telefon)
    public bool controlledByTouch = false;
    public Joystick joystick;

    private void Start()
    {
        LoadOldKnowledge();
        // Zapis ścieżki do pliku z danymi.
        string path = Application.dataPath + "/trainingData.txt";
        trainingDataFile = File.CreateText(path);
    }

    private void OnApplicationQuit()
    {
        // Zapisanie wszystkich zebranych danych do pliku.
        foreach (string trainingData in collectedTrainingData)
        {
            trainingDataFile.WriteLine(trainingData);
        }
        trainingDataFile.Close();
    }

    //Nieużywane, tylko wrzucone do pracy
    private void GetMeasurements()
    {
        // Zmienna przechowująca punkt trafienia.
        RaycastHit hit;
        // Skalowane odległości od ścian.
        float forwardDistance = 0;
        float rightDistance = 0;
        float leftDistance = 0;
        float right45Distance = 0;
        float left45Distance = 0;
        // Przód.
        if (Physics.Raycast(transform.position, transform.forward, out hit, visibleDistance))
        {
            forwardDistance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }
        // Prawo.
        if (Physics.Raycast(transform.position, transform.right, out hit, visibleDistance))
        {
            rightDistance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }
        // Lewo.
        if (Physics.Raycast(transform.position, -transform.right, out hit, visibleDistance))
        {
            leftDistance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }
        // Przód 45 stopni w prawo.
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * transform.right, out hit, visibleDistance))
        {
            right45Distance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }
        // Przód 45 stopni w lewo.
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(45, Vector3.up) * -transform.right, out hit, visibleDistance))
        {
            left45Distance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }
        //SaveToFile(forwardDistance, rightDistance, leftDistance, right45Distance, left45Distance);
    }

    private void Update()
    {
        // Sterowanie obiektem.
        DriveCar();

        // Raycasty pokazujące kierunki widzienia.
        Debug.DrawRay(transform.position, transform.forward * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, -transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(45, Vector3.up) * -transform.right * visibleDistance, Color.red);

        // Zmienna przechowująca punkt trafienia.
        RaycastHit hit;
        // Skalowane odległości od ścian.
        float forwardDistance = 0;
        float rightDistance = 0;
        float leftDistance = 0;
        float right45Distance = 0;
        float left45Distance = 0;

        // Przód.
        if (Physics.Raycast(transform.position, transform.forward, out hit, visibleDistance))
        {
            forwardDistance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }
        // Prawo.
        if (Physics.Raycast(transform.position, transform.right, out hit, visibleDistance))
        {
            rightDistance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }
        // Lewo.
        if (Physics.Raycast(transform.position, -transform.right, out hit, visibleDistance))
        {
            leftDistance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }
        // Przód 45 stopni w prawo.
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * transform.right, out hit, visibleDistance))
        {
            right45Distance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }
        // Przód 45 stopni w lewo.
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(45, Vector3.up) * -transform.right, out hit, visibleDistance))
        {
            left45Distance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }

        // Dane w postaci tekstowej, zapisywane do pliku jako linia.
        string trainingData = forwardDistance + Separator.dataSeparatorString 
                            + rightDistance + Separator.dataSeparatorString
                            + leftDistance + Separator.dataSeparatorString
                            + right45Distance + Separator.dataSeparatorString
                            + left45Distance + Separator.dataSeparatorString
                            + ActivationFunction.RoundValue(translationInput, decimalPlaces) + Separator.dataSeparatorString
                            + ActivationFunction.RoundValue(rotationInput, decimalPlaces);

        // Sprawdzenie czy dane wejściowe się nie duplikują.
        if (!collectedTrainingData.Contains(trainingData))
        {
            collectedTrainingData.Add(trainingData);
        }
    }

    // Funkcja manipulująca obiektem na podstawie wejść z klawiatury.
    void DriveCar()
    {
        if (!controlledByTouch)
        {
            translationInput = Input.GetAxis("Vertical");
            rotationInput = Input.GetAxis("Horizontal");
        }
        else
        {
            translationInput = joystick.Vertical;
            rotationInput = joystick.Horizontal;
        }
        float translation = translationInput * speed * Time.deltaTime;
        float rotation = rotationInput * rotationSpeed * Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }

    // Funkcja wczytywująca zapisane ruchy.
    void LoadOldKnowledge()
    {
        string path = Application.dataPath + "/trainingData.txt";
        // Pojedyncza linia z danych.
        if (File.Exists(path))
        {
            int lineCount = File.ReadAllLines(path).Length;
            StreamReader tdf = File.OpenText(path);
            tdf.BaseStream.Position = 0;
            for(int i = 0; i < lineCount; i++)
            {
                collectedTrainingData.Add(tdf.ReadLine());
            }
            tdf.Close();
        }
    }
}
