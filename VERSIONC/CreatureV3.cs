using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {

    //Game object interaction
    public SpriteRenderer sReader;
    public Rigidbody2D rBody2D;

    //Creature Variables
    public float[] creatureGenome;
    //The genome of the creature. 
    //Stored in the creature accessed by a genetic algorithm and the neural network. 
    //As well as some other phynotypes
    public float creatureHealth = 0;
    //Increases with eating, decreases with movement and existance. Despawns when it hits 0
    public float forwardSpeed = 0;
    //Speed at which it goes forward. Output from Neural Network
    public float turnSpeed;
    //Speed at which it turns. Output from Neural Network.
    public float recursiveNN1 = 0;
    public float recursiveNN2 = 0;
    //Temporary memory for creature. In essence this is both an input and output of the neural network.

    //Sensors WIP
    public float Smell = 0;
    public float Touch = 0;
    public float WIP = 0;

    void RunNeuralNet()
    {
        //Variables
        int WeightIndex = 0; //to increment each time a weight is used. 
        float NodeSum = 0;
        float[] Outputs = new float[4]; //Output array. currently only 4 in size
        float[] Inputs = new float[] { //Input into the neural network
            creatureHealth,
            recursiveNN1,
            recursiveNN2,
            Smell,
            Touch,
            WIP
        };
        float[] Hidden1 = new float[5]; //The first hidden network
        float[] Hidden2 = new float[5]; //The secound hidden network
        for (int i = 0; i < Hidden1.Length; i++) //Iterate through all first hidden nodes
        {
            NodeSum = 0;
            foreach (float input in Inputs)
            {
                NodeSum += input * creatureGenome[WeightIndex]; //Add input*weight to node sum
                WeightIndex++;
            }
            Hidden1[i] = ActivationFunction(NodeSum); //Use activation function to set new node value
        }
        for (int i = 0; i < Hidden2.Length; i++) //Same thing for secound hidden layer
        {
            NodeSum = 0;
            foreach (float hiddennode in Hidden1)
            {
                NodeSum += hiddennode * creatureGenome[WeightIndex];
                WeightIndex++;
            }
            Hidden2[i] = ActivationFunction(NodeSum);
        }
        for (int i = 0; i < Outputs.Length; i++) //Same thing for secound hidden layer
        {
            NodeSum = 0;
            foreach (float hiddennode in Hidden2)
            {
                NodeSum += hiddennode * creatureGenome[WeightIndex];
                WeightIndex++;
            }
            Outputs[i] = ActivationFunction(NodeSum);
        }
        forwardSpeed = Outputs[0];
        turnSpeed = Outputs[1];
        recursiveNN1 = Outputs[2];
        recursiveNN2 = Outputs[3];

    }

    float ActivationFunction(float i)
    {
        //Activation function is used to set the value of the nodes in the neural network.
        //get user setting for activation function
        switch (0) //user setting
        {
            case 0:
                return (Mathf.Exp(i) - Mathf.Exp(-i)) / (Mathf.Exp(i) + Mathf.Exp(-i)); //Hyperbolic Tan
                //No break required because return quits the switch.
        }
    }
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //Run sensors
        Smell = 0;
        WIP = 0;
        //Run neural network
        //RunNeuralNet();
        //Put outputs of NN into the physics engien.
        transform.eulerAngles += new Vector3 ( 0, 0, turnSpeed * 7f);
        rBody2D.velocity += new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.PI / 180f) * forwardSpeed, Mathf.Sin(transform.eulerAngles.z * Mathf.PI / 180f) * forwardSpeed);
	}
}
