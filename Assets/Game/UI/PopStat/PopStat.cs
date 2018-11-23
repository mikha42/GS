using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopStat : MonoBehaviour {


    public float width = 0.8f;
    public float scale = 1f;
    public float interval = 100;
    public float timer = 0;
    public int line = 0;
    public float progress { get { return (float)line / (float)graph.width; } }
    public bool test = false;


    public Color[] PopLineColors = new Color[] {
        Color.green, Color.cyan, Color.black
    };
    public Color[] checkerBoard = new Color[] {
        Color.white,
        Color.black
    };
    public int checkerBoardSize = 10;

    public Texture2D graph;


    public void resetGraph()
    {
        int creatureCount = GameObject.FindGameObjectWithTag("Stat").GetComponent<stat>().creatureMin;
        //checkerBoardSize = creatureCount / 15;
        graph = new Texture2D((int)(creatureCount * 1.5f * width * scale), (int)(creatureCount * 1.5 * scale));
        for (int i = 0; i < graph.width; i++)
            for (int j = 0; j < graph.height; j++)
                checkerboard(i, j);
        graph.Apply();
    }

    void checkerboard(int x, int y)
    {
        graph.SetPixel(x, y, checkerBoard[
            ((x / checkerBoardSize) + (y / checkerBoardSize)) % checkerBoard.Length
            ]);
    }

    void testf()
    {
        List<Color> creatures = new List<Color>();
        for (int i = 0; i < graph.height; i++)
            creatures.Add(new Color(Random.value, Random.value, Random.value));
        Color[] colorsOrder = new Color[creatures.Count()];
        Dictionary<int, float> refrence = new Dictionary<int, float>();
        for (int i = 0; i < creatures.Count(); i++)
            refrence.Add(i, colorsort(creatures[i]));
        var ordered = refrence.OrderBy(key => key.Value);
        int j = 0;
        foreach (KeyValuePair<int, float> i in ordered)
            colorsOrder[j++] = creatures[i.Key];
        for (int k = 0; k < 100; k++)
        {
            for (int i = 0; i < colorsOrder.Length; i++)
                graph.SetPixel(line + k, i, colorsOrder[i]);
            for (int i = colorsOrder.Length; i < graph.height; i++)
                checkerboard(line + k, i);
        }

        graph.Apply();
        if ((line += 100) >= graph.width)
            line = 0;
    }


    public float stepsH = 4;
    public float stepsL = 3;
    public float RoundC = 3;
    public float RoundCPre = 3;
    float colorsort(Color c)
    {
        float H;
        float S;
        float V;
        Color.RGBToHSV(c, out H, out S, out V);
        return H * 10 + V;
    }
    Color roundC(Color c)
    {
        float H;
        float S;
        float V;
        Color.RGBToHSV(c, out H, out S, out V);
        return Color.HSVToRGB(H, 1, V);
    }


    void scaleup()
    {
        Texture2D old = new Texture2D(graph.width, graph.height);
        old.SetPixels(graph.GetPixels());
        scale *= 1.1f;
        resetGraph();
        for (int i = 0; i < old.width; i++)
            for (int j = 0; j < old.height; j++)
                graph.SetPixel(i, j, old.GetPixel(i, j));
    }

    void addLine()
    {
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");
        if (creatures.Length > graph.height)
            scaleup();
        Color[] colorsOrder = new Color[creatures.Length];
        Dictionary<int, float> refrence = new Dictionary<int, float>();
        for (int i = 0; i < creatures.Length; i++)
            refrence.Add(i, colorsort(creatures[i].GetComponent<Creature>().colourBody.color));
        var ordered = refrence.OrderBy(key => key.Value);
        int j = 0;
        foreach (KeyValuePair<int, float> i in ordered)
            colorsOrder[j++] = creatures[i.Key].GetComponent<Creature>().colourBody.color;
        for (int i = 0; i < colorsOrder.Length; i++)
            graph.SetPixel(line, i, roundC(colorsOrder[i]));
        for (int i = colorsOrder.Length; i < graph.height; i++)
            checkerboard(line, i);
        graph.SetPixel(line, Mathf.RoundToInt(graph.height / ( 1.5f * scale)), PopLineColors[line % PopLineColors.Length]);
        graph.Apply();
        if (line++ >= graph.width)
            line = 0;
    }
    // Use this for initialization
    void Start () {
        resetGraph();
	}

    void FixedUpdate()
    {
        if (test)
        {
            test = false;
            testf();
        }

        timer += Time.fixedDeltaTime;
        if (timer * 1000 >= interval)
        {
            timer = 0;
            addLine();
        }
    }
}
