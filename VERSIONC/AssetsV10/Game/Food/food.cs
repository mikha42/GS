using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class food : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
    public void Eat()
    {
        //Creatures call this function when they eat the food.
        GameObject.FindGameObjectsWithTag("Stat")[0].BroadcastMessage("FoodEaten");
        Destroy(gameObject);
    }

	// Update is called once per frame
	void Update () {
		
	}
}