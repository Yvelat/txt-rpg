using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceSelectionHandler : MonoBehaviour
{
    [SerializeField] GameObject voiceList;
    [SerializeField] GameObject voiceSlotPrefab;

    [SerializeField] AudioSource voicePreview;

    [Header("Voices")]
    [SerializeField] List<Voice> voices;

    private void Start()
    {

        voicePreview.loop = false;
        voicePreview.clip = null;
        voicePreview.playOnAwake = false;

        foreach (Transform child in voiceList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Voice voice in voices)
        {
            var obj = Instantiate(voiceSlotPrefab, voiceList.transform);
        }
    }

    public void PlayPreview(AudioClip clip)
    {
        voicePreview.loop = false;
        voicePreview.clip = clip;
        voicePreview.playOnAwake = false;
        voicePreview.Play();
    }

}
