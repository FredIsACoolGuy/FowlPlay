using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer.GameControls
{
    public class FlameCharge : MonoBehaviour
    {
        [SerializeField] PlayerAimScript aimScript;
        bool isActive = false;

        private void Awake() {
            if (aimScript != null) {
                SetChildrenActive(false);
            }
        }

        private void Update() {
            //turn on/off
            if (aimScript != null) {
                if (aimScript.fireHeld || aimScript.simpleFireHeld && !isActive) {
                    SetChildrenActive(true);
                }
                else if (!aimScript.fireHeld && !aimScript.simpleFireHeld && isActive) {
                    SetChildrenActive(false);
                }
                isActive = aimScript.fireHeld || aimScript.simpleFireHeld;
            }

            float timeFactor = aimScript.timeHeld / aimScript.maxTimeHeld;
            transform.localScale = Vector3.Lerp(new Vector3(0.5f,0.5f,0.5f), new Vector3(1f,1f,1f), timeFactor);
        }

        private void SetChildrenActive(bool isActive) {
            for(int i=0; i<transform.childCount; i++) {
                transform.GetChild(i).gameObject.SetActive(isActive);
            }
        }
    }
}