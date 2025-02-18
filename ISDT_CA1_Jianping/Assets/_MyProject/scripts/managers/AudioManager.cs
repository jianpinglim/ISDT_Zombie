//
// Description:
// This AudioManager is implemented as a singleton to ensure only one instance exists
// during the lifetime of the game. It manages background music and sound effects (SFX)
// by holding arrays of Sound objects. The script provides methods for playing music,
// playing SFX, toggling mute for both, and setting volume levels. The AudioSource components
// for music and SFX are provided via the Inspector. The singleton instance persists
// across scenes using DontDestroyOnLoad().
//
// Usage:
// - Assign the AudioManager script to a GameObject in your starting scene.
// - Populate the musicSounds and sfxSounds arrays in the Inspector with your Sound objects.
// - Drag & drop your music and SFX AudioSource components to the corresponding fields.
// - Use PlayMusic, PlaySfx, ToggleSFX, ToggleMusic, SetSFXVolume, and SetMusicVolume through code or UI events.

using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    // Singleton instance of the AudioManager
    public static AudioManager instance;
    
    // Arrays of Sound objects for music and sound effects; these should be set in the Inspector
    public Sound[] musicSounds, sfxSounds;
    
    // AudioSources for playing music and sound effects
    public AudioSource musicSource, sfxSource;

    // Awake is called before Start; handle singleton pattern here
    private void Awake() {
        if (instance == null)
        {
            instance = this;
            // Persist this object across scene loads
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this new one
            Destroy(gameObject);
            return;
        }
    }

    // Start is called on the frame when a script is enabled
    private void Start() {
        // Play the background music using the sound name "Theme"
        PlayMusic("Theme");
    }
    
    // PlayMusic finds a Sound by its name in the musicSounds array and plays it through the musicSource.
    public void PlayMusic(string soundName)
    {
        Sound s = Array.Find(musicSounds, sound => sound.soundName == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }
        // Set the clip of the music source to the found sound and play it.
        musicSource.clip = s.clip;
        musicSource.Play();
    }

    // PlaySfx finds a Sound by its name in the sfxSounds array and plays it as a one-shot sound.
    public void PlaySfx(string soundName)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.soundName == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }
        else
        {
            // Play the sound effects clip once without interrupting any currently playing clips.
            sfxSource.PlayOneShot(s.clip);
        }
    }

    // ToggleSFX mutes or unmutes the SFX AudioSource.
    public void ToggleSFX(){
        sfxSource.mute = !sfxSource.mute;
    }

    // ToggleMusic mutes or unmutes the music AudioSource.
    public void ToggleMusic(){
        musicSource.mute = !musicSource.mute;
    }

    // SetSFXVolume updates the volume of the SFX AudioSource.
    public void SetSFXVolume(float volume){
        sfxSource.volume = volume;
    }

    // SetMusicVolume updates the volume of the music AudioSource.
    public void SetMusicVolume(float volume){
        musicSource.volume = volume;
    }
}