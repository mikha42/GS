using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour {

    public GameObject creature;
    public GameObject food;
    public bool testSpawnButton = false;
    public bool testSpawnFood = false;
    private bool prev = false;
    private bool prev2 = false;
    public float range = 40f;


    // Use this for initialization
    void Start () {
		
	}
	
    public void CreateFood()
    {
        GameObject.FindGameObjectsWithTag("Stat")[0].BroadcastMessage("FoodCreated");
        //Send the data to the stat gameobject
        Instantiate(
            food,
            new Vector3(
                Random.Range(-range, range),
                Random.Range(-range, range),
                0
                ),
            Quaternion.identity
            );
    }
    public void CreateCreature()
    {
        GameObject.FindGameObjectsWithTag("Stat")[0].BroadcastMessage("CreatureCreated");
        //Send the data to the stat gameobject
        GameObject newCreature = Instantiate(
            creature,
            new Vector3(
                Random.Range(-range, range),
                Random.Range(-range, range),
                0
                ),
            Quaternion.Euler(0, 0, Random.Range(-180f, 180f))
            );
        float[] randomgenes = new float[111];
        for (int i = 0; i < 3; i++)
        {
            randomgenes[i] = Random.value; //Color value 0-1
        }
        for (int i = 3; i < 111; i++)
        {
            randomgenes[i] = Random.value * 2 - 1; //Neural network weight -1 to 1
        }
        newCreature.GetComponent<Creature>().creatureGenome = randomgenes;
    }

	// Update is called once per frame
	void Update () {
	    if (prev != testSpawnButton)
        {
            prev = testSpawnButton;
            CreateCreature();
        }
        if (prev2 != testSpawnFood)
        {
            prev2 = testSpawnFood;
            CreateFood();
        }
    }
}
