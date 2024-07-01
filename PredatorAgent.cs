using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PredatorAgent : Agent
{
    [SerializeField] public float moveSpeed;
    [SerializeField] public float rotateSpeed;
    [SerializeField] public WorldManager worldManager;


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.forward);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Get the discrete actions
        int moveAction = actions.DiscreteActions[0];
        int rotateAction = actions.DiscreteActions[1];

        Vector3 moveDir = Vector3.zero;
        float rotation = 0f;

        // Move forward/backward
        if (moveAction == 1)
        {
            moveDir = transform.forward * moveSpeed;
        }
        else if (moveAction == 2)
        {
            moveDir = -transform.forward * moveSpeed * 0.5f;
        }

        // Rotate left/right
        if (rotateAction == 1)
        {
            rotation = -rotateSpeed;
        }
        else if (rotateAction == 2)
        {
            rotation = rotateSpeed;
        }

        // Apply movement and rotation
        transform.position += moveDir * Time.deltaTime;
        transform.Rotate(0, rotation * Time.deltaTime, 0);

        AddReward(-0.01f); // Penalize for each step
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions.Clear(); // Ensure all actions are reset to 0

        // Set discrete actions based on player input
        if (Input.GetKey(KeyCode.W)) // Move forward
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S)) // Move backward
        {
            discreteActions[0] = 2;
        }
        else
        {
            discreteActions[0] = 0; // No movement
        }

        if (Input.GetKey(KeyCode.A)) // Rotate left
        {
            discreteActions[1] = 1;
        }
        else if (Input.GetKey(KeyCode.D)) // Rotate right
        {
            discreteActions[1] = 2;
        }
        else
        {
            discreteActions[1] = 0; // No rotation
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PreyAgent")
        {
            AddReward(15f);

            PreyAgent preyAgent = collision.gameObject.GetComponent<PreyAgent>();
            if (preyAgent != null)
            {
                preyAgent.OnHit();
            }

        }
    }


    public override void OnEpisodeBegin()
    {
        if (worldManager == null) return;
        Vector3 position = new Vector3(Random.Range(-worldManager.coordinateLimit, worldManager.coordinateLimit), 1, Random.Range(-worldManager.coordinateLimit, worldManager.coordinateLimit));
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        transform.rotation = rotation;
        transform.localPosition = position;
    }
}
