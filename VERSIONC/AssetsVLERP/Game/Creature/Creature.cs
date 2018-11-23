using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {

    //Game object interaction
    public SpriteRenderer sRender;
    public Rigidbody2D rBody2D;

    //Creature Variables
    public float[] creatureGenome = new float[111];
    //The genome of the creature. 
    //Stored in the creature accessed by a genetic algorithm and the neural network. 
    //As well as some other phynotypes
    public float creatureHealth = 1;
    //Increases with eating, decreases with movement and existance. Despawns when it hits 0
    public float forwardSpeed = 0;
    //Speed at which it goes forward. Output from Neural Network
    public float turnSpeed;
    //Speed at which it turns. Output from Neural Network.
    public bool willingToBreed = false;
    public float recurrentNN1 = 0;
    public float recurrentNN2 = 0;
    //Temporary memory for creature. In essence this is both an input and output of the neural network.

    //time
    public float timealive = 0;
    
    //Sensors WIP
    public float Smell = 0;
    public float Touch = 0;
    public float viewForward = 0;
    public float viewLeft1 = 0;
    public float viewLeft2 = 0;
    public float viewRight1 = 0;
    public float viewRight2 = 0;
    public float WIP = 0;


    //This is the neural network
    void RunNeuralNet()
    {
        //Variables
        int WeightIndex = 3; //to increment each time a weight is used. Starts at three, because the first three are colour values
        float NodeSum = 0;
        float[] Outputs = new float[5]; //Output array. Updating the length of this will update the length of the output
        float[] Inputs = new float[] { //Input into the neural network
            creatureHealth,
            recurrentNN1,
            recurrentNN2,
            Smell,
            Touch,
            viewForward,
            viewLeft1,
            viewLeft2,
            viewRight1,
            viewRight2,
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

        //Debug.Log(WeightIndex); //Debug to show length of genes

        //Set output variables
        forwardSpeed = Outputs[0];
        turnSpeed = Outputs[1];
        recurrentNN1 = Outputs[2];
        recurrentNN2 = Outputs[3];
        willingToBreed = (Mathf.Round(Outputs[4]) > 0.5);

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

    public void Look()
    {

        //Use physics raycast to see if there are any objects in front of the creature (View)

        Vector2 anglevector = (Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * Vector2.right);
        RaycastHit2D viewForward = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y) + //Position of raycast
            anglevector * 0.5f, //Add direction vector, to avoid touching the collider of the sprite.
            anglevector, //Direction of raycast in a vector (normalized direction in this case)
            5 //Maximum distance
            );

        anglevector = (Quaternion.AngleAxis(transform.eulerAngles.z + 10, Vector3.forward) * Vector2.right);
        RaycastHit2D viewLeft1 = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y) + //Position of raycast
            anglevector * 0.5f, //Add direction vector, to avoid touching the collider of the sprite.
            anglevector, //Direction of raycast in a vector (normalized direction in this case)
            4 //Maximum distance
        );

        anglevector = (Quaternion.AngleAxis(transform.eulerAngles.z + 20, Vector3.forward) * Vector2.right);
        RaycastHit2D viewLeft2 = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y) + //Position of raycast
            anglevector * 0.5f, //Add direction vector, to avoid touching the collider of the sprite.
            anglevector, //Direction of raycast in a vector (normalized direction in this case)
            3 //Maximum distance
        );

        anglevector = (Quaternion.AngleAxis(transform.eulerAngles.z - 10, Vector3.forward) * Vector2.right);
        RaycastHit2D viewRight1 = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y) + //Position of raycast
            anglevector * 0.5f, //Add direction vector, to avoid touching the collider of the sprite.
            anglevector, //Direction of raycast in a vector (normalized direction in this case)
            4 //Maximum distance
        );

        anglevector = (Quaternion.AngleAxis(transform.eulerAngles.z - 20, Vector3.forward) * Vector2.right);
        RaycastHit2D viewRight2 = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y) + //Position of raycast
            anglevector * 0.5f, //Add direction vector, to avoid touching the collider of the sprite.
            anglevector, //Direction of raycast in a vector (normalized direction in this case)
            3 //Maximum distance
        );



        //Asign values to the view data, from the distance value in the raycast
        if (viewForward.distance == 0)
        {
            this.viewForward = 0; //Distance is zero if there is no object
        }
        else
        {
            this.viewForward = 1 - (viewForward.distance / 5); //return a value between 0-1 for distance to face. 1 is closest.
        }

        if (viewLeft1.distance == 0)
        {
            this.viewLeft1 = 0; //Distance is zero if there is no object
        }
        else
        {
            this.viewLeft1 = 1 - (viewLeft1.distance / 5); //return a value between 0-1 for distance to face. 1 is closest.
        }

        if (viewLeft2.distance == 0)
        {
            this.viewLeft2 = 0; //Distance is zero if there is no object
        }
        else
        {
            this.viewLeft2 = 1 - (viewLeft2.distance / 5); //return a value between 0-1 for distance to face. 1 is closest.
        }

        if (viewRight1.distance == 0)
        {
            this.viewRight1 = 0; //Distance is zero if there is no object
        }
        else
        {
            this.viewRight1 = 1 - (viewRight1.distance / 5); //return a value between 0-1 for distance to face. 1 is closest.
        }

        if (viewRight2.distance == 0)
        {
            this.viewRight2 = 0; //Distance is zero if there is no object
        }
        else
        {
            this.viewRight2 = 1 - (viewRight2.distance / 5); //return a value between 0-1 for distance to face. 1 is closest.
        }
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
        if (foods.Length != 0 && gameObject.GetComponent<CircleCollider2D>().IsTouching(closestFood.GetComponent<CircleCollider2D>()))
        {
            closestFood.SendMessage("Eat");
            creatureHealth += 1;
        }
        
        //set smell data
        Smell = (1f / closestDistance);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Touch = 1;
        if (willingToBreed)
        {
            if (collision.otherCollider.gameObject.GetComponent<Creature>().willingToBreed)
            {
                BreedingControl.AddCreatureToBreedingList(this);
                creatureHealth -= 0.55f; //New creatures spawn with 1HP, 0.1 growing cost
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Touch = 0;
    }

    // Update is called once per frame
    void Update () {
        //alive time update
        timealive += 0.01f;
        //Decrease health for living
        creatureHealth -= 0.0007f;

        FindClosestFood();
        //gets food info
        //eats if touching food
        //inputs smell sensor

        Look();
        //Throws 5 raycasts and puts the distance output into the view float variables.

        WIP = 0;
        //Here to remind me that there are more sensors that need to be done.

        //Check if dead
        if (creatureHealth <= 0)
        {
            GameObject.FindGameObjectsWithTag("Stat")[0].BroadcastMessage("CreatureDied", timealive);
            //Send the data to the stat gameobject
            Destroy(gameObject);
        }

        //Run neural network
        RunNeuralNet();

        //Put outputs of NN into the physics engien.
        transform.eulerAngles += new Vector3 ( 0, 0, turnSpeed * 7f);
        rBody2D.velocity += new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.PI / 180f) * forwardSpeed, Mathf.Sin(transform.eulerAngles.z * Mathf.PI / 180f) * forwardSpeed);

        //The energy cost of movement
        creatureHealth -= Mathf.Abs(turnSpeed / 10000);
        creatureHealth -= Mathf.Abs(forwardSpeed / 10000);
    }

    public void GenesInput(float[] genes)
    {
        creatureGenome = genes;
    }
}
