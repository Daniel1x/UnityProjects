using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CarBrain : MonoBehaviour
{
    // Pokaż GUI.
    public bool showGUI = true;
    // Skala czasu.
    public float TimeScale = 1f;
    // Wczytywanie wag.
    public bool LoadTrainedWeights = true;
    // Kontrola ruchu.
    public bool controllEnabled = false;

    // Sieć neuronowa.
    ArtificialNeuralNetwork ann;

    // Prędkość jazdy.
    public float speed = 50.0f;
    // Prędkość skrędu.
    public float rotationSpeed = 100.0f;
    // Zasięg widzenia (Raycast length).
    public float visibleDistance = 200.0f;
    // Liczba miejsc po przecinku.
    public int decimalPlaces = 1;

    // Losowe akcje.
    public bool enableRandom = true;
    // Liczba dostępnych akcji.
    int actions = 3;
    // Wartość nagrody związanej z akcjami.
    float reward = 0f;
    // Pamięć, jako lista akcji i nagród.
    List<Replay> replayMemory = new List<Replay>();
    // Maksymalna pojemność pamięci, w przypadku przepełnienia najstarsze wartości będą nadpisywane.
    int mCapacity = 10000;
    // Wartość określająca jak duży wpływ na warotść nagrody mają przyszłe stany.
    float discount = 0.99f;
    // Procent szansy na wybranie losowej akcji.
    float exploreRate = 100f;
    // Maksymalny procent wyboru losowej akcji.
    float maxExploreRate = 100f;
    // Minimalny procent wyboru losowej akcji.
    float minExploreRate = 0.01f;
    // Spadek wartości szansy losowego wyboru, co wywołanie Update().
    float exploreDecay = 0.001f;
    // Licznik kolizji.
    int failCount = 0;
    // Licznik najlepszego czasu bez kolizji.
    float timer = 0f;
    // Najlepszy czas bez kolizji.
    float maxDriveTime = 0f;
    // Pozycja startowa.
    Vector3 startPos = new Vector3();
    // Rotacja startowa.
    Quaternion startRot = new Quaternion();

    // Start, Ustawienia.
    private void Start()
    {
        // Pozycja startowa.
        startPos = transform.position;
        // Rotacja startowa.
        startRot = transform.rotation;
        // Konstruktor sieci.
        ann = new ArtificialNeuralNetwork(5, 3, 1, 10, 0.1f);
        // Przyspieszenie czasu.
        Time.timeScale = TimeScale;
        // Wczytywanie wag.
        if (LoadTrainedWeights) ann.LoadWeightsFromFile("QCarTrained");
    }

    // GUI do podglądu zmiennych.
    GUIStyle guiStyle = new GUIStyle();
    private void OnGUI()
    {
        if (!showGUI) return;
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 600, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 500, 30), "Fails: " + failCount, guiStyle);
        GUI.Label(new Rect(10, 50, 500, 30), "Decay Rate: " + ActivationFunction.RoundValue(exploreRate, 3), guiStyle);
        GUI.Label(new Rect(10, 75, 500, 30), "Last Best Drive: " + ActivationFunction.RoundValue(maxDriveTime, 2), guiStyle);
        GUI.Label(new Rect(10, 100, 500, 30), "This Drive: " + ActivationFunction.RoundValue(timer, 2), guiStyle);
        GUI.EndGroup();
    }

    private void ResetCar()
    {
        transform.position = startPos;
        transform.rotation = startRot;
        GetComponent<CarState>().crashed = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        // Licznik czasu.
        timer += Time.deltaTime;
        // Lista stanów, wejść sieci.
        List<float> states = new List<float>();
        // Wartości jakości Q.
        List<float> qValues = new List<float>();

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

        // Wypełnienie stanów danymi o odległościach od ścian.
        states.Add(forwardDistance);
        states.Add(rightDistance);
        states.Add(right45Distance);
        states.Add(leftDistance);
        states.Add(left45Distance);

        // Obliczanie wartości jakości Q, wykorzystując funkcję SoftMax.
        qValues = SoftMax(ann.CalculateOutputs(states));
        // Najlepsza(największa) wartość Q z wszystkich wartości.
        float nextMaxQValue = qValues.Max();
        // Indeks najlepszej wartości.
        int maxQIndex = qValues.ToList().IndexOf(nextMaxQValue);

        if (enableRandom)
        {
            // Zmniejszenie szansy na losową akcję, w granicach min-max.
            exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);
            // Losowy wybór akcji.
            if (UnityEngine.Random.Range(0f, 100f) < exploreRate)
                maxQIndex = UnityEngine.Random.Range(0, actions);
        }

        // Rotacja.
        float rotate = 0f;
        // Akcja.
        if (maxQIndex == 0)
        {
            rotate = -1f;
        }
        else if (maxQIndex == 1)
        {
            rotate = 0f;
        }
        else if (maxQIndex == 2)
        {
            rotate = 1f;
        }
        // Rotacja.
        transform.Rotate(0, rotate * rotationSpeed * Time.deltaTime, 0);
        // Do przodu.
        transform.Translate(0, 0, speed * Time.deltaTime);

        //Jeśli wykryto kolizję to nagroda jest ujemna.
        if (GetComponent<CarState>().crashed)
            reward = -1f;
        else
            reward = 0.1f;

        // Zapis stanów i odpowiadającej im nagrody.
        Replay lastMemory = new Replay(forwardDistance,
                                       rightDistance,
                                       right45Distance,
                                       leftDistance,
                                       left45Distance,
                                       reward);
        // Jeśli pamięć jest pełna, usuń najstarsze dane.
        if (replayMemory.Count > mCapacity)
            replayMemory.RemoveAt(0);

        // Dodawanie ostatnich danych.
        replayMemory.Add(lastMemory);

        // Trenowanie po wypadku.
        if (GetComponent<CarState>().crashed)
        {
            // Pętla poruszająca się po zapisanych danych w pamięci, od końca.
            for (int i = replayMemory.Count - 1; i >= 0; i--)
            {
                // Lista wartości Q dla aktualnych stanów.
                List<float> actualOutputs = new List<float>();
                // Lista wartości Q dla następnego stanu.
                List<float> nextOutputs = new List<float>();
                // Obliczanie wartości Q dla aktualnych stanów.
                actualOutputs = SoftMax(ann.CalculateOutputs(replayMemory[i].states));

                // Najlepsza wartość Q.
                float actualMaxQValue = actualOutputs.Max();
                // Akcja odpowiadająca najlepszej wartości Q.
                int action = actualOutputs.ToList().IndexOf(actualMaxQValue);
                // Sprzężenie zwrotne
                float feedback;
                // Jeśli dane aktualnego stanu są ostatnie na liście (koniec danych) lub nagroda jest ujemna(piłka upadła, koniec sekwencji).
                if (i == replayMemory.Count - 1 || replayMemory[i].reward == -1)
                    // Sprzężenie jest równe wartości aktualnej nagrody.
                    feedback = replayMemory[i].reward;
                // W przeciwnym wypadku:
                else
                {
                    // Obliczamy wartości Q następnej akcji.
                    nextOutputs = SoftMax(ann.CalculateOutputs(replayMemory[i + 1].states));
                    // Najlepsza wartość Q.
                    nextMaxQValue = nextOutputs.Max();
                    // Sprzężenie jest równe wartości aktualnej nagrody powiększonej o najlepszą wartość Q pomnożoną przez współczynnik pomniejszający.
                    // Na podstawie równania Bellmana.
                    feedback = replayMemory[i].reward + discount * nextMaxQValue;
                }
                // Dla wybranej akcji zmieniona zostaje warotść odpowiedniego wyjścia sieci neuronowej (dla aktualnych stanów) używając sprzężenia zwrotnego.
                actualOutputs[action] = feedback;
                // Uczenie sieci neuronowej na podstawie danych stanów oraz zmienionych wartości wyjść.
                ann.Train(replayMemory[i].states, actualOutputs);
            }
            // Sprawdzenie czy rekordowy czas balansu został poprawiony. 
            if (timer > maxDriveTime)
                maxDriveTime = timer;

            // Reset timera (Kod znajduje się w miejscu wywoływanym gdy piłka upadnie).
            timer = 0;
            // Resetowanie ustawień.
            ResetCar();
            // Czyszczenie pamięci.
            replayMemory.Clear();
            // Zliczanie upadków.
            failCount++;
        }
    }

    private void OnApplicationQuit()
    {
        ann.SaveWeightsToFile("QCar");
    }

    // Funkcja SoftMax, skalująca wartości wejściowe poprzez eksponentę oraz podział przez sumę eksponent wszystkich wartości, 
    // przez co suma wszystkich wartości wyjściowych jest równa 1.
    private List<float> SoftMax(List<float> values)
    {
        // Wartość maksymalna, pozwalająca na przesunięcie wartości eksponent do przedziały (0, 1>.
        float maxValue = values.Max();
        // Zmienna skalująca.
        float scale = 0f;
        // Sumowanie eksponent.
        for (int i = 0; i < values.Count; ++i)
            scale += Mathf.Exp((float)(values[i] - maxValue));

        // Lista wyników.
        List<float> result = new List<float>();
        // Obliczanie eksponent i skalowanie wyników.
        for (int i = 0; i < values.Count; ++i)
            result.Add(Mathf.Exp((float)(values[i] - maxValue)) / scale);
        // Zwracanie wyników.
        return result;
    }
}
