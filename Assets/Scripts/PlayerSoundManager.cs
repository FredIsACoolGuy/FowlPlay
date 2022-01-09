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

    public AudioClip[] chargeUps;
    public AudioClip[] attacks;
    public AudioClip[] hits;
    public AudioClip[] falls;
    public AudioClip[] water;
    public AudioClip pickUp;

    public PlayerMovementController movementController;

    void Start()
    {
        StartCoroutine(doFootstep());
    }

    public void playChargeUp()
    {
        playSoundArray(chargeUps);
    }

    public void playAttack()
    {
        playSoundArray(attacks);
    }

    public void playHit()
    {
        playSoundArray(hits);
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
        chirpAudioSource.clip = chirps[Random.Range(0, chirps.Length)];
        chirpAudioSource.pitch = Random.Range(0.95f, 1.05f);
        chirpAudioSource.Play();
    }

    private void playSoundArray(AudioClip[] sounds)
    {
        audioSource.clip = pickUp;
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.Play();
    }

    private IEnumerator doFootstep()
    {
        if (!movementController.knocked && movementController.prevInput.magnitude > 0.8f)
        {
            yield return new WaitForSeconds(footSpeed / 2);
            feetAudioSource.clip = footSteps[Random.Range(0, footSteps.Length)];
            feetAudioSource.pitch = Random.Range(0.95f, 1.05f);
            feetAudioSource.Play();
        }
        yield return new WaitForEndOfFrame();

        StartCoroutine(doFootstep());
    }
}
