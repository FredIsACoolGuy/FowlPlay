using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer.GameControls {
    public class DummyBird : MonoBehaviour
    {
        [SerializeField] float kbDuration = 0.3f;
        [SerializeField] float kbDistance = 6f;
        [SerializeField] float jumpHeight = 4f;
        [SerializeField] AnimationCurve jumpCurve;
        private bool knocked = false;
        private Vector3 kbDirection = new Vector3();
        private Vector3 ogPos = Vector3.zero;
        private float startTime = 0;
        private ParticleSystem[] ps;

        private void Start() {
            ogPos = transform.position;
            ps = GetComponentsInChildren<ParticleSystem>();
            ps[0].Play();
        }

        private void FixedUpdate() {
            if (knocked) {
                transform.position += kbDirection * kbDistance / kbDuration * Time.fixedDeltaTime;
                float y = jumpCurve.Evaluate((Time.time - startTime) / kbDuration) * jumpHeight + ogPos.y;
                transform.position = new Vector3(transform.position.x, y, transform.position.z);
            }
        }

        private void OnTriggerEnter(Collider other) {
            PlayerMovementController playerScript = other.GetComponent<PlayerMovementController>();
            if (playerScript) {
                if (knocked == false && playerScript.attacking) {
                    knocked = true;
                    kbDirection = transform.position - other.transform.position;
                    kbDirection.y = 0;
                    kbDirection.Normalize();
                    StartCoroutine(Knockback());
                    startTime = Time.time;
                    ps[1].Play();
                }
            }
        }

        IEnumerator Knockback() {
            yield return new WaitForSeconds(kbDuration);
            knocked = false;
            yield return new WaitForSeconds(0.7f);
            if (!knocked) {
                ps[0].Stop();
                transform.position = ogPos;

                yield return null;
                ps[0].Play();
            }
        }
    }
}