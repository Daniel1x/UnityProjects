using System;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialNeuralNetwork
{
    // Liczba danych wejściowych sieci neuronowej.
    public int numberOfInputs;

    // Liczba danych wyjściowych sieci neuronowej.
    public int numberOfOutputs;

    // Liczba ukrytych warstw sieci neuronowej.
    public int numberOfHiddenLayers;

    // Liczba neuronów w ukrytej warstwie sieci neuronowej.
    public int numberOfNeuronsPerHiddenLayer;

    // Wartość definiująca wpływ pojedyńczego neuronu na wartość błędu oraz szybkość jego korekty.
    public double learningSpeedRate;

    // Lista warstw sieci neuronowej.
    List<Layer> layers = new List<Layer>();

    // Konstruktor sieci neuronowej.
    public ArtificialNeuralNetwork(int numOfInputs, int numOfOutputs, int numOfHiddenLayers, int numOfNeuronsPerHiddenLayer, double valOfLearningSpeedRate)
    {
        // Ustawienie odpowiedniego separatora liczb (format 123.456).
        Separator.Set();

        // Przypisywanie wartości zmiennych.
        numberOfInputs = numOfInputs;
        numberOfOutputs = numOfOutputs;
        numberOfHiddenLayers = numOfHiddenLayers;
        numberOfNeuronsPerHiddenLayer = numOfNeuronsPerHiddenLayer;
        learningSpeedRate = valOfLearningSpeedRate;

        // Sprawdzenie czy sieć neuronowa posiada warstwy ukryte.
        if (numberOfHiddenLayers > 0)
        {
            // Tworzenie pierwszej warstwy ukrywej, która posiada liczbę wejść odpowiednią do liczby danych wejściowych.
            layers.Add(new Layer(numberOfNeuronsPerHiddenLayer, numberOfInputs));

            // Tworzenie kolejnych warstw ukrytych, posiadających tyle samo neuronów co wejść.
            for (int i = 0; i < numberOfHiddenLayers - 1; i++)
                layers.Add(new Layer(numberOfNeuronsPerHiddenLayer, numberOfNeuronsPerHiddenLayer));

            // Tworzenie warstwy wyjściowej, posiadającej daną liczbę wyjść oraz liczbę wejść pasującą do poprzedzającej warstwy ukrytej.
            layers.Add(new Layer(numberOfOutputs, numberOfNeuronsPerHiddenLayer));
        }
        // W przypadku braku warstw ukrytych tworzona jest jedna warstwa wyjściowa, przetwarzająca dane wejściowe w dane wyjściowe.
        else
            layers.Add(new Layer(numberOfOutputs, numberOfInputs));
    }

    // Funkcja trenująca sieć neuronową, uaktualniająca wagi na podstawie obliczonych wyjść.
    public List<double> Train(List<double> inputValues, List<double> desiredOutputs)
    {
        // Utworzenie listy na dane.
        List<double> outputs = new List<double>();

        // Obliczanie wyjść na podstawie danych.
        outputs = CalculateOutputs(inputValues, desiredOutputs);

        // Uaktualnianie wag sieci neuronowej.
        UpdateWeights(outputs, desiredOutputs);

        // Zwracanie wyników
        return outputs;
    }

    // Funkcja obliczająca wyjścia, nie zmieniająca wag sieci neuronowej.
    public List<double> CalculateOutputs(List<double> inputValues, List<double> desiredOutputs = null)
    {
        // Lista danych wejściowych.
        List<double> inputs = new List<double>();

        // Lista danych wyjściowych.
        List<double> outputs = new List<double>();

        // Sprawdzenie czy liczba danych wejściowych odpowiada liczbie wejść sieci neuronowej.
        if (inputValues.Count != numberOfInputs)
        {
            Debug.Log("Error: Number of inputs must be equal to " + numberOfInputs);
            return outputs;
        }

        // Przypisanie wartości wejściowych.
        inputs = new List<double>(inputValues);

        // Pętla poruszająca się po warstwach sieci neuronowej.
        for (int layerID = 0; layerID < numberOfHiddenLayers + 1; layerID++)
        {
            // Sprawdzenie czy dana warstwa jest warstwą wejściową, jeżeli nie to danymi wejściowymi stają się dane wyjściowe z poprzedniej warstwy.
            if (layerID != 0)
                inputs = new List<double>(outputs);

            // Czyszczenie niepotrzebnych pozostałości w danych wyjściowych, przygotowanie do kolejnego nadpisania.
            outputs.Clear();

            // Pętla poruszająca się po neuronach warstwy sieci neuronowej.
            for (int neuronID = 0; neuronID < layers[layerID].numberOfNeurons; neuronID++)
            {
                // Zmienna przechowująca sumę wejść pomnożonych przez odpowiadające im wagi.
                double value = 0;

                // Czyszczenie wejść neuronu.
                layers[layerID].neurons[neuronID].inputs.Clear();

                // Pętla poruszająca się po wejściach danego neuronu.
                for (int inputID = 0; inputID < layers[layerID].neurons[neuronID].numberOfInputs; inputID++)
                {
                    //Dodanie nowej wartości na wejście neuronu.
                    layers[layerID].neurons[neuronID].inputs.Add(inputs[inputID]);

                    //Dodanie wartości danego wejścia pomnożonego przez odpowiadającą mu wagę.
                    value += layers[layerID].neurons[neuronID].weights[inputID] * inputs[inputID];
                }

                // Odejmowanie wartości wyrównującej (offset).
                value -= layers[layerID].neurons[neuronID].bias;

                // Ustawienie wartości wyjściowej neuronu poprzez wykonanie funkcji aktywacji.
                layers[layerID].neurons[neuronID].output = (layerID == numberOfHiddenLayers) ? ActivationFun(value) : ActivationFunO(value);

                // Dodanie wartości wyjściowej do listy.
                outputs.Add(layers[layerID].neurons[neuronID].output);
            }
        }

        // Zwrot wartości wyjściowych.
        return outputs;
    }

    // Funkcja uaktualniająca wagi sieci neuronowej.
    private void UpdateWeights(List<double> outputs, List<double> desiredOutputs)
    {
        // Wartość błędu.
        double error = 0;

        // Pętla poruszająca się po warstwach sieci neuronowej, zaczynając od końca (Backpropagation).
        for (int layerID = numberOfHiddenLayers; layerID >= 0; layerID--)
        {
            // Pętla poruszająca się po neuronach warstwy sieci neuronowej.
            for (int neuronID = 0; neuronID < layers[layerID].numberOfNeurons; neuronID++)
            {
                // Sprawdzenie czy dana warstwa jest warstwą wyjściową.
                if (layerID == numberOfHiddenLayers)
                {
                    // Obliczanie wartości błędu.
                    error = desiredOutputs[neuronID] - outputs[neuronID];

                    // Obliczanie gradientu błędu zgodnie z regułą delty.https://en.wikipedia.org/wiki/Delta_rule
                    layers[layerID].neurons[neuronID].errorGradient = outputs[neuronID] * (1 - outputs[neuronID]) * error;
                }
                else
                {
                    // Zmienna przechowująca sumę gradientów błędów z poprzedzającej warstwy.
                    double errorGradientSum = 0;

                    // Obliczanie sumy gradientów błędów przemnożonych przez odpowiadające im wagi, poprzez przejście po neuronach następnej warstwy.
                    for (int neuID = 0; neuID < layers[layerID + 1].numberOfNeurons; neuID++)
                        errorGradientSum += layers[layerID + 1].neurons[neuID].errorGradient * layers[layerID + 1].neurons[neuID].weights[neuronID];

                    // Obliczanie gradientu błędu dla danego neuronu. Zgodnie z regułą delty leczy wykorzystując błąd gradientu następnej warstwy.
                    layers[layerID].neurons[neuronID].errorGradient = layers[layerID].neurons[neuronID].output * (1 - layers[layerID].neurons[neuronID].output) * errorGradientSum;
                }

                // Pętla poruszająca się po wejściach danego neuronu.
                for (int inputID = 0; inputID < layers[layerID].neurons[neuronID].numberOfInputs; inputID++)
                {
                    // Sprawdzenie czy dana warstwa jest warstwą wyjściową.
                    if (layerID == numberOfHiddenLayers)
                    {
                        // Obliczanie błędu.
                        error = desiredOutputs[neuronID] - outputs[neuronID];

                        // Uaktualnienie wagi dla danego wejścia neuronu.
                        layers[layerID].neurons[neuronID].weights[inputID] += learningSpeedRate * layers[layerID].neurons[neuronID].inputs[inputID] * error;
                    }
                    else
                        // Uaktualnienie wagi dla danego wejścia neuronu.
                        layers[layerID].neurons[neuronID].weights[inputID] += learningSpeedRate * layers[layerID].neurons[neuronID].inputs[inputID] * layers[layerID].neurons[neuronID].errorGradient;
                }
                // Uaktualnienie wartości wyrównującej (offset).
                layers[layerID].neurons[neuronID].bias -= learningSpeedRate * layers[layerID].neurons[neuronID].errorGradient;
            }
        }
    }

    // Funkcja wypisująca wagi sieci neuronowej w postaci stringu.
    public string PrintWeights()
    {
        // Tworzenie pustego stringu
        string weightStr = "";
        // Pętla poruszająca się po warstwach sieci neuronowej.
        foreach (Layer l in layers)
        {
            // Pętla poruszająca się po neuronach warstwy.
            foreach (Neuron n in l.neurons)
            {
                // Pętla poruszająca się po wagach wejść neuronu.
                foreach (double w in n.weights)
                {
                    // Dopisywanie wag.
                    weightStr += w + Separator.dataSeparator;
                }
                // Dopisywanie offsetu danego neuronu.
                weightStr += n.bias + Separator.dataSeparator;
            }
        }
        // Zwrot całości.
        return weightStr;
    }

    // Funkcja wczytywująca wagi sieci neuronowej z postaci stringu.
    public void LoadWeights(string weightStr)
    {
        // Sprawdzenie czy dane wejściowe nie są puste.
        if (weightStr == "") return;
        // Podział całości na części oddzielone separatorem.
        string[] weightValues = weightStr.Split(Separator.dataSeparator);
        // Indeks danych.
        int w = 0;

        // Pętla poruszająca się po warstwach sieci neuronowej.
        foreach (Layer l in layers)
        {
            // Pętla poruszająca się po neuronach warstwy.
            foreach (Neuron n in l.neurons)
            {
                // Pętla poruszająca się po wagach wejść neuronu.
                for (int i = 0; i < n.weights.Count; i++)
                {
                    // Zapis wag w sieci neuronowej.
                    n.weights[i] = System.Convert.ToDouble(weightValues[w]);
                    // Inkrementacja indeksu danych wejściowych.
                    w++;
                }
                // Dodanie offsetu dla neuronu.
                n.bias = System.Convert.ToDouble(weightValues[w]);
                w++;
            }
        }
    }

    // Funkcja aktywacji.
    private double ActivationFun(double value)
    {
        // Wywołanie odpowiedniej funkcji aktywacji.
        return ActivationFunction.TanH(value);
    }

    // Funkcja aktywacji warstwy wyjściowej.
    private double ActivationFunO(double value)
    {
        // Wywołanie odpowiedniej funkcji aktywacji.
        return ActivationFunction.Sigmoid(value);
    }
}

