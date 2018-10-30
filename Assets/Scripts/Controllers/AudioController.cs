using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    [SerializeField]
    AudioSource myAudioSource;
    [Space(10)]
    [SerializeField]
    AudioClip idle;
    [SerializeField]
    AudioClip fall;
    [SerializeField]
    AudioClip hazard;
    [SerializeField]
    AudioClip hitCeiling;
    [SerializeField]
    AudioClip jump;
    [SerializeField]
    AudioClip lose;
    [SerializeField]
    AudioClip stun;
    [SerializeField]
    AudioClip walk;
    [SerializeField]
    AudioClip win;
    [SerializeField]
    AudioClip letterFall;


    public void PlayIdle()
    {
        if (myAudioSource.clip != idle)
        {
            myAudioSource.Stop();

            myAudioSource.clip = idle;
            myAudioSource.volume = 1;
            myAudioSource.pitch = 1;
            myAudioSource.loop = true;

            myAudioSource.Play();
        }
    }

    public void PlayFall()
    {
        myAudioSource.Stop();

        myAudioSource.clip = fall;
        myAudioSource.volume = 1;
        myAudioSource.pitch = 1;
        myAudioSource.loop = false;

        myAudioSource.Play();
    }

    public void PlayHazard()
    {
        myAudioSource.Stop();

        myAudioSource.clip = hazard;
        myAudioSource.volume = 1;
        myAudioSource.pitch = 1;
        myAudioSource.loop = false;

        myAudioSource.Play();
    }

    public void PlayHitCeiling()
    {
        myAudioSource.Stop();

        myAudioSource.clip = hitCeiling;
        myAudioSource.volume = 1;
        myAudioSource.pitch = 1;
        myAudioSource.loop = false;

        myAudioSource.Play();
    }

    public void PlayJump()
    {
        myAudioSource.Stop();

        myAudioSource.clip = jump;
        myAudioSource.volume = 1;
        myAudioSource.pitch = 1;
        myAudioSource.loop = false;

        myAudioSource.Play();
    }

    public void PlayLose()
    {
        myAudioSource.Stop();

        myAudioSource.clip = lose;
        myAudioSource.volume = 1;
        myAudioSource.pitch = 1;
        myAudioSource.loop = false;

        myAudioSource.Play();

        StopCoroutine(CheckLoseIsPlaying());
        StartCoroutine(CheckLoseIsPlaying());
    }

    public void PlayStun()
    {
        if (myAudioSource.clip != stun)
        {
            myAudioSource.Stop();

            myAudioSource.clip = stun;
            myAudioSource.volume = 1;
            myAudioSource.pitch = 2;
            myAudioSource.loop = true;

            myAudioSource.Play();
        }
    }

    public void StopPlaying()
    {
        myAudioSource.Stop();
    }

    public void PlayWalk()
    {
        if (myAudioSource.clip != walk)
        {
            myAudioSource.Stop();

            myAudioSource.clip = walk;
            myAudioSource.volume = 1;
            myAudioSource.pitch = 2;
            myAudioSource.loop = true;

            myAudioSource.Play();
        }
    }

    public void PlayWin()
    {
        myAudioSource.Stop();

        myAudioSource.clip = win;
        myAudioSource.volume = 1;
        myAudioSource.pitch = 1;
        myAudioSource.loop = false;

        myAudioSource.Play();

        StopCoroutine(CheckWinIsPlaying());
        StartCoroutine(CheckWinIsPlaying());
    }

    private IEnumerator CheckWinIsPlaying()
    {
        while (myAudioSource.isPlaying && myAudioSource.clip == win)
        {
            yield return new WaitForEndOfFrame();
        }

        GameController.Instance.GoToLoadingScreen();
        yield break;
    }

    private IEnumerator CheckLoseIsPlaying()
    {
        while (myAudioSource.isPlaying && myAudioSource.clip == lose)
        {
            yield return new WaitForEndOfFrame();
        }

        GameController.Instance.GoToGameOverScreen();
        yield break;
    }

    public void PlayLetterFall()
    {
        myAudioSource.Stop();

        myAudioSource.clip = letterFall;
        myAudioSource.volume = 1;
        myAudioSource.pitch = 1f;
        myAudioSource.loop = false;

        myAudioSource.Play();
    }
}
