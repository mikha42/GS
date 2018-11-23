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
    public float creatureHealth;
    //Increases with eating, decreases with movement and existance. Despawns when it hits 0
    public float forwardSpeed;
    //Speed at which it goes forward. Output from Neural Network
    public float turnSpeed;
    //Speed at which it turns. Output from Neural Network.
    public float recursiveNN1;
    public float recursiveNN2;
    //Temporary memory for creature. In essence this is both an input and output of the neural network.

    //Sensors WIP
    public float Smell;
    public float WIP;

    void RunNeuralNet()
    {

    }
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //Run sensors
        //Run neural network
        //Put outputs of NN into the physics engien.
        transform.eulerAngles += new Vector3 ( 0, 0, turnSpeed * 7f);
        rBody2D.velocity += new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.PI / 180f) * forwardSpeed, Mathf.Sin(transform.eulerAngles.z * Mathf.PI / 180f) * forwardSpeed);
	}
}
