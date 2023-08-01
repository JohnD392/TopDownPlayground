using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Pulse
{ 
    public double Value { get; set; }
}

class Dendrite
{
    public Pulse InputPulse { get; set; }
    public double SynapticWeight { get; set; }
    public bool Learnable { get; set; } = true;
}

class Neuron
{
    public List<Dendrite> Dendrites { get; set; }
    public Pulse OutputPulse { get; set; }
    private double Weight;

    public Neuron()
    {
        Dendrites = new List<Dendrite>();
        OutputPulse = new Pulse();
    }

    public void Fire()
    {
        OutputPulse.Value = Sum();
        OutputPulse.Value = Activation(OutputPulse.Value);
    }

    public void Compute(double learningRate, double delta) {
        Weight += learningRate * delta;
        foreach(var terminal in Dendrites) {
            terminal.SynapticWeight = Weight;
        }
    }

    private double Sum() {
        double computeValue = 0.0f;
        foreach(var d in Dendrites) {
            computeValue += d.InputPulse.Value * d.SynapticWeight;
        }
        return computeValue;
    }

    private double Activation(double input)
    {
        double threshold = 1f;
        return input >= threshold ? 0 : threshold;
    }
}

class NeuralLayer {
    public List<Neuron> Neurons { get; set; }
    public string Name { get; set; }
    public double Weight { get; set; }
    public NeuralLayer(int count, double initialWeight, string name = "") {
        Neurons = new List<Neuron>();
        for (int i = 0; i < count; i++) { Neurons.Add(new Neuron()); }
        Weight = initialWeight;
        Name = name;
    }

    public void Compute(double learningRate, double delta) {
        foreach (var neuron in Neurons) { neuron.Compute(learningRate, delta);
    }

    void Log() {
            Debug.Log(Name + "Weight: " + Weight);
        }
    }
}

public class NeuralNetwork : MonoBehaviour {
    private int[] layers;
    private float[][] neurons;
    private float[][] biases;
    private float[][][] weights;
    private int[] activations;

    //genetic
    public float fitness = 0; //fitness

    //backprop
    public float learningRate = 0.01f; //learning rate
    public float cost = 0;

    private float[][] deltaBiases;//biasses
    private float[][][] deltaWeights;//weights
    private int deltaCount;

    public NeuralNetwork(int[] layers, string[] layerActivations)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }
        activations = new int[layers.Length - 1];
        for (int i = 0; i < layers.Length - 1; i++)
        {
            string action = layerActivations[i];
            switch (action)
            {
                case "sigmoid":
                    activations[i] = 0;
                    break;
                case "relu":
                    activations[i] = 1;
                    break;
                case "leakyrelu":
                    activations[i] = 2;
                    break;
                default:
                    activations[i] = 3;
                    break;
            }
        }
        InitNeurons();
        InitBiases();
        InitWeights();
    }


    private void InitNeurons()//create empty storage array for the neurons in the network.
    {
        List<float[]> neuronsList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]);
        }
        neurons = neuronsList.ToArray();
    }

    private void InitBiases()//initializes random array for the biases being held within the network.
    {


        List<float[]> biasList = new List<float[]>();
        for (int i = 1; i < layers.Length; i++)
        {
            float[] bias = new float[layers[i]];
            for (int j = 0; j < layers[i]; j++)
            {
                bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);
            }
            biasList.Add(bias);
        }
        biases = biasList.ToArray();
    }

    private void InitWeights()//initializes random array for the weights being held in the network.
    {
        List<float[][]> weightsList = new List<float[][]>();
        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInPreviousLayer = layers[i - 1];
            for (int j = 0; j < layers[i]; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }
                layerWeightsList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightsList.ToArray());
        }
        weights = weightsList.ToArray();
    }

    public float[] FeedForward(float[] inputs)//feed forward, inputs >==> outputs.
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }
        for (int i = 1; i < layers.Length; i++)
        {
            int layer = i - 1;
            for (int j = 0; j < layers[i]; j++)
            {
                float value = 0f;
                for (int k = 0; k < layers[i - 1]; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                neurons[i][j] = activate(value + biases[i - 1][j], layer);
            }
        }
        return neurons[layers.Length - 1];
    }
    //Backpropagation implemtation down until mutation.
    public float activate(float value, int layer)//all activation functions
    {
        switch (activations[layer])
        {
            case 0:
                return sigmoid(value);
            case 1:
                return relu(value);
            case 2:
                return leakyrelu(value);
            default:
                return relu(value);
        }
    }
    public float activateDer(float value, int layer)//all activation function derivatives
    {
        switch (activations[layer])
        {
            case 0:
                return sigmoidDer(value);
            case 1:
                return reluDer(value);
            case 2:
                return leakyreluDer(value);
            default:
                return reluDer(value);
        }
    }

    public float sigmoid(float x)//activation functions and their corrosponding derivatives
    {
        float k = (float)Mathf.Exp(x);
        return k / (1.0f + k);
    }

    public float relu(float x)
    {
        return (0 >= x) ? 0 : x;
    }
    public float leakyrelu(float x)
    {
        return (0 >= x) ? 0.01f * x : x;
    }
    public float sigmoidDer(float x)
    {
        return x * (1 - x);
    }
    public float tanhDer(float x)
    {
        return 1 - (x * x);
    }
    public float reluDer(float x)
    {
        return (0 >= x) ? 0 : 1;
    }
    public float leakyreluDer(float x)
    {
        return (0 >= x) ? 0.01f : 1;
    }

    public void BackPropagate(float[] inputs, float[] expected)//backpropogation;
    {
        float[] output = FeedForward(inputs);//runs feed forward to ensure neurons are populated correctly

        cost = 0;
        for (int i = 0; i < output.Length; i++) cost += (float)Mathf.Pow(output[i] - expected[i], 2);//calculated cost of network
        cost = cost / 2;//this value is not used in calculions, rather used to identify the performance of the network

        float[][] gamma;


        List<float[]> gammaList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            gammaList.Add(new float[layers[i]]);
        }
        gamma = gammaList.ToArray();//gamma initialization

        int layer = layers.Length - 2;
        for (int i = 0; i < output.Length; i++) gamma[layers.Length - 1][i] = (output[i] - expected[i]) * activateDer(output[i], layer);//Gamma calculation
        for (int i = 0; i < layers[layers.Length - 1]; i++)//calculates the w' and b' for the last layer in the network
        {
            biases[layers.Length - 2][i] -= gamma[layers.Length - 1][i] * learningRate;
            for (int j = 0; j < layers[layers.Length - 2]; j++)
            {

                weights[layers.Length - 2][i][j] -= gamma[layers.Length - 1][i] * neurons[layers.Length - 2][j] * learningRate;//*learning 
            }
        }

        for (int i = layers.Length - 2; i > 0; i--)//runs on all hidden layers
        {
            layer = i - 1;
            for (int j = 0; j < layers[i]; j++)//outputs
            {
                gamma[i][j] = 0;
                for (int k = 0; k < gamma[i + 1].Length; k++)
                {
                    gamma[i][j] += gamma[i + 1][k] * weights[i][k][j];
                }
                gamma[i][j] *= activateDer(neurons[i][j], layer);//calculate gamma
            }
            for (int j = 0; j < layers[i]; j++)//itterate over outputs of layer
            {
                biases[i - 1][j] -= gamma[i][j] * learningRate;//modify biases of network
                for (int k = 0; k < layers[i - 1]; k++)//itterate over inputs to layer
                {
                    weights[i - 1][j][k] -= gamma[i][j] * neurons[i - 1][k] * learningRate;//modify weights of network
                }
            }
        }
    }

    //Genetic implementations down onwards until save.

    public void Mutate(int high, float val)//used as a simple mutation function for any genetic implementations.
    {
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                biases[i][j] = (UnityEngine.Random.Range(0f, high) <= 2) ? biases[i][j] += UnityEngine.Random.Range(-val, val) : biases[i][j];
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = (UnityEngine.Random.Range(0f, high) <= 2) ? weights[i][j][k] += UnityEngine.Random.Range(-val, val) : weights[i][j][k];
                }
            }
        }
    }

    public int CompareTo(NeuralNetwork other) //Comparing For Genetic implementations. Used for sorting based on the fitness of the network
    {
        if (other == null) return 1;

        if (fitness > other.fitness)
            return 1;
        else if (fitness < other.fitness)
            return -1;
        else
            return 0;
    }

    public NeuralNetwork copy(NeuralNetwork nn) //For creatinga deep copy, to ensure arrays are serialzed.
    {
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                nn.biases[i][j] = biases[i][j];
            }
        }
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    nn.weights[i][j][k] = weights[i][j][k];
                }
            }
        }
        return nn;
    }
}