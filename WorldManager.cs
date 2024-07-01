using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using UnityEditorInternal.VR;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private GameObject agentContainer;
    [SerializeField] private int numberOfPrey;
    [SerializeField] private int numberOfPredators;
    public GameObject preyPrefab;
    public GameObject predatorPrefab;
    public List<GameObject> preyList = new List<GameObject>();
    public List<GameObject> predatorList = new List<GameObject>();
    public float coordinateLimit = 6f;

    public float episodeDuration = 60f; // Duration of each episode in seconds
    private float timer; // Timer for tracking episode duration

    void Start()
    {
        SpawnAgents();
        timer = episodeDuration; // Initialize timer at the start
    }

    private void SpawnAgents()
    {
        // Spawn predators
        for (int i = 0; i < numberOfPredators; i++)
        {
            Vector3 position = new Vector3(Random.Range(-coordinateLimit, coordinateLimit), 1, Random.Range(-coordinateLimit, coordinateLimit));
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0); // Random rotation around the y-axis
            GameObject predator = Instantiate(predatorPrefab, agentContainer.transform.position + position, rotation);
            predator.transform.parent = agentContainer.transform;
            predatorList.Add(predator);

            PredatorAgent predatorAgent = predator.GetComponent<PredatorAgent>();
            if (predatorAgent != null)
            {
                predatorAgent.worldManager = this;
            }
        }

        // Spawn prey
        for (int i = 0; i < numberOfPrey; i++)
        {
            Vector3 position = new Vector3(Random.Range(-coordinateLimit, coordinateLimit), 1, Random.Range(-coordinateLimit, coordinateLimit));
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0); // Random rotation around the y-axis
            GameObject prey = Instantiate(preyPrefab, agentContainer.transform.position + position, rotation);
            prey.transform.parent = agentContainer.transform;
            preyList.Add(prey);

            PreyAgent preyAgent = prey.GetComponent<PreyAgent>();
            if (preyAgent != null)
            {
                preyAgent.worldManager = this;
            }
        }
    }

    public int GetActivePreyCount()
    {
        int activePreyCount = 0;
        foreach (GameObject prey in preyList)
        {
            if (prey.activeSelf)
            {
                activePreyCount++;
            }
        }
        return activePreyCount;
    }

    public int GetActivePredatorCount()
    {
        int activePredatorCount = 0;
        foreach (GameObject predator in predatorList)
        {
            if (predator.activeSelf)
            {
                activePredatorCount++;
            }
        }
        return activePredatorCount;
    }


    private void Update()
    {
        // Update timer
        timer -= Time.deltaTime;

        // Check if episode should end
        if (timer <= 0f || GetActivePreyCount() == 0 || GetActivePredatorCount() == 0)
        {
            EndAllEpisodes();
            timer = episodeDuration; // Reset timer for next episode
        }
    }

    private void EndAllEpisodes()
    {
        // End episodes for prey and reset
        foreach (GameObject prey in preyList)
        {
            PreyAgent preyAgent = prey.GetComponent<PreyAgent>();
            if (preyAgent != null)
            {
                if (prey.activeSelf)
                {
                    preyAgent.AddReward(15f); // Reward for surviving
                }

                preyAgent.EndEpisode();
                prey.SetActive(true); // Ensure prey are active for next episode
            }
        }

        // End episodes for predators and reset
        foreach (GameObject predator in predatorList)
        {
            PredatorAgent predatorAgent = predator.GetComponent<PredatorAgent>();
            if (predatorAgent != null)
            {
                predatorAgent.EndEpisode();
            }
        }
    }
}
