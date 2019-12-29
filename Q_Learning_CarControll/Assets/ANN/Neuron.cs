using System.Collections.Generic;

public class Neuron
{
    // Liczba wejść neuronu.
    public int numberOfInputs;

    // Liczba dodawana do wyjścia neuronu (offset) wyrównująca straty.
    public float bias;

    // Wartość wyjściowa neuronu.
    public float output;

    // Gradient błędu wartości wyjściowej.
    public float errorGradient;
    
    // Lista wag dla danych wejściowych, tworzona na podstawie ich ilości.
    public List<float> weights = new List<float>();

    // Lista danych wejściowych.
    public List<float> inputs = new List<float>();

    // Konstruktor neuronu.
    public Neuron(int numOfInputs)
    {
        // Przypisywanie losowej wartości z przedziału <-1, 1>.
        bias = UnityEngine.Random.Range(-1f, 1f);

        // Przypisanie liczby danych wejściowych.
        numberOfInputs = numOfInputs;

        // Przypisywanie losowej wartości wag z przedziału <-1, 1>, dla każdego z wejść.
        for (int i = 0; i < numOfInputs; i++)
            weights.Add(UnityEngine.Random.Range(-1f, 1f));
    }
}
