using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Transform centerPoint;
    public float rotateSpeedMin;
    public float rotateSpeedMax;
    private float rotateSpeed;
    private Vector3 point;
    private bool isAnswerCorrect;

    public TextPrefabScript text;
    private ArcheryManager archeryManager;

    public ParticleSystem particleEffect;

    // Start is called before the first frame update
    void Awake()
    {
        archeryManager = FindAnyObjectByType<ArcheryManager>();
        point = centerPoint.position;
        transform.LookAt(point);
        particleEffect = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(point, new Vector3(0.0f, 1.0f, 0.0f), 20 * Time.deltaTime * rotateSpeed);
        transform.LookAt(point);
    }

    public void ResetTarget(string newWord, bool correctAnswer)
    {
        rotateSpeed = Random.Range(rotateSpeedMin, rotateSpeedMax);
        gameObject.transform.SetPositionAndRotation(RandomPoint(), Quaternion.identity);
        text.Text = newWord;
        isAnswerCorrect = correctAnswer;
    }

    private Vector3 RandomPoint()
    {
        Vector2 randomPointInUnitCircle2D = Random.insideUnitCircle;
        return new Vector3((randomPointInUnitCircle2D.x * 1.5f) + 1, Random.value, (randomPointInUnitCircle2D.y * 1.5f) + 1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        archeryManager.ChooseAnswer(isAnswerCorrect);
        particleEffect.Play();
    }
}
