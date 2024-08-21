using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/AudioClipRefsSO", fileName = "AudioClipRefsSO")]
public class AudioClipRefsSO : ScriptableObject
{
    public List<Sound> keySounds;
    public List<Combo> keyCombos;

    [System.Serializable]
    public class Sound
    {
        public KeySound eSound;
        public AudioClip audioClip;
    }

    [Serializable]
    public class Combo
    {
        public KeyCombo key;
        public List<AudioClip> audioClips;
    }
}