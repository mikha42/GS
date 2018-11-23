using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stat : MonoBehaviour {


    private static List<Vector3> currFoodLoc = new List<Vector3>();
    private static List<food> currFood = new List<food>();

    public int creatureCount = 0;
    public int foodCount = 0;
    public int creatureMin = 400;
    public int foodMin = 400;
    public int foodeaten = 0;
    public List<float> lifespans = new List<float>();
    public bool repopulate = true;
    public float food_statTimer = 0;
    public float foodPerMinute = 0;
    public float deaths = 0;
    public float deaths_statTimer = 0;
    public float deathsPerMinute = 0;
    public float births = 0;
    public float births_statTimer = 0;
    public float birthsPerMinute = 0;

    public static int genelength = 235;

    public static void updateGeneLength(int i)
    {
        genelength = i;
    }

    public static food closestFood(Vector3 pos)
    {
        if (currFoodLoc.Count == 0)
            return null;
        float minDist = Mathf.Infinity;
        float Dist;
        int id = 0;
        for (int i = 0; i < currFoodLoc.Count; i++)
        {
            Vector3 loc = currFoodLoc[i];
            Dist = (loc - pos).sqrMagnitude;
            if (minDist > Dist)
            {
                minDist = Dist;
                id = i;
            }
        }
        return currFood[id];
    }

    public int maxDegBreedable = 30;

    //These functions are called by other objects, and are stored here to be used by the player and the game
    public void FoodEaten(food self)
    {
        foodCount--;
        foodeaten++;
        currFoodLoc.Remove(self.transform.position);
        currFood.Remove(self);
        Destroy(self.gameObject);
    }
    public void FoodCreated(food self)
    {
        currFoodLoc.Add(self.transform.position);
        currFood.Add(self);
        foodCount++;
    }
    public void CreatureDied(float lifespan)
    {
        //Lifespan will be used to graph average lifespan over time and other things, however im not using it right now, so all its going to do is eat memory.
        //lifespans.Add(lifespan);
        creatureCount--;
        deaths++;
    }
    public void CreatureCreated()
    {
        creatureCount++;
        births++;
    }

	// Use this for initialization
	void Start () {
	}

    void Update()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate () {

        food_statTimer += Time.fixedDeltaTime;
        deaths_statTimer += Time.fixedDeltaTime;
        births_statTimer += Time.fixedDeltaTime;
        foodPerMinute = (60 * foodeaten / food_statTimer);
        deathsPerMinute = (60 * deaths / deaths_statTimer);
        birthsPerMinute = (60 * births / births_statTimer);

        if (food_statTimer > 60)
        {
            food_statTimer = 0;
            foodeaten = 0;
        }
        if (deaths_statTimer > 60)
        {
            deaths_statTimer = 0;
            deaths = 0;
        }
        if (births_statTimer > 60)
        {
            births_statTimer = 0;
            births = 0;
        }



        //Temporary if statements for keeping the population up
        if (repopulate)
        {
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
}
