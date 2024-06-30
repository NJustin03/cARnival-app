using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ResetAccountInfo : MonoBehaviour
{

    // 1 means that the player forgot their username, 2 means they forgot their password.
    public static int forgotUsernameOrPassword = 1;

    public TMP_InputField email;


    public void ResetInfo()
    {
        if (string.IsNullOrEmpty(email.text))
        {
            return;
        }
        else if (forgotUsernameOrPassword == 1)
        {
            StartCoroutine(APIManager.ForgotUsername(email.text));
        }
        else
        {
            StartCoroutine(APIManager.ForgotPassword(email.text));
        }
    }

    public static void SetForgotVariable(int userOrPassword)
    {
        forgotUsernameOrPassword = userOrPassword;
    }
}
