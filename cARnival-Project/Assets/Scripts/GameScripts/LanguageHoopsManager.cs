using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class LanguageHoopsManager : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private ModuleManager module;

    [SerializeField]
    private List<Answer> TermsList;

    [SerializeField]
    private BasketBallHoopPrefab HoopA, HoopB, HoopC;

    [SerializeField]
    private BasketBallPrefab Ball;

    [SerializeField]
    private Transform BallStartPosition;

    public static LanguageHoopsManager shared;
    public Answer newWord = null;
    public FishingGameQuestionBoard QuestionBoard;

    private bool HoldingBall = false;
    private bool LaunchingBall = false;

    private int MaxStoredMovement = 10;
    private int CurrentStoredMovementIndex = 0;
    private List<Vector2> StoredMovement;
    private Vector2 initialTouchPosition;
    private float swipeStartTime;

    private void Awake()
    {
        module = FindAnyObjectByType<ModuleManager>();
        TermsList = module.terms;
        Debug.Log(TermsList.Count);
        shared = this;
        StoredMovement = new List<Vector2>(Enumerable.Repeat(Vector2.zero, MaxStoredMovement));

        Rigidbody ballRigidbody = Ball.GetComponent<Rigidbody>();
        ballRigidbody.isKinematic = true;
    }

    // Start is called before the first frame update
    void Start()
    {
       PlayNewWord();
    }

    private void Update()
    {
        if (HoldingBall && !LaunchingBall && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                initialTouchPosition = touch.position;
                swipeStartTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                // Store or update touch deltaPosition in StoredMovement
                if (CurrentStoredMovementIndex >= StoredMovement.Count)
                {
                    StoredMovement.Add(touch.deltaPosition);
                }
                else
                {
                    StoredMovement[CurrentStoredMovementIndex] = touch.deltaPosition;
                }

                CurrentStoredMovementIndex = (CurrentStoredMovementIndex + 1) % MaxStoredMovement;

                Vector3 worldPoint = arCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, arCamera.nearClipPlane + 1));
                Ball.transform.position = worldPoint;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector2 finalTouchPosition = touch.position;
                float swipeDuration = Time.time - swipeStartTime;
                Vector2 swipeVector = (finalTouchPosition - initialTouchPosition) / swipeDuration;

                // Convert screen swipe vector to world direction
                Vector3 forceDirection = new Vector3(swipeVector.x, swipeVector.y, Mathf.Abs(swipeVector.y)).normalized;
                float verticalMultiplier = 1.5f; // Adjust this multiplier for more vertical force
                forceDirection.y *= verticalMultiplier;

                float forceMagnitude = swipeVector.magnitude * 0.01f; // Adjust the multiplier for appropriate force

                // Aim assist logic
                forceDirection = CalculateAimAssist(forceDirection, forceMagnitude);

                // Debug the calculated force
                Debug.Log($"Force direction: {forceDirection}, magnitude: {forceMagnitude}");

                // Set the Rigidbody to non-kinematic before applying force
                Rigidbody ballRigidbody = Ball.GetComponent<Rigidbody>();
                ballRigidbody.isKinematic = false; // Enable physics
                ballRigidbody.velocity = Vector3.zero; // Reset velocity before applying force
                ballRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity

                ballRigidbody.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
                Debug.Log($"Applied force: {forceDirection * forceMagnitude}");

                HoldingBall = false;
                LaunchingBall = true;
            }
        }

        // Check if the ball is picked up
        if (!HoldingBall && !LaunchingBall && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;

                if (Physics.Raycast(ray, out hitObject))
                {
                    Debug.Log($"Raycast hit: {hitObject.collider.name}");
                    if (hitObject.collider != null && hitObject.collider.gameObject.CompareTag("ball"))
                    {
                        Debug.Log("Ball tapped");
                        HoldingBall = true;
                        Rigidbody ballRigidbody = Ball.GetComponent<Rigidbody>();
                        ballRigidbody.isKinematic = true; // Disable physics while holding
                    }
                }
                else
                {
                    Debug.Log("Raycast did not hit any object");
                }
            }
        }
    }




    private Vector3 CalculateAimAssist(Vector3 forceDirection, float forceMagnitude)
    {
        const int numPoints = 30; // Number of trajectory points to simulate
        const float timeStep = 0.1f; // Time between each trajectory point

        Vector3 currentPosition = Ball.transform.position;
        Vector3 velocity = forceDirection * forceMagnitude / Ball.GetComponent<Rigidbody>().mass;
        Vector3 gravity = Physics.gravity;
        RaycastHit hit;

        for (int i = 0; i < numPoints; i++)
        {
            Vector3 nextPosition = currentPosition + velocity * timeStep;
            if (Physics.Raycast(currentPosition, nextPosition - currentPosition, out hit, (nextPosition - currentPosition).magnitude))
            {
                Debug.Log($"Aim assist collision detected with {hit.collider.name}");
                // Adjust the aim towards this object
                return (hit.point - Ball.transform.position).normalized;
            }

            currentPosition = nextPosition;
            velocity += gravity * timeStep;
        }

        return forceDirection; // Return the original direction if no collisions detected
    }


    private Vector3 GetClosestHoopPosition(Vector3 predictedLandingPosition)
    {
        List<BasketBallHoopPrefab> hoops = new List<BasketBallHoopPrefab> { HoopA, HoopB, HoopC };

        // Find the closest hoop based on the predicted landing position
        BasketBallHoopPrefab closestHoop = hoops
            .OrderBy(hoop => Vector3.Distance(predictedLandingPosition, hoop.transform.position))
            .FirstOrDefault();

        return closestHoop != null ? closestHoop.transform.position : Vector3.zero;
    }

    private void PlayNewWord()
    {
        Ball.transform.position = BallStartPosition.position;

        int randomIndex = 0;

        // Do not destroy the value this holds when modifying code
        newWord = AdaptiveLearning.GetNextAnswer(TermsList);
        TermsList.Remove(newWord);

        List<Answer> tempWords = new List<Answer>();
        for (int i = 0; i <= 2; i++)
        {
            randomIndex = UnityEngine.Random.Range(0, TermsList.Count);
            tempWords.Add(TermsList[randomIndex]);
            TermsList.RemoveAt(randomIndex);
        }

        tempWords.Insert(0, newWord); // Insert newWord at the start of tempWords

        List<BasketBallHoopPrefab> hoops = new List<BasketBallHoopPrefab>
        {
            HoopA, HoopB, HoopC
        };

        // Shuffle hoops to assign words randomly
        ShuffleList(hoops);

        // Assign words to hoops
        for (int i = 0; i < hoops.Count; i++)
        {
            hoops[i].ConfigureHoop(tempWords[i].GetBack());
        }

        QuestionBoard.ConfigureWithWord(newWord);
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("boundary"))
        {
            Debug.Log("Ball hit the boundary. Resetting position.");
           // BallHitBoundary();
        }
    }
    public void BallHitBoundary()
    {
        Ball.transform.position = BallStartPosition.position;
        Rigidbody ballRigidbody = Ball.GetComponent<Rigidbody>();
        ballRigidbody.velocity = Vector3.zero; // Reset velocity
        ballRigidbody.isKinematic = true; // Reset the ball to kinematic state
        HoldingBall = false;
        LaunchingBall = false;
    }
}
