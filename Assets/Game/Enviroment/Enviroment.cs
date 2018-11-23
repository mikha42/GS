using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enviroment : MonoBehaviour {

    public bool createWalls = false;
    public GameObject WallPrefab;


	// Use this for initialization
	void Start () {
        if (createWalls)
        {
            float range = GameObject.FindGameObjectWithTag("Spawner").GetComponent<CreatureSpawner>().range;
            GameObject wall = Instantiate(WallPrefab, new Vector3(-range - 4, 0, 0), Quaternion.identity);
            wall.transform.localScale = new Vector3(2, 2 * range + 10, 1);
            wall = Instantiate(WallPrefab, new Vector3(range + 4, 0, 0), Quaternion.identity);
            wall.transform.localScale = new Vector3(2, 2 * range + 10, 1);
            wall = Instantiate(WallPrefab, new Vector3(0, -range - 4, 0), Quaternion.identity);
            wall.transform.localScale = new Vector3(2 * range + 8, 2, 1);
            wall = Instantiate(WallPrefab, new Vector3(0, range + 4, 0), Quaternion.identity);
            wall.transform.localScale = new Vector3(2 * range + 8, 2, 1);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}