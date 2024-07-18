using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBallPrefab : MonoBehaviour
{
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private AudioSource audioSource;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "boundary")
        {
            LanguageHoopsManager.shared.ResetBall();
        }
    }

    public void PlaySound() => audioSource.Play();


}
