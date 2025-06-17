using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioSettingManager : MonoBehaviour
{
    private VCA masterVCA;
    private VCA musicVCA;
    private VCA sfxVCA;

    private void Start()
    {
        masterVCA = RuntimeManager.GetVCA("vca:/Master");
        musicVCA = RuntimeManager.GetVCA("vca:/Music");
        sfxVCA = RuntimeManager.GetVCA("vca:/SFX");

        // Optionally load saved settings
        SetVolume(masterVCA, PlayerPrefs.GetFloat("Volume_Master", 1f));
        SetVolume(musicVCA, PlayerPrefs.GetFloat("Volume_Music", 0.5f));
        SetVolume(sfxVCA, PlayerPrefs.GetFloat("Volume_SFX", 0.5f));
    }

    public void SetMasterVolume(float volume)
    {
        SetVolume(masterVCA, volume);
        PlayerPrefs.SetFloat("Volume_Master", volume);
    }

    public void SetMusicVolume(float volume)
    {
        SetVolume(musicVCA, volume);
        PlayerPrefs.SetFloat("Volume_Music", volume);
    }

    public void SetSFXVolume(float volume)
    {
        SetVolume(sfxVCA, volume);
        PlayerPrefs.SetFloat("Volume_SFX", volume);
    }

    private void SetVolume(VCA vca, float volume)
    {
        vca.setVolume(volume);
    }
}
