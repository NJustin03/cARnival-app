using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private NavMeshAgent duckAgent;
    private Vector3 direction;
    private bool canMove = true;

    void Start()
    {
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
        duckAgent = GetComponent<NavMeshAgent>();
        duckAgent.updateUpAxis = false;
    }

    void Update()
    {
        //MoveTowardsWaypoint();
        if (canMove)
        {
            StartCoroutine(Move());
            canMove = false;
        }
        //Quaternion rot = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Lerp(transform.rotation, rot, 1f * Time.deltaTime);
        

    }

    private IEnumerator Move()
    {
        Vector3 temp = waypoints[Random.Range(0, waypoints.Length - 1)].position;
        duckAgent.SetDestination(new Vector3(temp.x, transform.position.y, temp.z));
        direction = duckAgent.destination - transform.position;
        yield return new WaitForSeconds(3f);
        canMove = true;
    }

    void MoveTowardsWaypoint()
    {
        if (waypoints != null)
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector3 direction = targetWaypoint.position - transform.position;
            transform.position += direction.normalized * duckAgent.speed * Time.deltaTime;
            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                currentWaypointIndex = Random.Range(0, waypoints.Length - 1);
            }
        }
    }
}
