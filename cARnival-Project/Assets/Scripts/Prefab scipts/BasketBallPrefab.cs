using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBallPrefab : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "boundary")
        {
            LanguageHoopsManager.shared.ResetBall();
        }
    }
}
