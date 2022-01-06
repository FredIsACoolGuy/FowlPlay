using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Multiplayer.GameControls
{
    public class SmokeTrailController : MonoBehaviour
    {
        [SerializeField] PlayerMovementController playerController;
        [SerializeField] PlayerAimScript aimScript;
        private ParticleSystem[] children;
        private int activeChild = 2;

        private void Awake() {
            children = new ParticleSystem[transform.childCount];
            for (int i=0; i<children.Length; i++) {
                children[i] = transform.GetChild(i).GetComponent<ParticleSystem>();
            }
        }

        private void Update() {
            if (playerController.attacking) {
                if (activeChild != 1) {
                    children[activeChild].Stop();
                    children[1].Play();
                    activeChild = 1;
                }
            }
            else if (playerController.knocked) {
                if (activeChild != 1) {
                    children[activeChild].Stop();
                    children[2].Play();
                    children[3].Play();
                    activeChild = 2;
                }
            }
            else if (!playerController.falling) {
                if (activeChild != 0) {
                    children[activeChild].Stop();
                    children[0].Play();
                    activeChild = 0;
                }
                if (aimScript.fireHeld || aimScript.simpleFireHeld) {
                    children[0].Stop();
                }
            }
        }
    }
}
