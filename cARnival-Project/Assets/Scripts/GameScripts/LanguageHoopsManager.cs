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

    [SerializeField]
    private GameObject trajectoryPointPrefab;
    private List<GameObject> trajectoryPoints = new List<GameObject>();


    public static LanguageHoopsManager shared;
    public Answer newWord = null;
    public FishingGameQuestionBoard QuestionBoard;
    public GameObject incorrectCard;
    public GameObject resultCard;
    public GameObject settingsCard;
    public TextPrefabScript scoreText;
    public TimerPrefab timer;
    public List<Material> basketballColors;

    private int score = 0;
    private int numErrors = 0;

    private bool HoldingBall = false;
    private bool LaunchingBall = false;
    private bool isTouchActive = false;

    private int MaxStoredMovement = 10;
    private int CurrentStoredMovementIndex = 0;
    private List<Vector2> StoredMovement;
    private Vector2 initialTouchPosition;
    private float swipeStartTime;
    private float lastTouchTime;
    private float maxForceMagnitude = 6f;

    private void Awake()
    {
        Debug.Log("Game Started");
        module = FindAnyObjectByType<ModuleManager>();
        TermsList = module.terms;
        Debug.Log(TermsList.Count);
        shared = this;
        StoredMovement = new List<Vector2>(Enumerable.Repeat(Vector2.zero, MaxStoredMovement));
        StartCoroutine(APIManager.StartSession(module.currentModuleID));
        Rigidbody ballRigidbody = Ball.GetComponent<Rigidbody>();
        ballRigidbody.isKinematic = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreText.Text = "Score: " + score;
        PlayNewWord();
    }

    private void Update()
    {

        if (Ball.transform.position.y < -0.3f)
            ResetBall();

        if (timer.timeLeft < 0)
        {
            Time.timeScale = 0;
            resultCard.SetActive(true);
        }
        if (HoldingBall && !LaunchingBall && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            lastTouchTime = Time.time;

            Debug.Log(Time.time - lastTouchTime);

            if (!isTouchActive && (Time.time - lastTouchTime) < 0.1f)
            {
                initialTouchPosition = touch.position;
                swipeStartTime = Time.time;
                StoredMovement = new List<Vector2>(Enumerable.Repeat(Vector2.zero, MaxStoredMovement));
                CurrentStoredMovementIndex = 0;
                isTouchActive = true;
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

                Vector3 worldPoint = arCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, arCamera.nearClipPlane + 0.2f));
                Ball.transform.position = worldPoint;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector2 finalTouchPosition = touch.position;
                float swipeDuration = Time.time - swipeStartTime;
                Vector2 swipeVector = (finalTouchPosition - initialTouchPosition) / swipeDuration;
                
                // Debugging the touch positions and swipe vector

                // Convert screen swipe vector to world direction
                Vector3 forceDirection = new Vector3(swipeVector.x, swipeVector.y, Mathf.Abs(swipeVector.y)).normalized;
                float verticalMultiplier = 1f; // Adjust this multiplier for more vertical force
                forceDirection.y *= verticalMultiplier;

                // Normalize the force direction again
                forceDirection = forceDirection.normalized;

                float forceMagnitude = swipeVector.magnitude * 0.01f; // Adjust the multiplier for appropriate force
                forceMagnitude = Mathf.Clamp(forceMagnitude, 0, maxForceMagnitude);

                // Aim assist logic
                //Debug.Log("AIM ASSUST");
                forceDirection = CalculateAimAssist(forceDirection, forceMagnitude);

                // Set the Rigidbody to non-kinematic before applying force
                Rigidbody ballRigidbody = Ball.GetComponent<Rigidbody>();
                ballRigidbody.isKinematic = false; // Enable physics
                ballRigidbody.velocity = Vector3.zero; // Reset velocity before applying force
                ballRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity

                var impulseForce = forceDirection * forceMagnitude;

              //  if (impulseForce.y < 2.5f)
              //  {
             //      ResetBall();
             //       return;
             //   }

                impulseForce.x = Mathf.Clamp(impulseForce.x, -2f, 2f);

                // if Y force is less than 2.5 then reset the ball 
                // clamp x force to be between -2 and 2

                Debug.Log($"BasketBallGame -- ForceApplied: {impulseForce}");

                ballRigidbody.AddForce(impulseForce, ForceMode.Impulse);

                HoldingBall = false;
                LaunchingBall = true;
                isTouchActive = false;
            }
        }
        else
        {
            isTouchActive = false;
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

                    if (hitObject.collider != null && hitObject.collider.gameObject.CompareTag("ball"))
                    {

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
        const int numPoints = 3; // Number of trajectory points to simulate
        const float timeStep = 0.05f; // Time between each trajectory point

        Vector3 currentPosition = Ball.transform.position;
        Vector3 velocity = forceDirection * forceMagnitude / (Ball.GetComponent<Rigidbody>().mass + 0.2f);
        Vector3 gravity = Physics.gravity;
        RaycastHit hit;

        // Layer mask to ignore everything except hoops
        int layerMask = LayerMask.GetMask("Default");

        for (int i = 0; i < numPoints; i++)
        {
            Vector3 nextPosition = currentPosition + velocity * timeStep;

            // Instantiate a trajectory point at the simulated position
            //GameObject trajectoryPoint = Instantiate(trajectoryPointPrefab, nextPosition, Quaternion.identity);
            //trajectoryPoints.Add(trajectoryPoint);

            if (Physics.Raycast(currentPosition, nextPosition - currentPosition, out hit, (nextPosition - currentPosition).magnitude, layerMask))
            {
                Debug.Log($"Aim assist collision detected with {hit.collider.name} at point {hit.point}");
                // Adjust the aim towards this object
                return (hit.point - Ball.transform.position).normalized;
            }

            currentPosition = nextPosition;
            velocity += gravity * timeStep;
        }

        Vector3 closestHoopPosition = GetClosestHoopPosition(currentPosition);

        Vector3 assistedDirection = (closestHoopPosition - Ball.transform.position).normalized;

        return new Vector3(assistedDirection.x, Mathf.Abs(assistedDirection.y), Mathf.Abs(assistedDirection.z));
    }


    private Vector3 GetClosestHoopPosition(Vector3 predictedLandingPosition, float distanceThreshold = 0.15f)
    {
        List<Transform> hoops = new List<Transform>
        {
            HoopA.transform.Find("Hoop_02"),
            HoopB.transform.Find("Hoop_02"),
            HoopC.transform.Find("Hoop_02")
        };

        Debug.Log($"Hoop A:{hoops[0].position}, Hoop B:{hoops[1].position}, Hoop C:{hoops[2].position}, predicted {predictedLandingPosition}");
        // Find the closest hoop based on the predicted landing position
        Transform closestHoop = hoops
            .OrderBy(hoop => Vector3.Distance(predictedLandingPosition, hoop.position))
            .FirstOrDefault();

        if (closestHoop == null)
            return Vector3.zero;

        var distance = Vector3.Distance(predictedLandingPosition, closestHoop.position);

        Debug.Log($"Closest hoop {closestHoop.parent.name}, Distance: {distance}, Threshold: {distanceThreshold}");

        if (distance > distanceThreshold)
            return Vector3.zero;

        return closestHoop.transform.position;
    }

    private void PlayNewWord()
    {
        Ball.transform.position = BallStartPosition.position;

        int randomIndex = 0;

        // Do not destroy the value this holds when modifying code
        newWord = AdaptiveLearning.GetNextAnswer(TermsList);
        TermsList.Remove(newWord);

        List<Answer> tempWords = new List<Answer>();
        Debug.Log("TermList.Count = " + TermsList.Count);
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
        // Add back terms to TermList
        for (int i = 0; i < hoops.Count; i++)
        {
            hoops[i].ConfigureHoop(tempWords[i].GetBack());
            TermsList.Add(tempWords[i]);
        }
        TermsList.Add(newWord);
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

    public void ResetBall()
    {
        Ball.transform.position = BallStartPosition.position;
        Rigidbody ballRigidbody = Ball.GetComponent<Rigidbody>();
        ballRigidbody.velocity = Vector3.zero; // Reset velocity
        ballRigidbody.isKinematic = true; // Reset the ball to kinematic state
        HoldingBall = false;
        LaunchingBall = false;
    }

    public void QuitGame()
    {
        // TODO: Add the summary functionality if needed
        // TODO: Make sure the loading of the scene is the correct scene GameScene?
        StartCoroutine(APIManager.EndSession(score));
        SceneSwapper.SwapSceneStatic("GamesPage");
    }

    public void ShowSettings()
    {
        // TODO: Show the Settings Prefab 
        // Ex: SetActive call on a settings prefab

        // TODO: Pause the game
        Time.timeScale = 0;
        settingsCard.SetActive(true);
    }

    public void UnPause()
    {
        Time.timeScale = 1;
        settingsCard.SetActive(false);
    }

    public void SelectWord(BasketBallHoopPrefab enteredHoop, out bool isCorrect)
    {
        //TODO: Add logic to check for response time.
        var responseTime = 0;
        isCorrect = false;
        Debug.Log(enteredHoop.Text.Text);
        Debug.Log(newWord);
        if (enteredHoop.Text.Text == newWord.GetBack())
        {
            score++;
            StoreManager.AddCoins(1);
            StartCoroutine(APIManager.LogAnswer(newWord.GetTermID(), true));
            scoreText.Text = "Score: " + score;
            isCorrect = true;
            AdaptiveLearning.CalculateDecayContinuous(newWord, true, responseTime);
            AdaptiveLearning.CalculateActivationValue(newWord);

            PlayNewWord();
            ResetBall();
        }
        else
        {
            if (numErrors == 0)
            {
                numErrors++;
                StartCoroutine(ShowIncorrectCard());
                isCorrect = false;

            }
            //TODO: Add logic for giving correct answer after second incorrect guess
            else if (numErrors > 0)
            {
                AdaptiveLearning.CalculateDecayContinuous(newWord, false, responseTime);
                AdaptiveLearning.CalculateActivationValue(newWord);
                StartCoroutine(APIManager.LogAnswer(newWord.GetTermID(), false));

                PlayNewWord();
                isCorrect = false;
                numErrors = 0;
            }
        }
    }

    private IEnumerator ShowIncorrectCard()
    {
        Time.timeScale = 0;
        incorrectCard.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        incorrectCard.SetActive(false);
        Time.timeScale = 1;
        ResetBall();
    }

    public void PlayAgain()
    {
        Time.timeScale = 1;
        resultCard.SetActive(false);
        SceneSwapper.SwapSceneStatic("BasketballGame");
    }
}