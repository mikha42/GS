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


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if (prev != testSpawnButton)
        {
            prev = testSpawnButton;
            GameObject newCreature = Instantiate(
                creature,
                new Vector3(
                    Random.Range(-10f, 10f), 
                    Random.Range(-10f, 10f), 
                    0
                    ), 
                Quaternion.Euler(0, 0, Random.Range(-180f, 180f))
                );
            float[] randomgenes = new float[81];
            for (int i = 0; i < 3; i++)
            {
                randomgenes[i] = Random.value; //Color value 0-1
            }
            for (int i = 3; i < 81; i++)
            {
                randomgenes[i] = Random.value * 2 - 1; //Neural network weight -1 to 1
            }
            newCreature.GetComponent<Creature>().creatureGenome = randomgenes;
        }
        if (prev2 != testSpawnFood)
        {
            prev2 = testSpawnFood;
            Instantiate(
                food,
                new Vector3(
                    Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f),
                    0
                    ),
                Quaternion.identity
                );
        }
    }
}
