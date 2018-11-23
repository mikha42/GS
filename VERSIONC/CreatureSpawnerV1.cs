using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour {

    public GameObject creature;
    //Assigned to in the editor, used to instanciate the creature
    public GameObject food;
    //Same as creature, but for food.

    //These four are for testing in the editor.
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
                creature, //prefab
                new Vector3( //Random location
                    Random.Range(-10f, 10f),  //X
                    Random.Range(-10f, 10f),  //Y
                    Random.Range(-10f, 10f)   //Z
                    ), 
                Quaternion.Euler(0, 0, Random.Range(-180f, 180f)) //Random Angle
                );
            float[] randomgenes = new float[81]; // Genes to be put into newCreature
            for (int i = 0; i < 3; i++) //First 3 are color
            {
                randomgenes[i] = Random.value; //Color value 0-1
            }
            for (int i = 3; i < 81; i++) //last ones are genes.
            {
                randomgenes[i] = Random.value * 2 - 1; //Neural network weight -1 to 1
            }
            newCreature.GetComponent<Creature>().creatureGenome = randomgenes;
        }
        if (prev2 != testSpawnFood)
        {
            prev2 = testSpawnFood;
            Instantiate(
                food, //Prefab
                new Vector3( //Random position
                    Random.Range(-10f, 10f),  //X
                    Random.Range(-10f, 10f),  //Y
                    Random.Range(-10f, 10f)   //Z
                    ),
                Quaternion.identity   //Roatation, doesn't matter for food
                );
        }
    }
}
