using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingGameQuestionBoard : MonoBehaviour
{
    private enum TermType
    {
        Word,
        Image,
        Audio
    }
    public GameObject QuestionBoard;

    [SerializeField]
    private GameObject TermWordGameObject = null;

    [SerializeField]
    private GameObject TermImageGameObject = null;

    [SerializeField]
    private GameObject TermAudioGameObject = null;

    [SerializeField]
    private TextPrefabScript TermWordText = null;

    [SerializeField]
    private Image TermImageImage = null;

    public void ConfigureWithWord(string Term)
    {
        var allEnumValues = Enum.GetNames(typeof(TermType));
        var randomIndex = UnityEngine.Random.Range(0, allEnumValues.Count() - 1);
        var randomTermType = allEnumValues[randomIndex];

        TermWordGameObject.SetActive(false);
        TermImageGameObject.SetActive(false);
        TermAudioGameObject.SetActive(false);
        
        switch (randomTermType.ToString())
        {
            case "Image":
                /*
                if (!Term.hasImage)
                {
                    goto case "Word";
                }
                */
                goto case "Word";
                TermImageGameObject.SetActive(true);
                    // TermImageImage.sprite = // TODO: Get the image for the word and assign it
                    break;
                
            case "Audio":
                /*
                if (!Term.hasAudio)
                {
                    goto case "Word";
                }
                */
                goto case "Word";
                TermAudioGameObject.SetActive(true);
                break;

            case "Word":
                TermWordGameObject.SetActive(true);
                TermWordText.Text = Term;
                break;
        }
    }

    public void OnPlayAudio()
    {
        // TODO: Get the word Audio Clip
        FishingGameManager.shared.PlayAudioClip(null);
    }
}
