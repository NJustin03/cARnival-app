using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsPrefab : MonoBehaviour
{
    [SerializeField]
    private LanguageHoopsManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            gameManager.ResetBall();
        }
    }
}
