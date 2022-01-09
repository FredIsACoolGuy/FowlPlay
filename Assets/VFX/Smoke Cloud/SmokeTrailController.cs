using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Multiplayer.GameControls
{
    public class SmokeTrailController : MonoBehaviour
    {
        //[SerializeField] PlayerMovementController playerController;
        [SerializeField] NetworkGamePlayer networkPlayer;
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
            if (networkPlayer.currentState == 0) {
                if (activeChild != 0) {
                    children[activeChild].Stop();
                    children[0].Play();
                    activeChild = 0;
                }
                if (aimScript.fireHeld || aimScript.simpleFireHeld) {
                    children[0].Stop();
                }
            }
            else if (networkPlayer.currentState == 1) {
                if (activeChild != 1) {
                    children[activeChild].Stop();
                    children[1].Play();
                    activeChild = 1;
                }
            }
            else if (networkPlayer.currentState == 2) {
                if (activeChild != 2) {
                    children[activeChild].Stop();
                    children[2].Play();
                    children[3].Play();
                    activeChild = 2;
                }
            }
        }
    }
}
