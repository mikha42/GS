﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stat : MonoBehaviour {

    public int creatureCount = 0;
    public int foodCount = 0;
    public int creatureMin = 400;
    public int foodMin = 400;
    public List<float> lifespans = new List<float>();

    //These functions are called by other objects, and are stored here to be used by the player and the game
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

        //Temporary if statements for keeping the population up
		if (creatureCount < creatureMin)
        {
            GameObject.FindGameObjectsWithTag("Spawner")[0].BroadcastMessage("CreateCreature");
            //Send a request to create a creature to the creaturespawner object
        }
        if (foodCount < foodMin)
        {
            GameObject.FindGameObjectsWithTag("Spawner")[0].BroadcastMessage("CreateFood");
            //Send a request to create a creature to the creaturespawner object
        }
    }
}
