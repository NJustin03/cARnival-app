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

    public SpawnResultText spawnText;

    // Find the manager, audio, and particle effect.
    void Awake()
    {
        archeryManager = FindAnyObjectByType<ArcheryManager>();
        particleEffect = CosmeticManager.archeryParticle.particles;
        audioSource = GetComponent<AudioSource>();
    }

    // Function which assigns a target a term/answer and whether or not they were the correct term.
    public void ResetTarget(Answer answer, bool correctAnswer)
    {
        currentAnswer = answer;
        text.Text = answer.GetBack();
        isAnswerCorrect = correctAnswer;
    }

    // Function which on target impact, plays particles if the correct answer was chosen and starts the sequence to reset the targets.
    public void OnImpact()
    {
        if (isAnswerCorrect)
        {
            ParticleSystem temp = Instantiate(particleEffect, transform.position, Quaternion.identity);
            temp.Play();
            audioSource.Play();
            spawnText.AnsweredCorrect(transform.position);
        }
        else
        {
            spawnText.AnsweredIncorrect(transform.position);
        }
        archeryManager.ChooseAnswer(isAnswerCorrect);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isAnswerCorrect)
        {
            ParticleSystem temp = Instantiate(particleEffect, transform.position, Quaternion.identity);
            temp.Play();
            spawnText.AnsweredCorrect(transform.position);
        }
        else
        {
            spawnText.AnsweredIncorrect(transform.position);
        }
        archeryManager.ChooseAnswer(isAnswerCorrect);
    }
}
