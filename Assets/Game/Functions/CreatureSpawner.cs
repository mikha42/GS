using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour {

    public float countdown = 0;
    public float delay = 100;
    public float FoodCountdown = 0;
    public float FoodDelay = 100;
    public GameObject creature;
    public GameObject food;
    public float range = 16f;
    public int activationfunction = 7;


    // Use this for initialization
    void Start () {
		
	}
	
    public void UpdateRange(float newRange)
    {
        range = newRange;
    }
    public float GetRange()
    {
        return range;
    }

    public bool CreateFood()
    {
        //Send the data to the stat gameobject
        if (FoodCountdown > 0)
            return false;
        Vector3 point = new Vector3(
                Random.Range(-range, range),
                Random.Range(-range, range),
                0
                );
        if (Physics2D.OverlapCircleAll(point, 0.5f).Length != 0)
        {
            return false;
        }
        else
        {
            food Food = Instantiate(
                food,
                point,
                Quaternion.identity
                ).GetComponent<food>();
            GameObject.FindGameObjectsWithTag("Stat")[0].BroadcastMessage("FoodCreated", Food);
        }
        FoodCountdown = FoodDelay;
        return true;
    }
    public bool CreateCreature()
    {
        if (countdown > 0)
            return false;
        GameObject newCreature = Instantiate(
            creature,
            new Vector3(
                Random.Range(-range, range),
                Random.Range(-range, range),
                0
                ),
            Quaternion.Euler(0, 0, Random.Range(-180f, 180f))
            );
        float[] randomgenes = new float[stat.genelength];
        for (int i = 0; i < 3; i++)
        {
            randomgenes[i] = Random.value; //Color value 0-1
        }
        for (int i = 3; i < stat.genelength; i++)
        {
            randomgenes[i] = Random.value * 2 - 1; //Neural network weight -1 to 1
        }
        newCreature.GetComponent<Creature>().creatureGenome = randomgenes;
        newCreature.GetComponent<Creature>().activation = activationfunction;

        GameObject.FindGameObjectsWithTag("Stat")[0].BroadcastMessage("CreatureCreated");
        //Send the data to the stat gameobject
        countdown = delay;
        return true;
    }

    private void FixedUpdate()
    {
        if (countdown > 0)
            countdown -= Time.fixedDeltaTime * 1000;
        if (FoodCountdown > 0)
            FoodCountdown -= Time.fixedDeltaTime * 1000;

    }
}
