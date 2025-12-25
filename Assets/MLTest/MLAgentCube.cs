using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class MLAgentCube : Agent
{
    [Header("������")]
    public Transform target;      // ����, �� ������� ����� ����� ���������
    public Rigidbody rb;          // Rigidbody ����

    [Header("���������")]
    public float forceMultiplier = 10f; // ����, ������� ����� ����� ���������
    public float moveRange = 5f;        // �������� ���������� ��������� ����

    private Vector3 startPos;

    public override void Initialize()
    {
        startPos = transform.position;

        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }


    public override void OnEpisodeBegin()
    {
        // ����� ������� � �������� ����
        transform.position = startPos;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // ��������� ������� ����
        target.position = new Vector3(
            Random.Range(-moveRange, moveRange),
            target.position.y, // ��������� ������ ����
            Random.Range(-moveRange, moveRange)
        );
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // ������ � ����
        Vector3 toTarget = target.position - transform.position;

        // ��������� ����������
        sensor.AddObservation(transform.position);   // ������� ����
        sensor.AddObservation(rb.linearVelocity);          // �������� ����
        sensor.AddObservation(toTarget.normalized);  // ����������� �� ����
        sensor.AddObservation(toTarget.magnitude);   // ���������� �� ����
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // �������� ������: 2 ��������, ��� �������� �� X � Z
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        Vector3 force = new Vector3(moveX, 0f, moveZ) * forceMultiplier;
        rb.AddForce(force);

        // �������: ��� ����� � ����, ��� �����
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        AddReward(-distanceToTarget * 0.01f); // ����� �� ����������

        // ����� �� ���������� ����
        if (distanceToTarget < 1.0f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // ����� �� �������
        if (transform.position.y < -4f)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // ��� ����� ������� ����� ������� WASD
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}

