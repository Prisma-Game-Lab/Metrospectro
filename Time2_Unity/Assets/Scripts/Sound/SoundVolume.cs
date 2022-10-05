using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider musicVolume;
    public Slider overallVolume;
    public Slider sfxVolume;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVol"))
        {
            musicVolume.value = PlayerPrefs.GetFloat("musicVol");
        }
        else
        {
            PlayerPrefs.SetFloat("musicVol", 0.5f);
            musicVolume.value = PlayerPrefs.GetFloat("musicVol");
            mixer.SetFloat("musicVol", Mathf.Log10(musicVolume.value + 0.0001f) * 25);
        }

        if (PlayerPrefs.HasKey("overallVol"))
        {
            overallVolume.value = PlayerPrefs.GetFloat("overallVol");
        }
        else
        {
            PlayerPrefs.SetFloat("overallVol", 0.5f);
            overallVolume.value = PlayerPrefs.GetFloat("overallVol");
            mixer.SetFloat("overallVol", Mathf.Log10(overallVolume.value + 0.0001f) * 25);
        }

        if (PlayerPrefs.HasKey("sfxVol"))
        {
            sfxVolume.value = PlayerPrefs.GetFloat("sfxVol");
        }
        else
        {
            PlayerPrefs.SetFloat("sfxVol", 0.5f);
            sfxVolume.value = PlayerPrefs.GetFloat("sfxVol");
            mixer.SetFloat("sfxVol", Mathf.Log10(sfxVolume.value+0.0001f) * 25);
        }
    }

    public void SetMusictrackVolume()
    {
        PlayerPrefs.SetFloat("musicVol", musicVolume.value);
        mixer.SetFloat("musicVol", Mathf.Log10(musicVolume.value + 0.0001f) * 25);
    }
    
    public void SetoveralltrackVolume()
    {
        PlayerPrefs.SetFloat("overallVol", overallVolume.value);
        mixer.SetFloat("overallVol", Mathf.Log10(overallVolume.value + 0.0001f) * 25);
    }

    public void SetsfxtrackVolume()
    {
        PlayerPrefs.SetFloat("sfxVol", sfxVolume.value);
        mixer.SetFloat("sfxVol", Mathf.Log10(sfxVolume.value+0.0001f) * 25);
    }

}
