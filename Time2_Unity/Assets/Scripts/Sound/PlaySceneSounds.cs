using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySceneSounds : MonoBehaviour
{
    public string[] musicName;
    public bool stopOtherMusics;
    public bool stopEffects;
    // Start is called before the first frame update
    void Start()
    {
        if (stopOtherMusics)
            AudioManager.instance.StopAllMusicsSounds();
        if(stopEffects)
            AudioManager.instance.StopAllEffectsSounds();
        foreach (string item in musicName)
        {
            AudioManager.instance.Play(item);
        }
    }
    public void Play(string soundName)
    {
        AudioManager.instance.Play(soundName);
    }
}
