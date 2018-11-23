using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour {

    public RectTransform MainPanel;
    public GameObject GuiToggleButton;
    public GameObject CreatureTabButton;
    public GameObject CreatureTab;
    public GameObject PauseButton;
    public GameObject currentInfo;
    public GameObject timeSlider;
    public RawImage popStatImg;



    public GameObject CreatureSpawner;
    public GameObject Stat;
    public GameObject BreedingControl;
    public PopStat popStat;
    public GameObject Enviroment;


    public bool Graph1FullScreen = false;
    public Vector3 g1inPos;
    public Vector2 g1inSD;
    public Vector3 g1outPos;
    public Vector2 g1outSD;
    public float timeScale = 1f;
    public float timeMod = 7f;
    public float timeModG1 = 7f;
    public bool MainPanelOut = false;
    public Vector3 InPosition;
    public Vector3 OutPosition;
    public Vector3 worldScreenMiddle;
    public Vector3 screenSize;


    public GameObject inputMethodButtonText;
    public GameObject inputMethodNumbers;
    public GameObject inputMethodRatios;
    public GameObject ratioMagnitude;
    public GameObject ratioPopDensity;
    public GameObject ratioFoodDensity;
    public GameObject numberPopulation;
    public GameObject numberFood;
    public GameObject numberRange;


    public void toggleInputMethod()
    {
        if (inputMethodButtonText.GetComponent<Text>().text == "Numbers")
        {
            inputMethodButtonText.GetComponent<Text>().text = "Ratios";
            inputMethodNumbers.SetActive(false);
            inputMethodRatios.SetActive(true);
        }
        else
        {
            inputMethodButtonText.GetComponent<Text>().text = "Numbers";
            inputMethodNumbers.SetActive(true);
            inputMethodRatios.SetActive(false);
        }
    }

    // Use this for initialization
    void Start () {


        worldScreenMiddle = new Vector3(0, 0, 0);
        screenSize = new Vector3(Screen.width, Screen.height, 0);
        g1inPos = popStatImg.GetComponent<RectTransform>().anchoredPosition;
        g1inSD = popStatImg.GetComponent<RectTransform>().sizeDelta;
        g1outSD = new Vector2(Screen.width * 0.65f, Screen.height * 0.7f);

        InPosition = MainPanel.anchoredPosition + new Vector2(290, 0);
        OutPosition = MainPanel.anchoredPosition;
        numberPopulation.GetComponent<InputField>().text = Stat.GetComponent<stat>().creatureMin + "";
        numberFood.GetComponent<InputField>().text = Stat.GetComponent<stat>().foodMin + "";
        numberRange.GetComponent<InputField>().text = CreatureSpawner.GetComponent<CreatureSpawner>().GetRange() + "";
    }


    public void toggleFullscreenGraph1()
    {
        Graph1FullScreen = true;
        MainPanelOut = false;
        GuiToggleButton.GetComponentInChildren<Text>().text = "Return";
    }

    public void resetAll()
    {
        SceneManager.LoadScene("main");
    }
    public void Pause()
    {
        Time.timeScale = (Time.timeScale == 0) ? timeScale : 0;
        PauseButton.GetComponentInChildren<Text>().text = (Time.timeScale == 1) ? "Pause Creatures" : "Resume Creatures";
    }

    public void creatureInfo(GameObject creature)
    {
        MainPanelOut = true;
        CreatureTabButton.GetComponent<Toggle>().isOn = true;
        CreatureTab.GetComponentInChildren<CreatureUI>().details(creature.GetComponent<Creature>());
    }

    public void setPopulationSpawn()
    {
        int newPopulation;
        if (int.TryParse(numberPopulation.GetComponent<InputField>().text, out newPopulation))
        {
            Stat.GetComponent<stat>().creatureMin = newPopulation;
            popStat.resetGraph();
        }
        else
        {
            numberPopulation.GetComponent<InputField>().text = Stat.GetComponent<stat>().creatureMin + "";
        }

    }
    public void setFoodSpawn()
    {
        int newFoodCount;
        if (int.TryParse(numberFood.GetComponent<InputField>().text, out newFoodCount))
        {
            Stat.GetComponent<stat>().foodMin = newFoodCount;
        }
        else
        {
            numberFood.GetComponent<InputField>().text = Stat.GetComponent<stat>().foodMin + "";
        }
    }
    public void setSpawnRange()
    {
        float newRange;
        if (float.TryParse(numberRange.GetComponent<InputField>().text, out newRange))
        {
            CreatureSpawner.GetComponent<CreatureSpawner>().UpdateRange(newRange);
        }
        else
        {
            numberRange.GetComponent<InputField>().text = CreatureSpawner.GetComponent<CreatureSpawner>().GetRange() + "";
        }
    }
        
    public void ToggleGUI()
    {
        Graph1FullScreen = false;
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

    public void TimeWarp(float s)
    {
        timeScale = timeSlider.GetComponent<Slider>().value / 3 + 2 / 3;
        Time.timeScale = timeScale;
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


    float round(float a, int dig)
    {
        if (a == 0)
            return 0;
        a *= (Mathf.Pow(10, dig));
        a = Mathf.Floor(a);
        a /= Mathf.Pow(10, dig);
        return a;
    }




    // Update is called once per frame
    void Update()
    {
        float deltaTime = 1f / 60f;


        popStatImg.texture = popStat.graph;
        popStatImg.uvRect = new Rect(popStat.progress, 0, 1, 1);
        popStatImg.texture.filterMode = FilterMode.Point;
        popStatImg.texture.wrapMode = TextureWrapMode.Repeat;
        //Debug.Log(MainPanel.anchoredPosition);

        float area = (CreatureSpawner.GetComponent<CreatureSpawner>().GetRange() * CreatureSpawner.GetComponent<CreatureSpawner>().GetRange());
        currentInfo.GetComponent<Text>().text =
            "Population:               " + Stat.GetComponent<stat>().creatureCount + "\n" +
            ((Stat.GetComponent<stat>().creatureCount != 0) ?
            "Current Creature Density: " + round(Stat.GetComponent<stat>().creatureCount / area, 3) + "\n" :
            "Min Creature Density:     " + round(Stat.GetComponent<stat>().creatureMin / area, 3) + "\n" 
            )+
            "Food Count:               " + Stat.GetComponent<stat>().foodCount + "\n" +
            ((Stat.GetComponent<stat>().foodCount != 0) ?
            "Current food Density: " + round(Stat.GetComponent<stat>().foodCount / area, 3) + "\n" :
            "Min food Density:     " + round(Stat.GetComponent<stat>().foodMin / area, 3) + "\n"
            ) +
            "Average Eating Rate:      " + round(Stat.GetComponent<stat>().foodPerMinute / Stat.GetComponent<stat>().creatureCount, 2) + "\n" +
            "Death rate:               " + Stat.GetComponent<stat>().deathsPerMinute + "\n" +
            "Birth rate:               " + Stat.GetComponent<stat>().birthsPerMinute + "\n"
            ;


        if (Graph1FullScreen)
        {
            Vector3[] centers = new Vector3[4];
            popStatImg.GetComponent<RectTransform>().GetWorldCorners(centers);
            Vector3 screenDiff = centers[0] - new Vector3(50, 50);
            Vector3 anchorDelta = -timeModG1 * 6f * Vector3.Normalize(screenDiff) * Mathf.Min(3 * Mathf.Sqrt(screenDiff.magnitude / screenSize.magnitude), 10);
            if (anchorDelta.magnitude > timeModG1)
                popStatImg.gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(anchorDelta.x, anchorDelta.y);
            screenDiff = centers[2] - screenSize + new Vector3(50, 50);
            Vector3 sizeDeltaDelta = -timeModG1 * 6f * Vector3.Normalize(screenDiff) * Mathf.Min(3 * Mathf.Sqrt(screenDiff.magnitude / screenSize.magnitude), 10); ;
            if (sizeDeltaDelta.magnitude > timeModG1)
                popStatImg.gameObject.GetComponent<RectTransform>().sizeDelta += new Vector2(sizeDeltaDelta.x, sizeDeltaDelta.y);//new Vector2(sizeDeltaDelta.x, sizeDeltaDelta.y);

        }
        else
        {
            popStatImg.gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(popStatImg.gameObject.GetComponent<RectTransform>().anchoredPosition, g1inPos, timeModG1 * deltaTime);
            popStatImg.gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(popStatImg.gameObject.GetComponent<RectTransform>().sizeDelta, g1inSD, timeModG1 * deltaTime);
        }

        if (MainPanelOut)
            MainPanel.anchoredPosition = Vector3.Lerp(MainPanel.anchoredPosition, OutPosition, timeMod * deltaTime);
        else
            MainPanel.anchoredPosition = Vector3.Lerp(MainPanel.anchoredPosition, InPosition, timeMod * deltaTime);

    }
}
