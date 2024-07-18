using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    private bool isAnswerCorrect;

    public TextPrefabScript text;
    private ArcheryManager archeryManager;
    private AudioSource audioSource;

    public ParticleSystem particleEffect;
    public Answer currentAnswer;

    // Start is called before the first frame update
    void Awake()
    {
        archeryManager = FindAnyObjectByType<ArcheryManager>();
        particleEffect = CosmeticManager.archeryParticle.particles;
        audioSource = GetComponent<AudioSource>();
    }

    public void ResetTarget(Answer answer, bool correctAnswer)
    {
        currentAnswer = answer;
        text.Text = answer.GetBack();
        isAnswerCorrect = correctAnswer;
    }

    public void OnImpact()
    {
        if (isAnswerCorrect)
        {
            ParticleSystem temp = Instantiate(particleEffect, transform.position, Quaternion.identity);
            temp.Play();
            audioSource.Play();
        }
        archeryManager.ChooseAnswer(isAnswerCorrect, currentAnswer);
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
