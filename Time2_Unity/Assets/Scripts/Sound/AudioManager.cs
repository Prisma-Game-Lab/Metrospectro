using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>
{
    AudioSource audioSource;
	[SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip collideFront;
    [SerializeField] private AudioClip collideLeft;
    [SerializeField] private AudioClip collideRight;

    [SerializeField] private AudioClip stepFront;
    [SerializeField] private AudioClip stepLeft;
    [SerializeField] private AudioClip stepRight;
    [SerializeField] private AudioClip stepBack;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayStep(Vector2 direction)
    {
        if (direction.x < 0) audioSource.clip = stepLeft;
        if (direction.x > 0) audioSource.clip = stepRight;
        if (direction.y < 0) audioSource.clip = stepBack;
        if (direction.y > 0) audioSource.clip = stepFront;

        audioSource.PlayDelayed(.2f);
    }

    public void PlayCollide(Vector2 direction)
    {
        if (direction.x < 0) audioSource.clip = collideLeft;
        if (direction.x > 0) audioSource.clip = collideRight;
        if (direction.y < 0) audioSource.clip = collideFront;
        if (direction.y > 0) audioSource.clip = collideFront;
        audioSource.Play();
    }

    public void PlayClick()
    {
        audioSource.clip = buttonClick;
        audioSource.Play();
    }
}
