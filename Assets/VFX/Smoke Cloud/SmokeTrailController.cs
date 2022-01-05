using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Multiplayer.GameControls
{
    public class SmokeTrailController : MonoBehaviour
    {
        [SerializeField] PlayerMovementController playerController;
        private ParticleSystem[] children = new ParticleSystem[3];
        private int activeChild = 2;

        private void Awake() {
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
                    activeChild = 2;
                }
            }
            else if (!playerController.falling && activeChild != 0) {
                children[activeChild].Stop();
                children[0].Play();
                activeChild = 0;
            }
        }
    }
}
