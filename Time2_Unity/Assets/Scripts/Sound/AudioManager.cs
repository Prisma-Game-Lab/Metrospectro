using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	public Sound[] soundMusics;
	public Sound[] soundEffects;
	public static AudioManager instance;

	void Awake()
	{
		//Make Audiomanager persist between scenes
		if (instance == null)
			instance = this;
		else
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);

		//Create audioSources with settings
		foreach (Sound s in soundMusics)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = s.mixerGroup;
		}
		foreach (Sound s in soundEffects)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = s.mixerGroup;
		}
	}

	//Find specifc sound
	private Sound GetSound(string sound)
	{
		Sound s = Array.Find(soundMusics, item => item.name == sound);
		if (s != null)
		{
			return s;
		}

		s = Array.Find(soundEffects, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return null;
		}
		return s;
	}

	public void Play(string sound)
	{
		Sound s = GetSound(sound);
		if (s != null)
		{
			s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
			s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
            s.source.Play();
		}
	}

	public void PlayForSeconds(string sound, float time)
	{
		Sound s = GetSound(sound);
		if (s != null)
		{
			s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
			StartCoroutine(PlayForSecondsCoroutine(s, time));
		}
    }

	IEnumerator PlayForSecondsCoroutine(Sound sound, float time)
	{
		sound.source.Play();
		yield return new WaitForSeconds(time);
		sound.source.Stop();
	}

	//Play one of the sounds randonly
	public void playMultipleRandomSounds(string[] sounds)
	{
		int index = UnityEngine.Random.Range(0, sounds.Length);
		Play(sounds[index]);
	}


	public void Stop(string name)
	{
		Sound s = GetSound(name);
		if (s != null)
			s.source.Stop();
	}

    public void StopAllMusicsSounds()
    {
        foreach (Sound sound in soundMusics)
        {
            sound.source.Stop();
        }
    }

    public void StopAllEffectsSounds()
    {
        foreach (Sound sound in soundEffects)
        {
            sound.source.Stop();
        }
    }

	//Simulates 3 different sounds as a single one with the duration you desire
	//Repeats the middle sound to fit the duration passed
	public void PlayDynamicSound(string startSound, string middleSound, string endSound, float totalDurationSeconds)
	{
        Play(startSound);
		Sound startSoundSource = GetSound(startSound);
		Sound middleSoundSource = GetSound(middleSound);
		Sound endSoundSource = GetSound(endSound);

		float middleSoundDurationSeconds = totalDurationSeconds - startSoundSource.clip.length - endSoundSource.clip.length;

		StartCoroutine(PlayMiddleSound(middleSoundSource, endSoundSource, middleSoundDurationSeconds, startSoundSource.clip.length));
	}

    IEnumerator PlayMiddleSound(Sound middleSoundSource, Sound endSoundSource, float middleSoundDurationTime, float startSoundDurationTime)
    {
        float middleSoundClipTime = middleSoundSource.clip.length;

        if (startSoundDurationTime != 0)
            yield return new WaitForSeconds(startSoundDurationTime);

        while (middleSoundDurationTime >= middleSoundClipTime / 2)
        {
            Play(middleSoundSource.name);
            yield return new WaitForSeconds(middleSoundClipTime);
            middleSoundDurationTime -= middleSoundClipTime;
        }

		Play(endSoundSource.name);
    }


}
