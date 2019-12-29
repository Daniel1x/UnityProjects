using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;

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
    public float learningSpeedRate;

    // Lista warstw sieci neuronowej.
    List<Layer> layers = new List<Layer>();

    // Konstruktor sieci neuronowej.
    public ArtificialNeuralNetwork(int numOfInputs, int numOfOutputs, int numOfHiddenLayers, int numOfNeuronsPerHiddenLayer, float valOfLearningSpeedRate)
    {
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
    public List<float> Train(List<float> inputValues, List<float> desiredOutputs)
    {
        // Utworzenie listy na dane.
        List<float> outputs = new List<float>();

        // Obliczanie wyjść na podstawie danych.
        outputs = CalculateOutputs(inputValues, desiredOutputs);

        // Uaktualnianie wag sieci neuronowej.
        UpdateWeights(outputs, desiredOutputs);

        // Zwracanie wyników
        return outputs;
    }

    // Funkcja obliczająca wyjścia, nie zmieniająca wag sieci neuronowej.
    public List<float> CalculateOutputs(List<float> inputValues, List<float> desiredOutputs = null)
    {
        // Lista danych wejściowych.
        List<float> inputs = new List<float>();

        // Lista danych wyjściowych.
        List<float> outputs = new List<float>();

        // Sprawdzenie czy liczba danych wejściowych odpowiada liczbie wejść sieci neuronowej.
        if (inputValues.Count != numberOfInputs)
        {
            Debug.Log("Error: Number of inputs must be equal to " + numberOfInputs);
            return outputs;
        }

        // Przypisanie wartości wejściowych.
        inputs = new List<float>(inputValues);

        // Pętla poruszająca się po warstwach sieci neuronowej.
        for (int layerID = 0; layerID < numberOfHiddenLayers + 1; layerID++)
        {
            // Sprawdzenie czy dana warstwa jest warstwą wejściową, jeżeli nie to danymi wejściowymi stają się dane wyjściowe z poprzedniej warstwy.
            if (layerID != 0)
                inputs = new List<float>(outputs);

            // Czyszczenie niepotrzebnych pozostałości w danych wyjściowych, przygotowanie do kolejnego nadpisania.
            outputs.Clear();

            // Pętla poruszająca się po neuronach warstwy sieci neuronowej.
            for (int neuronID = 0; neuronID < layers[layerID].numberOfNeurons; neuronID++)
            {
                // Zmienna przechowująca sumę wejść pomnożonych przez odpowiadające im wagi.
                float value = 0;

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
    private void UpdateWeights(List<float> outputs, List<float> desiredOutputs)
    {
        // Zmienna przechowująca wartość aktualnego błędu.
        float error = 0;
        // Pętla poruszająca się po warstwach sieci neuronowej, zaczynając od końca.
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
                    // Obliczanie gradientu błędu zgodnie z regułą delty.
                    layers[layerID].neurons[neuronID].errorGradient = outputs[neuronID] * (1 - outputs[neuronID]) * error;
                }
                else
                {
                    // Zmienna przechowująca sumę gradientów błędów z poprzedzającej warstwy.
                    float errorGradientSum = 0;
                    // Obliczanie sumy gradientów błędów przemnożonych przez odpowiadające im wagi, poprzez przejście po neuronach następnej warstwy.
                    for (int neuID = 0; neuID < layers[layerID + 1].numberOfNeurons; neuID++)
                        errorGradientSum += layers[layerID + 1].neurons[neuID].errorGradient * layers[layerID + 1].neurons[neuID].weights[neuronID];
                    // Obliczanie gradientu błędu dla danego neuronu. Zgodnie z regułą delty lecz wykorzystując błąd gradientu następnej warstwy.
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
                foreach (float w in n.weights)
                {
                    // Dopisywanie wag.
                    weightStr += w + Separator.dataSeparatorString;
                }
                // Dopisywanie offsetu danego neuronu.
                weightStr += n.bias + Separator.dataSeparatorString;
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
        string[] weightValues = weightStr.Split(Separator.dataSeparatorChar);
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
                    n.weights[i] = float.Parse(weightValues[w]);
                    // Inkrementacja indeksu danych wejściowych.
                    w++;
                }
                // Dodanie offsetu dla neuronu.
                n.bias = float.Parse(weightValues[w]);
                w++;
            }
        }
    }

    // Zapisywanie wag sieci neuronowej do pliku.
    public void SaveWeightsToFile(string name = "")
    {
        string path = Application.dataPath + "/weights" + name + ".txt";
        StreamWriter wf = File.CreateText(path);
        wf.WriteLine(PrintWeights());
        wf.Close();
    }

    // Wczytywanie wag sieci neuronowej z pliku.
    public void LoadWeightsFromFile(string name = "")
    {
        string path = Application.dataPath + "/weights" + name + ".txt";
        StreamReader wf = File.OpenText(path);

        if (File.Exists(path))
        {
            Debug.Log("Weights loaded from " + path);
            string line = wf.ReadLine();
            LoadWeights(line);
        }
    }

    // Funkcja aktywacji.
    private float ActivationFun(float value)
    {
        // Wywołanie odpowiedniej funkcji aktywacji.
        return ActivationFunction.TanH(value);
    }

    // Funkcja aktywacji warstwy wyjściowej.
    private float ActivationFunO(float value)
    {
        // Wywołanie odpowiedniej funkcji aktywacji.
        return ActivationFunction.Sigmoid(value);
    }
}

