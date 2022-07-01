using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Crea nuova Voce")]
public class Voice : ScriptableObject
{
    [SerializeField] string voiceName;
    [SerializeField] List<AudioClip> voiceClipList;

    public List<AudioClip> VoiceClips => voiceClipList;
    public string Name => voiceName;
}
