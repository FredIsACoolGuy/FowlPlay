using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiplayer.GameControls;

public class PlayerSoundManager : MonoBehaviour
{
    public AudioSource feetAudioSource;
    public AudioSource chirpAudioSource;
    public AudioSource audioSource;

    public AudioClip[] chirps;

    public AudioClip[] footSteps;
    public float footSpeed = 1f;

    public AudioClip chargeUp;
    public AudioClip[] attacks;
    public AudioClip[] hits;
    public AudioClip[] falls;
    public AudioClip[] water;
    public AudioClip pickUp;

    public PlayerMovementController movementController;

    public SettingsHolder settingsHolder;

    void Start()
    {
        audioSource.volume = settingsHolder.sfxVolume;
        chirpAudioSource.volume = settingsHolder.sfxVolume;
        feetAudioSource.volume = settingsHolder.sfxVolume * 0.6f;
        StartCoroutine(doFootstep());
    }

    public void playChargeUp()
    {
        playSound(chargeUp);
        playChirp();
    }

    public void playAttack()
    {
        playSoundArray(attacks);
        playChirp();
    }

    public void playHit()
    {
        playSoundArray(hits);
        playChirp();
    }

    public void playFall()
    {
        playSoundArray(falls);
    }

    public void playWater()
    {
        playSoundArray(water);
    }

    public void playChirp()
    {
        chirpAudioSource.clip = chirps[Random.Range(0, chirps.Length)];
        chirpAudioSource.pitch = Random.Range(0.95f, 1.05f);
        chirpAudioSource.Play(); 
    }

    public void playPickup()
    {
        playSound(pickUp);
    }

    private void playSoundArray(AudioClip[] sounds)
    {
        audioSource.clip = sounds[Random.Range(0, sounds.Length)];
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.Play();
    }

    private void playSound(AudioClip sound)
    {
        audioSource.clip = sound;
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.Play();
    }

    private IEnumerator doFootstep()
    {
        if (!movementController.knocked && movementController.prevInput.magnitude > 0.8f)
        {
            feetAudioSource.clip = footSteps[Random.Range(0, footSteps.Length)];
            feetAudioSource.pitch = Random.Range(0.95f, 1.05f);
            feetAudioSource.Play();
            yield return new WaitForSeconds(footSpeed / 2);
        }
        yield return new WaitForEndOfFrame();

        StartCoroutine(doFootstep());
    }
}
