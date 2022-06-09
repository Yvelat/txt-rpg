using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip intro;
    [SerializeField] AudioClip loop;

    public bool otherClip = false;

    private void Start()
    {
        source.playOnAwake = false;
    }

    private void Update()
    {
        if (!source.isPlaying && !otherClip)
        {
            otherClip = true;
            source.clip = loop;
            source.loop = true;
            source.Play();
        }
    }

    public void ResetAndPlay()
    {
        if(intro != null)
        {
            source.loop = false;
            source.clip = intro;
            source.Play();
            otherClip = false;
        }
        else
        {
            source.Play();
        }
        
    }

    public void Pause()
    {
        source.Pause();
    }

    public void Resume()
    {
        source.UnPause();
    }

    public void Stop()
    {
        source.Stop();
    }
}
