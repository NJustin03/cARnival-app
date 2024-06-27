using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Answer
{
    private int termID;
    private float activation;
    private float decay;
    private float intercept;
    private AudioClip answerAudio;
    private Texture2D answerImage;
    private string front;
    private string back;
    private string type;
    private string gender;
    private string language;
    private string audioLocation;
    private string imageLocation;
    private string presentationTimes;

    private string initialTime;

    public bool hasImage;
    public bool hasAudio;

    // Various constructors for whether or not there is an image or audio attached to the answer.

    public Answer(int termID, string front, string back, string type, string gender, string language, string audioLocation, string imageLocation)
    {
        this.termID = termID;
        answerAudio = null;
        answerImage = null;
        this.front = front;
        this.back = back;
        this.type = type;
        this.gender = gender;
        this.language = language;
        this.audioLocation = audioLocation;
        // this.activation = activation;
        // this.decay = decay;
        // this.presentationTimes = presentationTimes;
        // this.initialTime = date;

        if (audioLocation.Length == 0 )
        {
            hasAudio = false;
        }

        this.imageLocation = imageLocation;
        if (imageLocation.Length == 0 )
        {
            hasImage = false;
        }


    }
    public List<int> GetPresentationTimes()
    {
        if (string.IsNullOrEmpty(presentationTimes))
        {
            return new List<int>();
        }
        return presentationTimes.Split(',').Select(int.Parse).ToList();
    }

    public void AddPresentationTime(int time)
    {
        var times = GetPresentationTimes();
        times.Add(time);

        if (times.Count() > 5000)
            times.RemoveAt(0);

        presentationTimes = string.Join(",", times);
    }

    public void SetAnswerAudio(AudioClip answerAudio)
    { this.answerAudio = answerAudio; }

    public void SetAnswerImage(Texture2D answerImage)
    { this.answerImage = answerImage; }

    public void SetActivation(float activationValue)
    { this.activation = activationValue; }

    public void SetDecay(float decay)
    { this.decay = decay; }

    public void SetIntercept(float alpha)
    { this.intercept = alpha; }

    public void SetInitialTime(string initialTime)
    { this.initialTime = initialTime; }

    public int GetTermID()
    { return termID; }

    public string GetFront() 
    { return front; }

    public string GetBack()
    { return back; }

    public string GetAnswerType()
    { return type; }

    public string GetGender()
    { return gender; }

    public string GetLanguage() 
    { return language; }

    public string GetImageLocation()
    { return imageLocation; }

    public string GetAudioLocation()
    { return audioLocation; }

    public Texture2D GetImage()
    { return answerImage; }

    public AudioClip GetAudio() 
    { return answerAudio; }

    public float GetActivation()
    { return activation; }

    public float GetDecay()
    { return decay; }

    public string GetInitialTime()
    { return initialTime; }

    public float GetIntercept()
    { return intercept; }

    override
    public string ToString()
    {
        return "Term ID: " + termID + "\n Term Front: " + front + "\n Term Back: " + back + "\n";
    }

    public void SetAdaptiveValues(AdaptiveValuesJson adapt)
    {
        activation = adapt.activation_val;
        decay = adapt.decay_val;
        intercept = adapt.alpha_val;
        presentationTimes = adapt.times;
        initialTime = adapt.dates;
    }
}
