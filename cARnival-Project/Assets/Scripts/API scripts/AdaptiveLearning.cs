using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// AdapativeLearning is a class with helper functions to apply AdaptiveLearning algorithms
/// to the AnimELLE Crossing: cARnival suite of language learning games. This class
/// works solely with the Answer object and will update the Decay and ActivationValues 
/// on the Answer object.
/// </summary>
public class AdaptiveLearning
{
    /// <summary>
    /// The c value is a fixed scaling parameter used in calulating an Answer's Decay value.
    /// </summary>
    private const float c = 0.25f;

    /// <summary>
    /// A value, when exceeded, will determine whether or not an unseen Answer needs to be presented to the player.
    /// Needs some testing to come to a conclusion on this value.
    /// </summary>
    private const float ActivationThreshold = 1f;

    /// <summary>
    /// A constant to represent the default activation threshold for an unseen Answer.
    /// </summary>
    private const float UnseedWordActivation = Mathf.NegativeInfinity;

    /// <summary>
    /// Constant to represent the base of natural log
    /// </summary>
    private const float e = MathF.E;

    /// <summary>
    /// Determines the Answer from a list of Answers that needs the most practice. This is determined by comparing the activation value of
    /// each Answer and selecting the Answer with the lowest activation value that under exceeds the ActivationThreshold. If no Answer
    /// under exceeds the ActivationThreshold, then an unseen Answer will be randomly selected from the list of Answers.
    /// When all Answers have been seen and no Answers have activation values that exceed the ActivationThreshold then the Answer with the
    /// lowest activation value will be returned.
    /// 
    /// When an unseen Answer is selected by the calculations, then the InitialTime will be set on the Answer Object
    /// </summary>
    /// <param name="answers">A list of answers to select the next answer from.</param>
    /// <returns>The answer that is determined to need the most practice</returns>
    public static Answer GetNextAnswer(List<Answer> answers)
    {
        if (answers.Count == 0)
            return null;

        float currentActivationValue = Mathf.Infinity;
        Answer nextAnswer = answers.FirstOrDefault();

        var unseenAnswers = new List<Answer>();

        // Find the answer with the lowest ActivationValue
        foreach (var answer in answers)
        {
            var activationValue = answer.GetActivation();

            if (activationValue == UnseedWordActivation)
            {
                unseenAnswers.Add(answer);
                continue;
            }

            if (activationValue < currentActivationValue)
            {
                currentActivationValue = activationValue;
                nextAnswer = answer;
            }
        }

        if (nextAnswer.GetActivation() > ActivationThreshold && unseenAnswers.Count() > 0)
        {
            var randomIndex = UnityEngine.Random.Range(0, unseenAnswers.Count() - 1);
            nextAnswer = unseenAnswers[randomIndex];
            nextAnswer.SetInitialTime(DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"));
        }

        return nextAnswer;
    }

    public static void SelectAnswer(Answer answer, bool isCorrect, float responseTime, float fixedTimeCost = 300f)
    {
        CalculateDecay(answer, isCorrect, responseTime, fixedTimeCost);
        CalculateActivationValue(answer);
    }

    /// <summary>
    /// Calculates the Decay value for a given Answer. The calculated decay will be different whether or not the Answer was correct
    /// or incorrect as well as within the proper response time. An incorrect selection will have a change in the decay of 0.01 while
    /// a correct answer will change the decay on a range from -0.01 to 0.01. The FixedTimeCost will determine when the responseTime
    /// will start applying a diminishing return.
    ///
    /// The Answer will be updated with the new decay value as well as an Intercept value. Check answer.GetIntercept() and answer.GetDecay()
    /// for more information. A presentation time will be added to the Answer on both correct and incorrect answers. 
    /// </summary>
    /// <param name="answer">The Answer object that is being tested by the player</param>
    /// <param name="isCorrect">Boolean value to determine if the selection was correct or incorrect</param>
    /// <param name="responseTime">The observed response time for a player to respond to a given answer. This value is measured in miliseconds.</param>
    /// <param name="fixedTimeCost">The default time before an Answer receives a score with diminishing return. This value is in milliseconds.</param>
    public static void CalculateDecay(Answer answer, bool isCorrect, float responseTime, float fixedTimeCost = 300f)
    {
        var initialDateTime = DateTime.Parse(answer.GetInitialTime());
        var daysSinceInitial = (int)(DateTime.UtcNow - initialDateTime).TotalDays;
        answer.AddPresentationTime(daysSinceInitial);

        var alpha = answer.GetIntercept();

        // predicted response time
        var L = Mathf.Pow(e, -answer.GetActivation()) + fixedTimeCost;

        if (!isCorrect)
            alpha += 0.01f;
        else if(L - responseTime > 500f)
            alpha += Mathf.Clamp(responseTime - L, -0.01f, 0.01f);

        answer.SetIntercept(alpha);
        
        var decay = c * Mathf.Pow(e, answer.GetActivation()) + answer.GetIntercept();

        answer.SetDecay(decay);
    }

    /// <summary>
    /// Calculates the activation value for a given Answer.
    ///
    /// The activation value is updated on the Answer object. Check answer.GetActivation() for more information.
    /// </summary>
    /// <param name="answer">Answer object that will calculate an activation value for.</param>
    public static void CalculateActivationValue(Answer answer)
    {
        var totalTime = 0f;
        int currentTimeDays = 0;

        DateTime currentTime = DateTime.UtcNow;
        var result = DateTime.TryParse(answer.GetInitialTime(), out DateTime initialTime);

        if (result)
            currentTimeDays = (int)((TimeSpan)(currentTime - initialTime)).TotalDays;
        else
            answer.SetInitialTime(currentTime.ToString());
       
        foreach (var presentationTime in answer.GetPresentationTimes())
        {
            float timeDiff = (currentTimeDays - presentationTime);
            totalTime += Mathf.Pow(timeDiff, -answer.GetDecay());
        }

        var activationValue = Mathf.Log(totalTime);

        answer.SetActivation(activationValue);
    }
}
