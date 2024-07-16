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
    public Answer currentAnswer;

    // Start is called before the first frame update
    void Awake()
    {
        archeryManager = FindAnyObjectByType<ArcheryManager>();
        point = centerPoint.position;
        transform.LookAt(point);
        particleEffect = CosmeticManager.archeryParticle.particles;
    }

    public void ResetTarget(Answer answer, bool correctAnswer)
    {
        currentAnswer = answer;
        text.Text = answer.GetBack();
        isAnswerCorrect = correctAnswer;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (isAnswerCorrect)
        {
            ParticleSystem temp = Instantiate(particleEffect, transform.position, Quaternion.identity);
            temp.Play();
        }
        Debug.Log("Here");
        archeryManager.ChooseAnswer(isAnswerCorrect, currentAnswer);
    }
}
