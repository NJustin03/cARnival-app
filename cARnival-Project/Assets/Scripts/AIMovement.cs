using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    public Vector3 centerPoint;
    public float rangeRadius; 

    private int currentWaypointIndex = 0;
    private NavMeshAgent duckAgent;
    private bool canMove = true;
    private Vector3 resultDestination;


    void Start()
    {
        duckAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(Move());
    }

    void OnEnable()
    {
        // Start the coroutine when the GameObject is enabled
        StartCoroutine(Move());
    }

    void OnDisable()
    {
        // Stop all coroutines when the GameObject is disabled
        StopAllCoroutines();
    }

    void Update()
    {
        if (canMove)
        {
            StartCoroutine(Move());
            canMove = false;

        }
    }

    private IEnumerator Move()
    {
        if (RandomPoint(centerPoint, rangeRadius, out Vector3 randomPoint))
        {
            resultDestination = randomPoint;
            duckAgent.SetDestination(resultDestination);
        }
        yield return new WaitForSeconds(3f);
        canMove = true;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 randomPointInUnitCircle2D = Random.insideUnitCircle;
            Vector3 randomPointInUnitCircle3D = new Vector3(randomPointInUnitCircle2D.x, 0, randomPointInUnitCircle2D.y);
            Vector3 randomPoint = center + randomPointInUnitCircle3D * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}
