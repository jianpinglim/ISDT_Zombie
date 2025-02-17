using UnityEngine;
using System;
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start() {
        PlayMusic("Theme");
    }
    public void PlayMusic(string soundName)
    {
        Sound s = Array.Find(musicSounds, sound => sound.soundName == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }
        musicSource.clip = s.clip;
        musicSource.Play();
    }

    public void PlaySfx(string soundName)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.soundName == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }
        else{
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void ToggleSFX(){
        sfxSource.mute = !sfxSource.mute;
    }

    public void ToggleMusic(){
        musicSource.mute = !musicSource.mute;
    }

    public void SetSFXVolume(float volume){
        sfxSource.volume = volume;
    }

    public void SetMusicVolume(float volume){
        musicSource.volume = volume;
    }
}
