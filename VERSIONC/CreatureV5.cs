using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {

    //Game object interaction
    public SpriteRenderer sRender;
    public Rigidbody2D rBody2D;

    //Creature Variables
    public float[] creatureGenome = new float[] { 1, 0, 0, 0, 0 };
    //The genome of the creature. 
    //Stored in the creature accessed by a genetic algorithm and the neural network. 
    //As well as some other phynotypes
    public float creatureHealth = 1;
    //Increases with eating, decreases with movement and existance. Despawns when it hits 0
    public float forwardSpeed = 0;
    //Speed at which it goes forward. Output from Neural Network
    public float turnSpeed;
    //Speed at which it turns. Output from Neural Network.
    public float recurrentNN1 = 0;
    public float recurrentNN2 = 0;
    //Temporary memory for creature. In essence this is both an input and output of the neural network.

    //time
    public float timealive = 0;
    
    //Sensors WIP
    public float Smell = 0;
    public float Touch = 0;
    public float WIP = 0;


    //This is the neural network
    void RunNeuralNet()
    {
        //Variables
        int WeightIndex = 3; //to increment each time a weight is used. Starts at three, because the first three are colour values
        float NodeSum = 0;
        float[] Outputs = new float[4]; //Output array. currently only 4 in size
        float[] Inputs = new float[] { //Input into the neural network
            creatureHealth,
            recurrentNN1,
            recurrentNN2,
            Smell,
            Touch,
            WIP
        };
        float[] Hidden1 = new float[5]; //The first hidden network
        float[] Hidden2 = new float[5]; //The secound hidden network

        //Go through the neural network
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

        //Set output variables
        forwardSpeed = Outputs[0];
        turnSpeed = Outputs[1];
        recurrentNN1 = Outputs[2];
        recurrentNN2 = Outputs[3];

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
        sRender = gameObject.GetComponent<SpriteRenderer>();
        rBody2D = gameObject.GetComponent<Rigidbody2D>();
        sRender.color = new Color(creatureGenome[0], creatureGenome[1], creatureGenome[2]);
	}

    public void FindClosestFood()
    {
        GameObject[] foods;
        foods = GameObject.FindGameObjectsWithTag("Food");
        GameObject closestFood = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject food in foods)
        {
            Vector3 diffrence = food.transform.position - currentPosition;
            float foodDistance = diffrence.sqrMagnitude;
            if (foodDistance < closestDistance)
            {
                closestFood = food;
                closestDistance = foodDistance;
            }
        }
        //if touching eat
        if (gameObject.GetComponent<CircleCollider2D>().IsTouching(closestFood.GetComponent<CircleCollider2D>()))
        {
            closestFood.SendMessage("Eat");
            creatureHealth += 1;
        }
        
        //set smell data
        Smell = (1f / closestDistance);
    }

    // Update is called once per frame
    void Update () {
        //alive time update
        timealive += 0.01f;
        //Run sensors and eat
        creatureHealth -= 0.0007f;
        FindClosestFood();
        WIP = 0;

        //Check if dead
        if (creatureHealth <= 0)
        {
            Destroy(gameObject);
        }

        //Run neural network
        //RunNeuralNet();

        //Put outputs of NN into the physics engien.
        transform.eulerAngles += new Vector3 ( 0, 0, turnSpeed * 7f);
        rBody2D.velocity += new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.PI / 180f) * forwardSpeed, Mathf.Sin(transform.eulerAngles.z * Mathf.PI / 180f) * forwardSpeed);
	}

    public void GenesInput(float[] genes)
    {
        creatureGenome = genes;
    }
}
