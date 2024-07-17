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

    private AudioClip TermAudio = null;

    public void ConfigureWithWord(Answer Term)
    {
        var allEnumValues = Enum.GetNames(typeof(TermType));
        var randomIndex = UnityEngine.Random.Range(0, allEnumValues.Count());
        var randomTermType = allEnumValues[randomIndex];

        TermWordGameObject.SetActive(false);
        TermImageGameObject.SetActive(false);
        TermAudioGameObject.SetActive(false);
        //randomTermType.ToString()
        switch ("Audio")
        {
            case "Image":
                
                if (Term.hasImage)
                {
                    Debug.Log("No image found!");
                    goto case "Word";
                }

                Texture2D texture = Term.GetImage();
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                TermImageImage.sprite = sprite;
                TermImageGameObject.SetActive(true);
                break;
                
            case "Audio":
                if (Term.hasAudio)
                {
                    Debug.Log("No sound found!");
                    goto case "Word";
                }

                TermAudio = Term.GetAudio();  
                TermAudioGameObject.SetActive(true);
                OnPlayAudio();
                break;

            case "Word":
                TermWordGameObject.SetActive(true);
                TermWordText.Text = Term.GetFront();
                break;
        }
    }

    public void OnPlayAudio()
    {
        FishingGameManager.shared.PlayAudioClip(TermAudio);
    }
}
