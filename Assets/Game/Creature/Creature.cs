using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {

    //Game object interaction
    public SpriteRenderer sRender;
    public Rigidbody2D rBody2D;
    public SpriteRenderer colourBody;
    public SpriteRenderer colourEye;
    public GameObject noiseShow;

    public int activation = 0;


    //Creature Variables
    public float[] creatureGenome;
    //The genome of the creature. 
    //Stored in the creature accessed by a genetic algorithm and the neural network. 
    //As well as some other phynotypes
    public float creatureHealth = 1;
    //Increases with eating, decreases with movement and existance. Despawns when it hits 0
    public float forwardSpeed = 0;
    //Speed at which it goes forward. Output from Neural Network
    public float turnSpeed = 0;
    //Speed at which it turns. Output from Neural Network.
    public float talk = 0;
    //Creatures making noise
    public bool willingToBreed = false;
    public float[] recurrentNN = { 0, 0, 0, 0 };
    public float willingnessToBreed = 0; //This isnt used for the creature, its used for debuging and for the user to see.
    //Temporary memory for creature. In essence this is both an input and output of the neural network.

    //time
    public float timealive = 0;

    public int steps = 0;

    //Sensors WIP
    public float Smell = 0;
    public float Touch = 0;
    public float[] Sight;
    public float WIP = 0;
    public float listen = 0;
    public float hear = 0;

    public float viewDistance = 15;
    public int sideSightLines = 2;

    public bool getweightcount = false;





    //NEURAL NET INBETWEENS
    //Better performance at a cost of memory usage?
    int WeightIndex;
    float NodeSum;
    float[] Outputs;
    float[] PreInputs;
    float[] Inputs;
    float[] Hidden1;
    float[] Hidden2;

    //This is the neural network
    void RunNeuralNet()
    {
        //Variables
        WeightIndex = 3; //to increment each time a weight is used. Starts at three, because the first three are colour values
        NodeSum = 0;
        Outputs = new float[8]; //Output array. Updating the length of this will update the length of the output
        PreInputs = new float[] { //Input into the neural network
            creatureHealth,
            hear,
            Smell,
            Touch,
            recurrentNN[0],
            recurrentNN[1],
            recurrentNN[2],
            recurrentNN[3]
        };
        Inputs = new float[PreInputs.Length + Sight.Length];
        PreInputs.CopyTo(Inputs, 0);
        Sight.CopyTo(Inputs, PreInputs.Length);
        Hidden1 = new float[8]; //The first hidden network
        Hidden2 = new float[8]; //The secound hidden network
        
        if (Inputs.Length * Hidden1.Length + Hidden1.Length * Hidden2.Length + Hidden2.Length * Outputs.Length + 3 != creatureGenome.Length)
        {
            Debug.Log("Invalid Genome Length");
            stat.updateGeneLength(Inputs.Length * Hidden1.Length + Hidden1.Length * Hidden2.Length + Hidden2.Length * Outputs.Length + 3);
            Die();
        }

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
        recurrentNN = new float[]{Outputs[2] , Outputs[3], Outputs[4], Outputs[5]};
        willingToBreed = (Outputs[6] > 0);
        willingnessToBreed = Outputs[6];
        talk = Outputs[7];
        //willingToBreed = true; //FOR TESTING PURPOSES
        //Show the user if the creature wants to breed
        colourEye.color = willingToBreed ? new Color(1, 0, 0) : new Color(1, 1, 1);

    }

    float ActivationFunction(float i)
    {
        //Activation function is used to set the value of the nodes in the neural network.
        //get user setting for activation function
        switch (activation) //user setting
        {
            case 0: //hyperbolic tan
                return (float)Math.Tanh(i);
            //No break required because return quits the switch.
            case 1:
                return Mathf.Sin(i); // Sinosoidal
            case 2:
                return Mathf.Atan(i); // Arc Tan 
            case 3:
                return i; // Linear
            case 4:
                return Mathf.Exp(-Mathf.Pow(i, 2)); //Gausean
            case 5:
                return (1f / (1f + Mathf.Exp(-i))); //Logistic
            case 6:
                return (i >= 0) ? 1 : 0; //Binary
            case 7:
                return (i == 0) ? 0f : ((i > 0) ? 1 : -1); //Sign

        }
        return 0;
    }
    
	// Use this for initialization
	void Start () {
        //Debug.Log(creatureHealth);
        Sight = new float[1 + 2 * sideSightLines];
        for (int i = 0; i < Sight.Length; i++)
            Sight[i] = 0;
        sRender = gameObject.GetComponent<SpriteRenderer>();
        rBody2D = gameObject.GetComponent<Rigidbody2D>();
        colourBody.color = new Color(creatureGenome[0], creatureGenome[1], creatureGenome[2]);
    }

    public void Look()
    {

        //Use physics raycast to see if there are any objects in front of the creature (View)
        float anglebetween = 5f;
        Vector2 anglevector;
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        float raycast;
        float angle = - 2.5f * sideSightLines * 2 + transform.rotation.eulerAngles.z;
        anglebetween *= (Mathf.PI) / 180;
        angle *= (Mathf.PI) / 180;

        float mod = 0.4f; //Min mod
        float modDelta = (1 - mod) / sideSightLines;

        for (int i = 0; i < 1 + 2 * sideSightLines; i++)
        {
            anglevector = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            angle += anglebetween;
            //Debug.DrawLine(pos + anglevector * 0.5f, pos + anglevector * 0.5f + anglevector * viewDistance * mod, Color.red);
            raycast = Physics2D.Raycast(
                pos + //Position of raycast
                anglevector * 0.5f, //Add direction vector, to avoid touching the collider of the sprite.
                anglevector, //Direction of raycast in a vector (normalized direction in this case)
                viewDistance * mod //Maximum distance
            ).distance;
            mod += (i < sideSightLines) ? modDelta : -modDelta;
            //Asign values to the view data, from the distance value in the raycast
            if (raycast == 0)
                Sight[i] = 0; //Distance is zero if there is no object
            else
                Sight[i] = 1 - (raycast / viewDistance); //return a value between 0-1 for distance to face. 1 is closest.
        }
    }

    public void creatureListen(float r)
    {
        listen += 1 / r;
    }

    public void creatureTalk()
    {
        GameObject[] Creatures;
        Creatures = GameObject.FindGameObjectsWithTag("Creature");
        Vector3 currentPosition = transform.position;
        foreach (GameObject Creature in Creatures)
        {
            Vector3 diffrence = Creature.transform.position - currentPosition;
            float CreatureDistance = diffrence.sqrMagnitude;
            if (CreatureDistance < 4)
            {
                Creature.GetComponent<Creature>().BroadcastMessage("creatureListen", CreatureDistance);
            }
        }
    }

    public void FindClosestFood()
    {
        food closestFood = stat.closestFood(transform.position);

        //if touching eat
        if (closestFood != null)
        {
            float distance = (closestFood.transform.position - transform.position).magnitude;
            if (distance <= 0.7)
            {
                closestFood.SendMessage("Eat");
                creatureHealth += 1;
            }
            Smell = (1f / distance);
        }
        else
            Smell = 0;


        //set smell data
    }

    public override string ToString()
    {
        System.Random r = new System.Random(GetInstanceID());
        char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        string hash = "";
        for (int i = 0; i < r.Next(3, 6); i++)
            hash += alpha[r.Next(alpha.Length)];
        return "CreatureHash:" + hash;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Touch = 1;

        if (collision.collider.gameObject.tag == "Creature" && willingToBreed && BreedingControl.canBreed(this))
        {
            Creature other = collision.collider.gameObject.GetComponent<Creature>();
            if (other.willingToBreed && BreedingControl.canBreed(other))
            {
                BreedingControl.AddCreatureToBreedingList(this, other);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Touch = 0;
    }

    void Die()
    {
        GameObject.FindGameObjectsWithTag("Stat")[0].BroadcastMessage("CreatureDied", timealive);
        //Send the data to the stat gameobject
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate () {

        steps++;
        if (steps >= 4)
        {
            steps = 0;
        }

        //alive time update
        timealive += 0.01f;
        //Decrease health for living
        creatureHealth -= 0.0005f;

        FindClosestFood();
        //gets food info
        //eats if touching food
        //inputs smell sensor

        Look();
        //Throws 5 raycasts and puts the distance output into the view float variables.

        WIP = 0;
        //Here to remind me that there are more sensors that need to be done.

        //Check if dead
        if (creatureHealth <= 0 /*|| float.IsNaN(creatureHealth)*/)
        {
            Die();
        }

        //Run neural network
        RunNeuralNet();

        //Put outputs of NN into the physics engien.
        
        //THERE IS A BUG HERE, QUATERNION OPERATION SQRT(0)?
        transform.Rotate(new Vector3(0, 0, turnSpeed * 7f));
        rBody2D.velocity += new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.PI / 180f) * forwardSpeed, Mathf.Sin(transform.eulerAngles.z * Mathf.PI / 180f) * forwardSpeed);
        //The energy cost of movement
        creatureHealth -= Mathf.Abs(turnSpeed / 10000);
        creatureHealth -= Mathf.Abs(forwardSpeed / 10000);

        /*if (talk >= 0.5)
        {
            noiseShow.SetActive(true);
            creatureTalk();
            creatureHealth -= 0.00002f;
        }
        else
        {
            noiseShow.SetActive(false);
        }
        if (steps == 0)
        {
            hear = listen;
            listen = 0;
        }*/
    }

    public void GenesInput(float[] genes)
    {
        creatureGenome = genes;
    }

}
