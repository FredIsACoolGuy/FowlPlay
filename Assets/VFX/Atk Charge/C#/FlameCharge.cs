using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer.GameControls
{
    public class FlameCharge : MonoBehaviour
    {
        [SerializeField] PlayerAimScript aimScript;
        bool isActive = false;
        bool cloudPSPlaying = false;

        private void Update() {
            //turn on/off
            float timeFactor;
            if (aimScript != null) {
                if (aimScript.fireHeld || aimScript.simpleFireHeld && !isActive) {
                    transform.GetChild(0).gameObject.SetActive(true);
                }
                else if (!aimScript.fireHeld && !aimScript.simpleFireHeld && isActive) {
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
                    cloudPSPlaying = false;
                }
                isActive = aimScript.fireHeld || aimScript.simpleFireHeld;
                timeFactor = aimScript.timeHeld / aimScript.maxTimeHeld;
            }
            else {
                timeFactor = Time.time % 1;
            }

            transform.localScale = Vector3.Lerp(new Vector3(0.5f,0.5f,0.5f), new Vector3(1f,1f,1f), timeFactor);

            if (isActive && timeFactor == 1 && !cloudPSPlaying) {
                transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                cloudPSPlaying = true;
            }
        }
    }
}