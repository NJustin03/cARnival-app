using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBallGameQuestionBoard : MonoBehaviour
{
   
    public GameObject QuestionBoard;

    [SerializeField]
    private GameObject TermWordGameObject = null;

    [SerializeField]
    private GameObject TermAudioGameObject = null;

    [SerializeField]
    private TextPrefabScript TermWordText = null;

    private AudioClip TermAudio = null;

    public void ConfigureWithWord(Answer Term)
    {

        TermWordGameObject.SetActive(false);
        TermAudioGameObject.SetActive(false);
   
        switch ("Audio")
        {
           //TODO: add logic for setting to go to word
            case "Audio":
                if (Term.GetAudio() == null)
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
        LanguageHoopsManager.shared.PlayAudioClip(TermAudio);
    }
}
