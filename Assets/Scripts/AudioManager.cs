using System;
using System.Collections;
using DG.Tweening;
using UnityEngine.Audio;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.outputAudioMixerGroup = sound.outputAudioMixerGroup;
            sound.source.loop = sound.loop;
        }
        
    }

    public AudioSource Play(string soundName, float duration = -1f)
    {
        Sound soundToPlay = GetSound(soundName);
        if (soundToPlay == null)
        {
            return null;
        }
        soundToPlay.source.Play();
        if (duration > -1f)
        { 
            StartCoroutine(StopSoundAfterTime(soundToPlay.source, duration));
        }
        return soundToPlay.source;
    }
    
    public IEnumerator PlayAfterTime(string soundName, float durationToWait, float durationToStop = -1f)
    {
        yield return new WaitForSeconds(durationToWait);
        Play(soundName, durationToStop);
    }


    public Sound GetSound(string soundName)
    {
        Sound sound =  Array.Find(sounds, sound => sound.name == soundName);
        if (sound == null)
        {
            Debug.Log("Sound " + soundName + " not found !");
            return null;
        }

        return sound;
    }

    public IEnumerator StopSoundAfterTime(AudioSource source, float time, float fadeDuration = 0.3f)
    {
        yield return new WaitForSeconds(time);
        StopSound(source, fadeDuration);
        
    }

    private void StopSound(AudioSource source, float fadeDuration)
    {
        float volume = source.volume;
        source.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            source.Stop();
            source.volume = volume;
        });
    }
}
