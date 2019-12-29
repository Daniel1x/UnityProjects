using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

public class ANNDrive : MonoBehaviour
{
    // Pokaż GUI.
    public bool showGUI = true;
    // Sieć neuronowa.
    ArtificialNeuralNetwork ann;
    // Zasięg widzenia.
    public float visibleDistance = 200f;
    // Liczba epok.
    public int epochs = 1000;
    // Początkowa wartość prędkości uczenia sieci neuronowej.
    [Range(0.001f, 0.999f)] public float learningSpeed = 0.1f;
    // Prędkość jazdy.
    public float speed = 50.0f;
    // Prędkość skrętu.
    public float rotationSpeed = 100.0f;
    // Liczba miejsc po przecinku.
    public int decimalPlaces = 1;

    // Zmienna blokująca Update() w trakcie treningu sieci neuronowej.
    bool trainingDone = false;
    // Zmienna do śledzenia postępów uczenia.
    float trainingProgress = 0;
    // Błąd SSE, suma błędów kwadratowych.
    float sse = 0;
    // Ostatni błąd SSE, do śledzenia postępów i ewentualnej zmiany prędkości uczenia.
    float lastSSE = 1;

    // Zmienne wyjściowe sieci neuronowej, sterujące obiektem.
    public float translation;
    public float rotation;
    public float translationInput;
    public float rotationInput;

    // Zmienna sterująca odczytem wag sieci neurnowej z pliku.
    public bool loadFromFile = true;
    
    void Start()
    {
        // Konstrukcja sieci neurnowej.
        ann = new ArtificialNeuralNetwork(5, 2, 1, 10, learningSpeed);
        // Odczyt wag sieci neuronowej z pliku lub uruchomienie uczenia na podstawie danych.
        if (loadFromFile)
        {
            ann.LoadWeightsFromFile();
            trainingDone = true;
        }
        else
            StartCoroutine(LoadTrainingSet());
    }

    void OnGUI()
    {
        if (!showGUI) return;
        // GUI do śledzenia zmiennych w trakcie uczenia.
        GUI.Label(new Rect(25, 25, 250, 30), "SSE: " + lastSSE);
        GUI.Label(new Rect(25, 40, 250, 30), "Alpha: " + ann.learningSpeedRate);
        GUI.Label(new Rect(25, 55, 250, 30), "Trained: " + trainingProgress);
    }

    IEnumerator LoadTrainingSet()
    {
        // Ścieżka do danych.
        string path = Application.dataPath + "/trainingData.txt";
        // Pojedyncza linia z danych.
        string line;
        if (File.Exists(path))
        {
            // Obliczanie ilości linii danych.
            int lineCount = File.ReadAllLines(path).Length;
            // Otwarcie pliku.
            StreamReader tdf = File.OpenText(path);
            // Listy do przechowywania danych.
            List<float> calcOutputs = new List<float>();
            List<float> inputs = new List<float>();
            List<float> outputs = new List<float>();

            // Pętla wykonywująca uczenie, epochs razy.
            for (int i = 0; i < epochs; i++)
            {
                // Resetowanie błędu.
                sse = 0;
                // Ustawienie wskaźnika pliku na początek.
                tdf.BaseStream.Position = 0;
                // Zapisanie aktualnych wag sieci neuronowej.
                string currentWeights = ann.PrintWeights();
                // Odczyt do końca pliku.
                while ((line = tdf.ReadLine()) != null)
                {
                    // Rozdzielenie linii na dane, względem separatora.
                    string[] data = line.Split(Separator.dataSeparatorChar);
                    // Aktualny błąd.
                    float thisError = 0;
                    // Sprawdzanie czy wyjścia sieci są różne od zera, czyli jest się czego uczyć.
                    if (float.Parse(data[5]) != 0 && float.Parse(data[6]) != 0)
                    {
                        // Czyszczenie wejść i wyjść.
                        inputs.Clear();
                        outputs.Clear();
                        // Dodawanie danych wejściowych.
                        inputs.Add(float.Parse(data[0]));
                        inputs.Add(float.Parse(data[1]));
                        inputs.Add(float.Parse(data[2]));
                        inputs.Add(float.Parse(data[3]));
                        inputs.Add(float.Parse(data[4]));
                        // Mapowanie zmiennych wyjściowych z zakresu <-1,1> do zakresu <0,1> oraz dodawanie ich do listy.
                        float o1 = ActivationFunction.Map(0, 1, -1, 1, float.Parse(data[5]));
                        outputs.Add(o1);
                        float o2 = ActivationFunction.Map(0, 1, -1, 1, float.Parse(data[6]));
                        outputs.Add(o2);
                        // Trenowanie sieci neuronowej na aktualnych danych.
                        calcOutputs = ann.Train(inputs, outputs);
                        // Obliczanie aktualnego błędu. Uśredniona suma kwadratów różnic wyjść.
                        thisError = ((Mathf.Pow(outputs[0] - calcOutputs[0], 2) +
                            Mathf.Pow(outputs[1] - calcOutputs[1], 2))) / 2.0f;
                    }
                    // Sumowanie błędów.
                    sse += thisError;
                }
                // Ustawienie postępów na podstawie numeru wykonywanej epoki.
                trainingProgress = 100f * i / epochs;
                // Skalowanie błędu ze względu na liczbę danych.
                sse /= lineCount;
                
                // W przypadku zwiększenia się błędu wczytywanie poprzednich wag oraz zmniejszenie prędkości uczenia.
                if (lastSSE < sse)
                {
                    ann.LoadWeights(currentWeights);
                    ann.learningSpeedRate = Mathf.Clamp(ann.learningSpeedRate - 0.0001f, 0.001f, 0.9f);
                }
                else // Zwiększenie prędkości uczenia.
                {
                    ann.learningSpeedRate = Mathf.Clamp(ann.learningSpeedRate + 0.0001f, 0.001f, 0.9f);
                    lastSSE = sse;
                }
                // Yield do pracy programu w tle. Bez zamrażania klatek.
                yield return null;
            }
        }
        // Trening zakończony. Odblokowywanie Update().
        trainingDone = true;
        // Zapis wag sieci neurnowej do pliku.
        ann.SaveWeightsToFile("ANNTrained2");
    }

    void Update()
    {
        // Koniec funkcji jeśli trening ciągle trwa.
        if (!trainingDone) return;
        // Listy wejść i obliczonych wyjść.
        List<float> inputs = new List<float>();
        List<float> calcOutputs = new List<float>();

        // Zmienna przechowująca punkt trafienia.
        RaycastHit hit;

        // Skalowane odległości od ścian.
        float forwardDistance = 0, rigthDistance = 0, leftDistance = 0, rigth45Distance = 0, left45Distance = 0;

        // Przód.
        if (Physics.Raycast(transform.position, this.transform.forward, out hit, visibleDistance))
        {
            forwardDistance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }

        // Prawo.
        if (Physics.Raycast(transform.position, this.transform.right, out hit, visibleDistance))
        {
            rigthDistance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }

        // Lewo.
        if (Physics.Raycast(transform.position, -this.transform.right, out hit, visibleDistance))
        {
            leftDistance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }

        // Przód 45 stopni w prawo.
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * this.transform.right, out hit, visibleDistance))
        {
            rigth45Distance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }

        // Przód 45 stopni w lewo.
        if (Physics.Raycast(transform.position,
                            Quaternion.AngleAxis(45, Vector3.up) * -this.transform.right, out hit, visibleDistance))
        {
            left45Distance = 1 - ActivationFunction.RoundValue(hit.distance / visibleDistance, decimalPlaces);
        }

        // Dodawanie zmiennych do listy wejść.
        inputs.Add(forwardDistance);
        inputs.Add(rigthDistance);
        inputs.Add(leftDistance);
        inputs.Add(rigth45Distance);
        inputs.Add(left45Distance);
        // Obliczanie wyjść sterujących.
        calcOutputs = ann.CalculateOutputs(inputs);
        // Mapowanie wyjść z zakresu <0,1> na <-1,1>.
        translationInput = ActivationFunction.Map(-1, 1, 0, 1, calcOutputs[0]);
        rotationInput = ActivationFunction.Map(-1, 1, 0, 1, calcOutputs[1]);
        // Mnożenie wyjść z prędkościami i czasem.
        translation = translationInput * speed * Time.deltaTime;
        rotation = rotationInput * rotationSpeed * Time.deltaTime;
        // Manipulacja obiektem.
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }
}