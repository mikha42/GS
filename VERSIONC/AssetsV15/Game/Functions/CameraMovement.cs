using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    Vector2 mouseNow;
    Vector2 mouseBefore;
    Vector2 deltaMouse;
    Camera cam;

    // Use this for initialization
    void Start () {
        cam = gameObject.GetComponent<Camera>();
        mouseNow = Input.mousePosition;
        mouseBefore = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {
        mouseNow = Input.mousePosition; //Get current mouse
        deltaMouse = mouseNow - mouseBefore; //Get mouse movement since last frame
        mouseBefore = mouseNow; //Set current mouse to last frame mouse

        if (Input.GetMouseButton(1)) //Check if mouse is down
        {
            float height = 2f * cam.orthographicSize;       //By getting the resolution of the camera in game
            float width = height * cam.aspect;              //And using a mouse movement not relative to the screen resoulution, I can get a more user friendly system
                                                            //that changes with camera zoom automaticaly.

            transform.position += new Vector3((-(float)(deltaMouse.x) / (float)Screen.width) * width, (-(float)(deltaMouse.y) / (float)Screen.height) * height) ; //Move camera by the mouse movememnt.
        }


        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, cam.orthographicSize * Mathf.Pow(1.75f, -Input.mouseScrollDelta.y), 1f); //Zoom
        //Lerp is a function that changes the value of the size over a period of time. in this case 0.05f secounds (defined above)
        

	}
}
