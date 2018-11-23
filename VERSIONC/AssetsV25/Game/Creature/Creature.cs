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
    public float[] creatureGenome = new float[236];
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
    public float viewForward = 0;
    public float viewLeft1 = 0;
    public float viewLeft2 = 0;
    public float viewRight1 = 0;
    public float viewRight2 = 0;
    public float WIP = 0;
    public float listen = 0;
    public float hear = 0;

    public float viewDistance = 15;

    //This is the neural network
    void RunNeuralNet()
    {
        //Variables
        int WeightIndex = 3; //to increment each time a weight is used. Starts at three, because the first three are colour values
        float NodeSum = 0;
        float[] Outputs = new float[8]; //Output array. Updating the length of this will update the length of the output
        float[] Inputs = new float[] { //Input into the neural network
            creatureHealth,
            hear,
            Smell,
            Touch,
            viewForward,
            viewLeft1,
            viewLeft2,
            viewRight1,
            viewRight2,
            recurrentNN[0],
            recurrentNN[1],
            recurrentNN[2],
            recurrentNN[3]
        };
        float[] Hidden1 = new float[8]; //The first hidden network
        float[] Hidden2 = new float[8]; //The secound hidden network

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
        colourEye.color = willingToBreed ? new Color(1, 182f / 255f, 193f / 255f) : new Color(1, 1, 1);

    }

    float ActivationFunction(float i)
    {
        //Activation function is used to set the value of the nodes in the neural network.
        //get user setting for activation function
        switch (activation) //user setting
        {
            case 0: //hyperbolic tan
                i = i * 1;  // Convert to number.
                            // i is Infinity or NaN
                if (Mathf.Abs(i) == Mathf.Infinity)
                {
                    if (i > 0) return 1;
                    if (i < 0) return -1;
                    return i;
                }
                var ai = Mathf.Abs(i);
                float z;
                // |i| < 22
                if (ai < 22)
                {
                    var twoM55 = 2.77555756156289135105e-17; // 2^-55, empty lower half
                    if (ai < twoM55)
                    {
                        // |i| < 2^-55, tanh(small) = small.
                        return i;
                    }
                    if (ai >= 1)
                    {
                        // |i| >= 1
                        var t = Mathf.Exp(2 * ai);
                        z = 1 - 2 / (t + 2);
                    }
                    else
                    {
                        var t = Mathf.Exp(-2 * ai);
                        z = -t / (t + 2);
                    }
                }
                else
                {
                    // |i| > 22, return +/- 1
                    z = 1;
                }
                return (i >= 0) ? z : -z;
            //No break required because return quits the switch.
            case 1:
                return Mathf.Sin(i);
            case 2:
                return Mathf.Atan(i);
            case 3:
                return i;
            case 4:
                return Mathf.Exp(-Mathf.Pow(i, 2));
            case 5:
                return (1f / (1f + Mathf.Exp(-i)));
            case 6:
                return (i >= 0) ? 1 : 0;
            case 7:
                return (i == 0) ? 0f : ((i > 0) ? 1 : -1);

        }
        return 0;
    }
    
	// Use this for initialization
	void Start () {
        //Debug.Log(creatureHealth);
        sRender = gameObject.GetComponent<SpriteRenderer>();
        rBody2D = gameObject.GetComponent<Rigidbody2D>();
        colourBody.color = new Color(creatureGenome[0], creatureGenome[1], creatureGenome[2]);
    }

    public void Look()
    {

        //Use physics raycast to see if there are any objects in front of the creature (View)

        Vector2 anglevector = (Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * Vector2.right);
        RaycastHit2D viewForward = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y) + //Position of raycast
            anglevector * 0.5f, //Add direction vector, to avoid touching the collider of the sprite.
            anglevector, //Direction of raycast in a vector (normalized direction in this case)
            viewDistance //Maximum distance
            );

        anglevector = (Quaternion.AngleAxis(transform.eulerAngles.z + 10, Vector3.forward) * Vector2.right);
        RaycastHit2D viewLeft1 = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y) + //Position of raycast
            anglevector * 0.5f, //Add direction vector, to avoid touching the collider of the sprite.
            anglevector, //Direction of raycast in a vector (normalized direction in this case)
            viewDistance * 0.8f //Maximum distance
        );

        anglevector = (Quaternion.AngleAxis(transform.eulerAngles.z + 20, Vector3.forward) * Vector2.right);
        RaycastHit2D viewLeft2 = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y) + //Position of raycast
            anglevector * 0.5f, //Add direction vector, to avoid touching the collider of the sprite.
            anglevector, //Direction of raycast in a vector (normalized direction in this case)
            viewDistance * 0.6f //Maximum distance
        );

        anglevector = (Quaternion.AngleAxis(transform.eulerAngles.z - 10, Vector3.forward) * Vector2.right);
        RaycastHit2D viewRight1 = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y) + //Position of raycast
            anglevector * 0.5f, //Add direction vector, to avoid touching the collider of the sprite.
            anglevector, //Direction of raycast in a vector (normalized direction in this case)
            viewDistance * 0.8f//Maximum distance
        );

        anglevector = (Quaternion.AngleAxis(transform.eulerAngles.z - 20, Vector3.forward) * Vector2.right);
        RaycastHit2D viewRight2 = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y) + //Position of raycast
            anglevector * 0.5f, //Add direction vector, to avoid touching the collider of the sprite.
            anglevector, //Direction of raycast in a vector (normalized direction in this case)
            viewDistance * 0.6f //Maximum distance
        );



        //Asign values to the view data, from the distance value in the raycast
        if (viewForward.distance == 0)
        {
            this.viewForward = 0; //Distance is zero if there is no object
        }
        else
        {
            this.viewForward = 1 - (viewForward.distance / viewDistance); //return a value between 0-1 for distance to face. 1 is closest.
        }

        if (viewLeft1.distance == 0)
        {
            this.viewLeft1 = 0; //Distance is zero if there is no object
        }
        else
        {
            this.viewLeft1 = 1 - (viewLeft1.distance / (viewDistance * 0.8f)); //return a value between 0-1 for distance to face. 1 is closest.
        }

        if (viewLeft2.distance == 0)
        {
            this.viewLeft2 = 0; //Distance is zero if there is no object
        }
        else
        {
            this.viewLeft2 = 1 - (viewLeft2.distance / (viewDistance * 0.6f)); //return a value between 0-1 for distance to face. 1 is closest.
        }

        if (viewRight1.distance == 0)
        {
            this.viewRight1 = 0; //Distance is zero if there is no object
        }
        else
        {
            this.viewRight1 = 1 - (viewRight1.distance / (viewDistance * 0.8f)); //return a value between 0-1 for distance to face. 1 is closest.
        }

        if (viewRight2.distance == 0)
        {
            this.viewRight2 = 0; //Distance is zero if there is no object
        }
        else
        {
            this.viewRight2 = 1 - (viewRight2.distance / (viewDistance * 0.6f)); //return a value between 0-1 for distance to face. 1 is closest.
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
                BreedingControl.AddCreatureToBreedingList(collision.otherCollider.gameObject.GetComponent<Creature>());
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Touch = 0;
    }

    // Update is called once per frame
    void Update () {

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
            GameObject.FindGameObjectsWithTag("Stat")[0].BroadcastMessage("CreatureDied", timealive);
            //Send the data to the stat gameobject
            Destroy(gameObject);
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

        if (talk >= 0.5)
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
        }
    }

    public void GenesInput(float[] genes)
    {
        creatureGenome = genes;
    }
}
