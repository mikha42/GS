using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goToMouse : MonoBehaviour {

    Camera mainCamera;
    float lastClickTime;
    public float doubleClickTime = 0.1f;

	// Use this for initialization
	void Start () {
        mainCamera = Camera.allCameras[0];
	}
	
    public void toggleCollider()
    {
        this.GetComponent<CircleCollider2D>().enabled = !this.GetComponent<CircleCollider2D>().enabled;
        this.GetComponent<SpriteRenderer>().enabled = !this.GetComponent<SpriteRenderer>().enabled;
    }

	// Update is called once per frame
	void Update () {
        float height = 2f * mainCamera.orthographicSize;       //By getting the resolution of the camera in game
        float width = height * mainCamera.aspect;              //And using a mouse movement not relative to the screen resoulution, I can get a more user friendly system
                                                        //that changes with camera zoom automaticaly.

        transform.position = new Vector3(((float)(Input.mousePosition.x) / (float)Screen.width) * width, ((float)(Input.mousePosition.y) / (float)Screen.height) * height) + mainCamera.transform.position - new Vector3(width / 2, height / 2, -10); //Move camera by the mouse movememnt.

        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastClickTime < doubleClickTime)
            {
                //double click
                Debug.Log("doubleclick");
                GameObject[] Creatures;
                Creatures = GameObject.FindGameObjectsWithTag("Creature");
                GameObject closestCreature = null;
                float closestDistance = Mathf.Infinity;
                Vector3 currentPosition = transform.position;
                foreach (GameObject Creature in Creatures)
                {
                    Vector3 diffrence = Creature.transform.position - currentPosition;
                    float CreatureDistance = diffrence.sqrMagnitude;
                    if (CreatureDistance < closestDistance)
                    {
                        closestCreature = Creature;
                        closestDistance = CreatureDistance;
                    }
                }
                //if touching see info
                if (Creatures.Length != 0 && closestCreature.GetComponent<CircleCollider2D>().bounds.Contains(transform.position))
                {
                    GameObject.FindGameObjectWithTag("UI").GetComponent<UserInterface>().creatureInfo(closestCreature);
                }
            }
            else
            {
                Debug.Log("SingleClick");
                //normal click
            }
            lastClickTime = Time.time;
        }
    }
}
