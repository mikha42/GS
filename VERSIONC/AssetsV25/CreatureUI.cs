using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureUI : MonoBehaviour {

    Creature ActiveView;
    public Text CreatureName;
    public Text CreatureStats;
    public Text CreatureNN;
    

	// Use this for initialization
	void Start () {
		
	}

    public void details(Creature details)
    {
        ActiveView = details;
    }
	
	// Update is called once per frame
	void Update () {
        if (ActiveView != null)
        {
            CreatureName.text = "IID: " + ActiveView.GetInstanceID();
            CreatureStats.text =
                "Time Alive: " + ActiveView.timealive +
                "\nCurrent Health: " + ActiveView.creatureHealth +
                "\nColor: " + ActiveView.colourBody.color.r * 255 + ", " + ActiveView.colourBody.color.g * 255 + ", " + ActiveView.colourBody.color.b * 255;
            CreatureNN.text = "Willingness to breed: " + ActiveView.willingnessToBreed;
        }
        else
        {
            CreatureName.text = "No Creature Selected";
            CreatureStats.text = "Double click to select a creature";
            CreatureNN.text = "";
        }

    }
}
