using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMan : MonoBehaviour
{
    public SettingsHolder settingsHolder;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public AudioSource musicMan;

    // Start is called before the first frame update
    void Start()
    {
        masterSlider.value = settingsHolder.masterVolume;
        musicSlider.value = settingsHolder.musicVolume;
        sfxSlider.value = settingsHolder.sfxVolume;
        updateValues();

        DontDestroyOnLoad(musicMan.gameObject);
    }


    public void updateValues()
    {
        AudioListener.volume = masterSlider.value;
        musicMan.volume = musicSlider.value;
    }

    public void applyChanges()
    {
        settingsHolder.masterVolume = masterSlider.value;
        settingsHolder.musicVolume = musicSlider.value;
        settingsHolder.sfxVolume = sfxSlider.value;
    }

    public void resetValues()
    {
        masterSlider.value = 1f;
        musicSlider.value = 1f;
        sfxSlider.value = 1f;

        settingsHolder.masterVolume = masterSlider.value;
        settingsHolder.musicVolume = musicSlider.value;
        settingsHolder.sfxVolume = sfxSlider.value;
    }
}
