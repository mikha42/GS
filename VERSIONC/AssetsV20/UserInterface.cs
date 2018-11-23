using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {

    public GameObject timeScaleSlider;


	// Use this for initialization
	void Start () {
		
	}
	


	// Update is called once per frame
	void Update () {
        //Sets the speed of the game depending on the value of the slider
        Time.timeScale = timeScaleSlider.GetComponent<Slider>().value;
    }
}
