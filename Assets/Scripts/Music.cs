using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioClip ambienceMusic, accentMusic;

    private AudioSource ambienceSource, accentSource;

    public static Music instance = null;
    void Awake(){
        if(instance != null){
            Destroy(gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        ambienceSource = gameObject.AddComponent<AudioSource>();
        accentSource = gameObject.AddComponent<AudioSource>();

        ambienceSource.clip = ambienceMusic;
        accentSource.clip = accentMusic;

        accentSource.volume = 0;
        ambienceSource.volume = 0.25f;
        accentSource.playOnAwake = false;
        ambienceSource.playOnAwake = false;
        accentSource.loop = true;
        ambienceSource.loop = true;

        ambienceSource.Play();

        StartCoroutine(DelayedAccentStart());
        StartCoroutine(AmbienceVolume());

        GameEventHandler.Subscribe(GameEvent.GameStarted, OnGameStarted);
    }

    void OnGameStarted(GameEventArgs args)
    {
        StartCoroutine(AccentVolume(true));
    }

    IEnumerator AccentVolume(bool volumeUp)
    {
        for (float i = 0; i < 1; i += Time.deltaTime / 5)
        {
            accentSource.volume = (volumeUp ? i : 1 - i) * 0.25f;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator AmbienceVolume()
    {
        for (float i = 0; i < 1; i += Time.deltaTime / 10)
        {
            ambienceSource.volume = i*0.25f;

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DelayedAccentStart()
    {
        yield return new WaitForSeconds(12.366f);

        accentSource.Play();
    }
}
