using Anura.Templates.MonoSingleton;
using System.Collections;
using UnityEngine;

public class SFXManager : MonoSingleton<SFXManager>
{
    public AudioSource musicSource;
    public AudioSource oneShotAudioSource;

    private float oneShotSourceBaseVolume;

    private void Start()
    {
        ApplySettings();
    }

    private void OnEnable()
    {
        GameSettings.onGameSettingsApplied += ApplySettings;
    }

    private void OnDisable()
    {
        GameSettings.onGameSettingsApplied -= ApplySettings;
    }

    private void ApplySettings()
    {
        if(musicSource != null)
        {
            musicSource.volume = GameState.gameSettings.musicVolume * GameState.gameSettings.masterVolume;
        }

        if(oneShotAudioSource != null)
        {
            oneShotSourceBaseVolume = oneShotAudioSource.volume = GameState.gameSettings.soundFXVolume * GameState.gameSettings.masterVolume;
        }
    }

    public void PlayOneShot(AudioClip clip, float volume = 1)
    {
        StopOneShot();
        oneShotAudioSource.volume = oneShotAudioSource.volume > 0 ? volume * oneShotSourceBaseVolume : 0;
        oneShotAudioSource.PlayOneShot(clip);
    }

    public void StopOneShot()
    {
        oneShotAudioSource.Stop();
    }
}
