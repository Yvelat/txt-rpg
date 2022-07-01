using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VoiceListSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI voiceName;
    private List<AudioClip> voiceClips;

    public void SetData(Voice voice)
    {
        voiceClips = voice.VoiceClips;
        voiceName.text = voice.Name;
    }

    public void Listen()
    {

    }
}
