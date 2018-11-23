using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreedingControl : MonoBehaviour {

    public GameObject creaturePrefab;

    public static List<Creature> currentBreedingCreatures = new List<Creature>(); // Animals that currently want to breed
    public static List<Creature> closedBreedingCreatures = new List<Creature>(); // Animals that can no longer breed
    public static List<float> closedBreedingCreatureTime = new List<float>();   // Time until creatures can breed again
    public float breedingPeriod = 1f;

    public float initialBabyHealth = 1f;
    public float babyGrothCost = 0.1f;
    public float mutationChance = 0.2f;
    public int babyOutputMin= 1;
    public int babyOutputMax = 2;

    public bool viewBabies = false;

    public void Update()
    {
        // Check every frame for animals that are waiting to breed. If there are at least 2, we continue.
        while (currentBreedingCreatures.Count >= 2)
        {
            HandleBreeding(currentBreedingCreatures[0], currentBreedingCreatures[1]);
        }
        currentBreedingCreatures.Clear();
        List<int> readyToBreed = new List<int>();
        for (int i = 0; i < closedBreedingCreatureTime.Count; i++)
        {
            closedBreedingCreatureTime[i] += Time.deltaTime; //Add to time after breeding
            if (closedBreedingCreatureTime[i] >= breedingPeriod) //if time is > breeding period
            {
                readyToBreed.Add(i);
            }
            
        }
        readyToBreed.Reverse();
        for (int i = 0; i < readyToBreed.Count; i++)
        {
            closedBreedingCreatureTime.RemoveAt(i); //remove from closed list
            closedBreedingCreatures.RemoveAt(i);
        }
        Debug.Log("Cant: " + closedBreedingCreatures.Count);
        Debug.Log("Want: " + currentBreedingCreatures.Count);
    }

    void Start()
    {


        //What to do about asking the question?
        //Unity fourms

        closedBreedingCreatures = new List<Creature>();
        currentBreedingCreatures = new List<Creature>();
    }

    // Put your code that handles the breeding inside of here. 
    private void HandleBreeding(Creature creature1, Creature creature2)
    {
        RemoveCreatureFromBreedingList(creature1);
        RemoveCreatureFromBreedingList(creature2);

        float[] genes1 = creature1.creatureGenome;
        float[] genes2 = creature2.creatureGenome;

        int geneLength = genes1.Length;
        //Because i use the length of the genes alot, i can define them here instead of calling the genes.length constantly

        float[][] offspringGenes = new float[Random.Range(babyOutputMin, babyOutputMax)][];
        //This is where the babies are created. This line also defines how many babies are to be made.

        int crossoverPoint = 0;
        float[][] crossoverGenes;


        //Good health genetic algorithm. Uses the three sources of randomness that cells use.
        for (int i = 0; i < offspringGenes.Length; i++)
        {
            crossoverPoint = Random.Range(0, geneLength - 2); //get crossover point, assuming both genes are of equal length
            //-2 because im using the points inbetween the floats, dont want to cut a float in half.
            //This also means that there will always be at least one float that gets put into the other genes.
            crossoverGenes = new float[2][] { new float[geneLength], new float[geneLength] };
            
            //Crossover
            for (int j = 0; j < geneLength; j++)
            {
                if (j <= crossoverPoint) // (0 <= 0) and (geneLength - 1 !<= genelength - 2) so ONE is always spared
                {
                    crossoverGenes[0][j] = genes1[j];
                    crossoverGenes[1][j] = genes2[j];
                }
                else
                {
                    crossoverGenes[0][j] = genes2[j]; //Notice the gene1/gene2 have swaped sides.
                    crossoverGenes[1][j] = genes1[j]; //Hence crossover
                }
            }

            //Randomly select one of the crossover genes for the baby
            offspringGenes[i] = crossoverGenes[Random.Range(0, 1)];

            //Mutation
            while (Random.value <= mutationChance) //While loop for multiple point mutations. Read my part on efficiency to see why i do this.
            {
                offspringGenes[i][Random.Range(0, offspringGenes[i].Length - 1)] += (Random.value - 1) / 4; // += (-0.125 to +0.125)
            }
        }

        Vector3 spawnpos = new Vector2(
            (creature1.gameObject.transform.position.x + creature2.gameObject.transform.position.x) / 2,
            (creature1.gameObject.transform.position.y + creature2.gameObject.transform.position.y) / 2
            );
        //get average between the two creatures, to spawn the babies in between them.


        //See how much the parents can pay
        float availableFunds = Mathf.Min(creature1.creatureHealth, (initialBabyHealth / 2) * offspringGenes.Length) + Mathf.Min(creature2.creatureHealth, (initialBabyHealth / 2) * offspringGenes.Length);
        float babyHealth = availableFunds / offspringGenes.Length; //Devide the funds equaly

        //PLACE THE BABIES
        foreach (float[] genes in offspringGenes)
        {
            GameObject.FindGameObjectsWithTag("Stat")[0].GetComponent<stat>().CreatureCreated();
            GameObject baby = Instantiate(creaturePrefab, spawnpos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            baby.GetComponent<Creature>().GenesInput(genes);
            baby.GetComponent<Creature>().creatureHealth = babyHealth;
            baby.GetComponent<Creature>().activation = creature1.activation;
            RemoveCreatureFromBreedingList(baby.GetComponent<Creature>()); //Baby cant breed for breedingPeriod
        }


        //Its time for energy costs of the babies.
        //if the parents cant pay, the babies gets less, and the parents die!~

        
        creature1.creatureHealth -= ((initialBabyHealth + babyGrothCost) / 2) * offspringGenes.Length;
        creature2.creatureHealth -= ((initialBabyHealth + babyGrothCost) / 2) * offspringGenes.Length;

        //This is in case i want to look at the babies once they are created.
        if (viewBabies) {
            Camera cam = Camera.allCameras[0];
            cam.gameObject.transform.position = new Vector3(spawnpos.x, spawnpos.y, -10);
        }
    }

    public static void AddCreatureToBreedingList(Creature creature)
    {
        if (creature == null)
        {
            Debug.Log("Attempted to add a null creature into the currentBreedingCreatures list.");
            return; // Creature is null. We HATE nulls.
        }

        if (closedBreedingCreatures.Contains(creature))
            return; // Creature already bred. We don't want this one.

        if (currentBreedingCreatures.Contains(creature))
            return; // We already added this creature. We don't want this one, either.

        currentBreedingCreatures.Add(creature);
    }

    private void RemoveCreatureFromBreedingList(Creature creature)
    {
        if (currentBreedingCreatures.Contains(creature))
            currentBreedingCreatures.Remove(creature);

        closedBreedingCreatures.Add(creature);
        closedBreedingCreatureTime.Add(0f);
    }
}
