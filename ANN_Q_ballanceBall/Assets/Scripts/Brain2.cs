using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Brain2 : MonoBehaviour
{
    // Obiekt do śledzienia, piłka.
    public GameObject ball;
    public bool enableRandom = false;

    // Sieć neuronowa.
    ArtificialNeuralNetwork ann;

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
    float exploreDecay = 0.0005f;

    // Pozycja startowa piłki.
    Vector3 ballStartPos;
    // Licznik upadków piłki.
    int failCount = 0;
    // Prędkość obrotu platformy, powinna być odpowiednio dobrana aby można było wystarczająco szybko reagować na zmiany położenia piłki.
    float tiltSpeed = 0.5f;

    // Licznik czasu, używany do śledzenia najlepszego czasu.
    float timer = 0;
    // Najlepszy z czasów.
    float maxBalanceTime = 0;

    // Start, Ustawienia.
    private void Start()
    {
        // Konstrukcja sieci neuronowej.
        ann = new ArtificialNeuralNetwork(8, 4, 4, 16, 0.2);
        // Zapisywanie pozycji startowej.
        ballStartPos = ball.transform.position;
        // Przyspieszenie skali czasu.
        Time.timeScale = 5f;
    }

    // GUI do podglądu zmiennych.
    GUIStyle guiStyle = new GUIStyle();
    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 600, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 500, 30), "Fails: " + failCount, guiStyle);
        GUI.Label(new Rect(10, 50, 500, 30), "Decay Rate: " + ActivationFunction.RoundValue(exploreRate, 3), guiStyle);
        GUI.Label(new Rect(10, 75, 500, 30), "Last Best Balance: " + ActivationFunction.RoundValue(maxBalanceTime, 2), guiStyle);
        GUI.Label(new Rect(10, 100, 500, 30), "This Balance: " + ActivationFunction.RoundValue(timer, 2), guiStyle);
        GUI.EndGroup();
    }

    // Update, wywoływany co klatkę.
    private void Update()
    {
        // Resetowanie pozycji piłki po wciśnięciu spacji.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetBall();
        }
    }

    private void FixedUpdate()
    {
        // Licznik czasu.
        timer += Time.deltaTime;
        // Lista stanów, wejść sieci neuronowej.
        List<double> states = new List<double>();
        // Wartości jakości Q.
        List<double> qValues = new List<double>();

        // Wypełnianie stanów, danymi o położeniu i prędkości piłki.
        states.Add(transform.rotation.x);
        states.Add(transform.rotation.z);
        states.Add(ball.transform.position.x);
        states.Add(ball.transform.position.y);
        states.Add(ball.transform.position.z);
        states.Add(ball.GetComponent<Rigidbody>().angularVelocity.x);
        states.Add(ball.GetComponent<Rigidbody>().angularVelocity.y);
        states.Add(ball.GetComponent<Rigidbody>().angularVelocity.z);

        // Obliczanie wartości jakości Q, wykorzystując funkcję SoftMax.
        qValues = SoftMax(ann.CalculateOutputs(states));
        // Najlepsza(największa) wartość Q z wszystkich wartości.
        double nextMaxQValue = qValues.Max();
        // Indeks najlepszej wartości.
        int maxQIndex = qValues.ToList().IndexOf(nextMaxQValue);
        // Zmniejszenie szansy na losową akcję, w granicach min-max.
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);
        if (enableRandom)
        {
            // Losowy wybór akcji.
            if (Random.Range(0f, 100f) < exploreRate)
                maxQIndex = Random.Range(0, 4);
        }
        // Jeżeli najlepszą Q wartość znaleziono na ID 0 to wykonuje się rotacja w prawo.
        if (maxQIndex == 0)
            transform.Rotate(Vector3.right, tiltSpeed * (float)qValues[maxQIndex]);
        // Jeżeli najlepszą Q wartość znaleziono na ID 1 to wykonuje się rotacja w lewo.
        else if (maxQIndex == 1)
            transform.Rotate(Vector3.right, -tiltSpeed * (float)qValues[maxQIndex]);
        else if (maxQIndex == 2)
            transform.Rotate(Vector3.forward, tiltSpeed * (float)qValues[maxQIndex]);
        else if (maxQIndex == 3)
            transform.Rotate(Vector3.forward, -tiltSpeed * (float)qValues[maxQIndex]);

        // Jeśli piłka upadła zmniejszamy nagrodę, jeśli nie to zwiększamy.
        if (ball.GetComponent<BallState>().dropped)
            reward = -1f;
        else
            reward = 0.1f;

        // Zapis stanów i odpowiadającej im nagrody.
        Replay lastMemory = new Replay(transform.rotation.x,
                                       transform.rotation.z,
                                       ball.transform.position.x,
                                       ball.transform.position.y,
                                       ball.transform.position.z,
                                       ball.GetComponent<Rigidbody>().angularVelocity.x,
                                       ball.GetComponent<Rigidbody>().angularVelocity.y,
                                       ball.GetComponent<Rigidbody>().angularVelocity.z,
                                       reward);

        // Jeśli pamięć jest pełna, usuń najstarsze dane.
        if (replayMemory.Count > mCapacity)
            replayMemory.RemoveAt(0);

        // Dodawanie ostatnich danych.
        replayMemory.Add(lastMemory);

        // Trenowanie następuje dopiero gdy piłka zostanie upuszczona.
        if (ball.GetComponent<BallState>().dropped)
        {
            // Pętla poruszająca się po zapisanych danych w pamięci, od końca.
            for (int i = replayMemory.Count - 1; i >= 0; i--)
            {
                // Lista wartości Q dla aktualnych stanów.
                List<double> actualOutputs = new List<double>();
                // Lista wartości Q dla następnego stanu.
                List<double> nextOutputs = new List<double>();
                // Obliczanie wartości Q dla aktualnych stanów.
                actualOutputs = SoftMax(ann.CalculateOutputs(replayMemory[i].states));

                // Najlepsza wartość Q.
                double actualMaxQValue = actualOutputs.Max();
                // Akcja odpowiadająca najlepszej wartości Q.
                int action = actualOutputs.ToList().IndexOf(actualMaxQValue);
                // Sprzężenie zwrotne
                double feedback;
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
            if (timer > maxBalanceTime)
                maxBalanceTime = timer;

            // Reset timera (Kod znajduje się w miejscu wywoływanym gdy piłka upadnie).
            timer = 0;
            // Resetowanie ustawień.
            ResetBall();
            // Czyszczenie pamięci.
            replayMemory.Clear();
            // Zliczanie upadków.
            failCount++;
        }
    }

    // Resetowanie pozycji oraz prędkości piłki.
    private void ResetBall()
    {
        // Reset rotacji platformy.
        transform.rotation = Quaternion.identity;

        ball.transform.position = ballStartPos;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        ball.GetComponent<BallState>().dropped = false;
        timer = 0f;
    }

    // Funkcja SoftMax, skalująca wartości wejściowe poprzez eksponentę oraz podział przez sumę eksponent wszystkich wartości, 
    // przez co suma wszystkich wartości wyjściowych jest równa 1.
    private List<double> SoftMax(List<double> values)
    {
        // Wartość maksymalna, pozwalająca na przesunięcie wartości eksponent do przedziały (0, 1>.
        double maxValue = values.Max();
        // Zmienna skalująca.
        float scale = 0f;
        // Sumowanie eksponent.
        for (int i = 0; i < values.Count; ++i)
            scale += Mathf.Exp((float)(values[i] - maxValue));

        // Lista wyników.
        List<double> result = new List<double>();
        // Obliczanie eksponent i skalowanie wyników.
        for (int i = 0; i < values.Count; ++i)
            result.Add(Mathf.Exp((float)(values[i] - maxValue)) / scale);
        // Zwracanie wyników.
        return result;
    }
}
