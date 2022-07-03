using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] Image spriteRenderer;
    [SerializeField] List<Sprite> frames;
    [SerializeField] float frameRate = 0.16f;

    int currentFrame;
    float timer;

    public void SetData(List<Sprite> frames)
    {
        this.frames = frames;
    }

    public void Start()
    {
        currentFrame = 0;
        timer = 0f;

        spriteRenderer.sprite = frames[currentFrame];
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if(timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;
        }
    }

    public List<Sprite> Frames
    {
        get { return frames; }
    }
}
