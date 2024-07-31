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

    [SerializeField]
    private MusicManager musicManager = null;

    [SerializeField]
    private AudioSource BasketBallGameAudioSource = null;

    [SerializeField]
    private TextPrefabScript correctAnswerBox;


    public static LanguageHoopsManager shared;
    public Answer newWord = null;
    public BasketBallGameQuestionBoard QuestionBoard;
    public GameObject resultCard;
    public GameObject settingsCard;
    public TextPrefabScript scoreText;
    public TimerPrefab timer;
    public List<Material> basketballColors;
    public Spline spline;


    private int score = 0;
    private int numErrors = 0;

    private bool HoldingBall = false;
    private bool LaunchingBall = false;
    private bool isTouchActive = false;
    private bool isMoving = false;

    private int MaxStoredMovement = 10;
    private int CurrentStoredMovementIndex = 0;
    private List<Vector2> StoredMovement;
    private Vector2 initialTouchPosition;
    private float swipeStartTime;
    private float lastTouchTime;
    private float maxForceMagnitude = 5f;
    private float moveSpeed = 1.2f;
    private float splineProgress = 0f;
    Rigidbody ballRigidbody = null;

    private void Awake()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("musicVolume", 1f);
        Debug.Log("Game Started");
        module = FindAnyObjectByType<ModuleManager>();
        TermsList = module.terms;
        Debug.Log(TermsList.Count);
        shared = this;
        StoredMovement = new List<Vector2>(Enumerable.Repeat(Vector2.zero, MaxStoredMovement));
        StartCoroutine(APIManager.StartSession(module.currentModuleID));
        ballRigidbody = Ball.GetComponent<Rigidbody>();
        ballRigidbody.isKinematic = true;
        basketballColors = new List<Material>(CosmeticManager.basketballMaterial.materials);
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreText.Text = "Score: " + score;
        PlayNewWord();
    }

    private void Update()
    {

        if (isMoving)
        {
            splineProgress += Time.deltaTime * moveSpeed;
            if (splineProgress >= 1f)
            {
                splineProgress = 1f;
                isMoving = false;
            }
            if (isMoving)
            Ball.transform.position = spline.GetPoint(splineProgress);
        }

        if (BasketBallGameAudioSource.isPlaying)
        {
            musicManager.audioSource.volume = 0.1f;
        }
        else
        {
            musicManager.audioSource.volume = 0.8f;
        }

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

            // Debug.Log(Time.time - lastTouchTime);

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
                Vector2 finalTouchPositionvec3 = touch.position;
                float swipeDuration = Time.time - swipeStartTime;
                Vector2 swipeVector = (finalTouchPosition - initialTouchPosition) / swipeDuration;

                // Convert screen touch position to world position
                Vector3 worldStartPosition = Ball.transform.position;
                Vector3 worldEndPosition = arCamera.ScreenToWorldPoint(new Vector3(finalTouchPosition.x, finalTouchPosition.y, arCamera.nearClipPlane + 0.2f));

                // Debugging the touch positions and swipe vector

                // Convert screen swipe vector to world direction
               // Vector3 forceDirection = (worldEndPosition - worldStartPosition).normalized;
                Vector3 forceDirection = new Vector3(swipeVector.x, swipeVector.y, Mathf.Abs(swipeVector.y)).normalized;
                float verticalMultiplier = 0.38f; // Adjust this multiplier for more vertical force
                forceDirection.y *= verticalMultiplier;

                // Normalize the force direction again
                forceDirection = forceDirection.normalized;

                float forceMagnitude = swipeVector.magnitude * 0.01f; // Adjust the multiplier for appropriate force
                forceMagnitude = Mathf.Clamp(forceMagnitude, 0, maxForceMagnitude);
                Debug.Log("forceMagnitude:" + forceMagnitude);

                Vector3 velocity = forceDirection * forceMagnitude;


                if (forceMagnitude < 2.5)
                {
                    ballRigidbody.isKinematic = false; // Enable physics
                    ballRigidbody.velocity = Vector3.zero; // Reset velocity before applying force
                    ballRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity
                    var impulseForce = forceDirection * forceMagnitude;

                    ballRigidbody.AddForce(impulseForce, ForceMode.Impulse);
                }
                else
                {
                    Transform targetHoop = GetClosestHoopPosition(worldStartPosition, velocity);
                    Vector3 controlPoint = CalculateControlPoint(worldStartPosition, forceDirection, targetHoop.position);

                    spline.GenerateSpline(worldStartPosition, targetHoop.position, controlPoint);
                    isMoving = true;
                    splineProgress = 0f;
                    
                }

                HoldingBall = false;
                LaunchingBall = true;
                isTouchActive = false;
                Ball.PlaySound();

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

    Vector3 CalculateControlPoint(Vector3 start, Vector3 direction, Vector3 hoopPosition)
    {
        float controlHeight = Mathf.Abs(hoopPosition.y - start.y) / 2f + start.y;
        float controlOffset = direction.x * 0.1f;  // Adjust this value to change the curve

        return new Vector3(start.x + controlOffset, controlHeight +0.2f, 0.55f);
    }


    public void PlayAudioClip(AudioClip clip) => BasketBallGameAudioSource.PlayOneShot(clip);

    private Vector3 CalculateAimAssist(Vector3 forceDirection, float forceMagnitude)
    {
        const int numPoints = 3; // Number of trajectory points to simulate
        const float timeStep = 0.03f; // Time between each trajectory point

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

        // Vector3 closestHoopPosition = GetClosestHoopPosition(currentPosition);

        // Vector3 assistedDirection = (closestHoopPosition - Ball.transform.position).normalized;

        //  return new Vector3(assistedDirection.x, Mathf.Abs(assistedDirection.y), Mathf.Abs(assistedDirection.z));

        return Vector3.zero;
    }


    private Transform GetClosestHoopPosition(Vector3 StartPosition, Vector3 velocity, float distanceThreshold = 0.15f)
    {

        float timeStep = 0.01f;
        Vector3 currentPosition = StartPosition;
        List<Transform> hoops = new List<Transform>
        {
            HoopA.transform.Find("Hoop_02/Hoop_B/Hoop_C"),
            HoopB.transform.Find("Hoop_02/Hoop_B/Hoop_C"),
            HoopC.transform.Find("Hoop_02/Hoop_B/Hoop_C")
        };

        for (int i = 0; i < 4; i++)
        {
           currentPosition += velocity * timeStep;
           velocity += (Physics.gravity) * timeStep;

         //  GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
         //  sphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
         //  sphere.transform.position = currentPosition;
        }

        //Debug.Log($"Hoop A:{hoops[0].position}, Hoop B:{hoops[1].position}, Hoop C:{hoops[2].position}, predicted {currentPosition}");
        // Find the closest hoop based on the predicted landing position
        Transform closestHoop = hoops
            .OrderBy(hoop => Vector3.Distance(currentPosition, hoop.position))
            .FirstOrDefault();

        if (closestHoop == null)
            return null;

        var distance = Vector3.Distance(currentPosition, closestHoop.position);

        Debug.Log($"Closest hoop {closestHoop.parent.name}, Distance: {distance}, Threshold: {distanceThreshold}");

       // if (distance > distanceThreshold)
       //     return (0, 0, 0);

        return closestHoop.transform;
    }

    private void PlayNewWord()
    {
        Ball.transform.position = BallStartPosition.position;
        numErrors = 0;
        int randomIndex = 0;

        // Do not destroy the value this holds when modifying code
        newWord = AdaptiveLearning.GetNextAnswer(TermsList);
        TermsList.Remove(newWord);

        List<Answer> tempWords = new List<Answer>();
        Debug.Log("TermList.Count = " + TermsList.Count);
        for (int i = 0; i < 2; i++)
        {
            randomIndex = UnityEngine.Random.Range(0, TermsList.Count - 1);
            tempWords.Add(TermsList[randomIndex]);
            TermsList.RemoveAt(randomIndex);
        }

        tempWords.Insert(0, newWord); // Insert newWord at the start of tempWords

        List<BasketBallHoopPrefab> hoops = new List<BasketBallHoopPrefab>
        {
            HoopA, HoopB, HoopC
        };

        // Shuffle hoops to assign words randomly
        ShuffleList(tempWords);

        // Assign words to hoops
        // Add back terms to TermList
        for (int i = 0; i < hoops.Count; i++)
        {
            hoops[i].ConfigureHoop(tempWords[i].GetBack());
            TermsList.Add(tempWords[i]);
        }
        QuestionBoard.ConfigureWithWord(newWord);
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            T temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
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
        Time.timeScale = 1;
        // TODO: Add the summary functionality if needed
        // TODO: Make sure the loading of the scene is the correct scene GameScene?
        StartCoroutine(EndSession());
    }

    private IEnumerator EndSession()
    {

        yield return StartCoroutine(APIManager.EndSession(score));
        SceneSwapper.SwapSceneStatic("GamesPage");
    }

    private IEnumerator SendSingleALToDatabase(Answer currentTerm)
    {
        Debug.Log("Updating term: " + currentTerm.GetFront());
        string times = string.Join(",", currentTerm.GetPresentationTimes());
        yield return StartCoroutine(APIManager.UpdateAdaptiveLearningValue(currentTerm.GetTermID(), currentTerm.GetActivation(), currentTerm.GetDecay(), currentTerm.GetIntercept(), currentTerm.GetInitialTime(), times));
    }

    // Function that sends all Adaptive Learning values at once to the database. 

    private IEnumerator SendALToDatabase()
    {
        Time.timeScale = 1;
        foreach (Answer answer in TermsList)
        {
            string times = string.Join(",", answer.GetPresentationTimes());
            yield return StartCoroutine(APIManager.UpdateAdaptiveLearningValue(answer.GetTermID(), answer.GetActivation(), answer.GetDecay(), answer.GetIntercept(), answer.GetInitialTime(), times));
        }
        StartCoroutine(APIManager.EndSession(score));
        SceneSwapper.SwapSceneStatic("GamesPage");
    }

    public void ShowSettings()
    {
        // TODO: Show the Settings Prefab 
        // Ex: SetActive call on a settings prefab

        Time.timeScale = 0;
        settingsCard.SetActive(true);
        settingsCard.GetComponent<Animator>().SetTrigger("SlideIn");
    }

    public void UnPause()
    {
        Time.timeScale = 1;
        StartCoroutine(UnpauseAnimation());
    }

    private IEnumerator UnpauseAnimation()
    {
        settingsCard.GetComponent<Animator>().SetTrigger("SlideOut");
        yield return new WaitForSeconds(1.5f);
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
            enteredHoop.PlaySound();
            score++;
            StoreManager.AddCoins(1);
            StartCoroutine(APIManager.LogAnswer(newWord.GetTermID(), true));
            scoreText.Text = "Score: " + score;
            isCorrect = true;
            AdaptiveLearning.CalculateDecayContinuous(newWord, true, responseTime);
            AdaptiveLearning.CalculateActivationValue(newWord);
            StartCoroutine(SendSingleALToDatabase(newWord));
            PlayNewWord();
            ResetBall();
        }
        else
        {
            if (numErrors == 0)
            {
                isCorrect = false;
                numErrors++;
                ResetBall();
            }
            else
            {
                StartCoroutine(OnAnswerIncorrect());
                //TODO: Add logic for giving correct answer after second incorrect guess
                AdaptiveLearning.CalculateDecayContinuous(newWord, false, responseTime);
                AdaptiveLearning.CalculateActivationValue(newWord);
                StartCoroutine(APIManager.LogAnswer(newWord.GetTermID(), false));
                StartCoroutine(SendSingleALToDatabase(newWord));
                ResetBall();
                PlayNewWord();
                isCorrect = false;
            }
        }
    }

    private IEnumerator OnAnswerIncorrect()
    {
        correctAnswerBox.Text = "The correct answer is: " + newWord.GetBack();
        correctAnswerBox.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        correctAnswerBox.gameObject.SetActive(false);
    }

    public void PlayAgain()
    {
        Time.timeScale = 1;
        resultCard.SetActive(false);
        SceneSwapper.SwapSceneStatic("BasketballGame");
    }
}