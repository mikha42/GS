using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stat : MonoBehaviour {

    public int creatureCount = 0;
    public int foodCount = 0;
    public List<float> lifespans = new List<float>();

    public void FoodEaten()
    {
        foodCount--;
    }
    public void FoodCreated()
    {
        foodCount++;
    }
    public void CreatureDied(float lifespan)
    {
        lifespans.Add(lifespan);
        creatureCount--;
    }
    public void CreatureCreated()
    {
        creatureCount++;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (creatureCount < 10)
        {
            GameObject.FindGameObjectsWithTag("Spawner")[0].BroadcastMessage("CreateCreature");
        }
        if (foodCount < 20)
        {
            GameObject.FindGameObjectsWithTag("Spawner")[0].BroadcastMessage("CreateFood");
        }
    }
}
