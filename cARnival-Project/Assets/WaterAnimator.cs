using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAnimator : MonoBehaviour
{
    public Texture2D[] frames;
    public float framesPerSecond = 10.0f;

    private Renderer renderer;
    private int currentFrame;
    private float frameTimer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        currentFrame = 0;
        frameTimer = 0.0f;
    }

    void Update()
    {
        if (frames.Length == 0) return;

        frameTimer += Time.deltaTime;
        if (frameTimer >= 1.0f / framesPerSecond)
        {
            currentFrame = (currentFrame + 1) % frames.Length; // Loop through the frames
            renderer.material.mainTexture = frames[currentFrame]; // Set the current frame as the texture
            frameTimer = 0.0f; // Reset the timer
        }
    }
}
