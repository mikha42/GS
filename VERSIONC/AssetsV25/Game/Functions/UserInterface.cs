using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour {

    
    public RectTransform MainPanel;
    public GameObject GuiToggleButton;
    public GameObject PopulationSpawnInput;
    public GameObject FoodSpawnInput;
    public GameObject SpawnRangeInput;
    public GameObject CreatureTabButton;
    public GameObject CreatureTab;



    public GameObject CreatureSpawner;
    public GameObject Stat;
    public GameObject BreedingControl;
    public GameObject Enviroment;


    public float timeMod = 7f;
    public bool MainPanelOut = false;
    public Vector3 InPosition;
    public Vector3 OutPosition;
    

    // Use this for initialization
    void Start () {
        InPosition = MainPanel.anchoredPosition + new Vector2(290, 0);
        OutPosition = MainPanel.anchoredPosition;
        PopulationSpawnInput.GetComponent<InputField>().text = Stat.GetComponent<stat>().creatureMin + "";
        FoodSpawnInput.GetComponent<InputField>().text = Stat.GetComponent<stat>().foodMin + "";
        SpawnRangeInput.GetComponent<InputField>().text = CreatureSpawner.GetComponent<CreatureSpawner>().GetRange() + "";
    }
	
    public void resetAll()
    {
        SceneManager.LoadScene("main");
    }

    public void creatureInfo(GameObject creature)
    {
        CreatureTabButton.GetComponent<Toggle>().isOn = true;
        CreatureTab.GetComponentInChildren<CreatureUI>().details(creature.GetComponent<Creature>());
    }

    public void setPopulationSpawn()
    {
        int newPopulation;
        if (int.TryParse(PopulationSpawnInput.GetComponent<InputField>().text, out newPopulation))
        {
            Stat.GetComponent<stat>().creatureMin = newPopulation;
            PopulationSpawnInput.GetComponent<InputField>().text = Stat.GetComponent<stat>().creatureMin + "";
        }

    }
    public void setFoodSpawn()
    {
        int newFoodCount;
        if (int.TryParse(FoodSpawnInput.GetComponent<InputField>().text, out newFoodCount))
        {
            Stat.GetComponent<stat>().foodMin = newFoodCount;
            FoodSpawnInput.GetComponent<InputField>().text = Stat.GetComponent<stat>().foodMin + "";
        }
    }
    public void setSpawnRange()
    {
        float newRange;
        if (float.TryParse(SpawnRangeInput.GetComponent<InputField>().text, out newRange))
        {
            CreatureSpawner.GetComponent<CreatureSpawner>().UpdateRange(newRange);
            SpawnRangeInput.GetComponent<InputField>().text = CreatureSpawner.GetComponent<CreatureSpawner>().GetRange() + "";
        }
    }
        
    public void ToggleGUI()
    {
        MainPanelOut = !MainPanelOut;
        if (MainPanelOut)
        {
            GuiToggleButton.GetComponentInChildren<Text>().text = "Close";
        }
        else
        {
            GuiToggleButton.GetComponentInChildren<Text>().text = "Open";
        }
    }

    public void StatToggleSpwning()
    {
        Stat.GetComponent<stat>().repopulate = !Stat.GetComponent<stat>().repopulate;
    }


    public void CamToggleOffspring()
    {
        BreedingControl.GetComponent<BreedingControl>().viewBabies = !BreedingControl.GetComponent<BreedingControl>().viewBabies;
    }

    public void ActivationFunction(Text i)
    {
        Debug.Log(i.text);
        int num = 0;
        switch (i.text)
        {
            case "Hyperbolic Tan":
                num = 0;
                break;
            case "Sinusoid":
                num = 1;
                break;
            case "Arctan":
                num = 2;
                break;
            case "Linear":
                num = 3;
                break;
            case "Gaussian":
                num = 4;
                break;
            case "Logistic":
                num = 5;
                break;
            case "Binary":
                num = 6;
                break;
            case "Heaviside":
                num = 7;
                break;
        }
        CreatureSpawner.GetComponent<CreatureSpawner>().activationfunction = num;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(MainPanel.anchoredPosition);

        if (MainPanelOut)
        {
            MainPanel.anchoredPosition = Vector3.Lerp(MainPanel.anchoredPosition, OutPosition, timeMod * Time.deltaTime);
        }
        else
        {
            MainPanel.anchoredPosition = Vector3.Lerp(MainPanel.anchoredPosition, InPosition, timeMod * Time.deltaTime);
        }

    }
}
