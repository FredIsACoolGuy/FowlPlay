using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Multiplayer.GameControls
{
    public class PlayerAimScript : NetworkBehaviour
    {
        private LineRenderer line;

        public Transform target;

        private Camera cam;

        private GameControls controls;

        private Vector2 pointerOffset;

        private bool simpleFireHeld = false;

        private bool fireHeld=false;
        private bool freshFire = false;
        public float lineSpeed;
        public float maxLineLength = 5f;
        public float maxTimeHeld = 2f;

        private PlayerMovementController movementController;
        private GameControls Controls
        {
            get
            {
                if (controls != null)
                {
                    return controls;
                }
                return controls = new GameControls();
            }
        }

        public override void OnStartAuthority()
        {
            movementController = GetComponent<PlayerMovementController>();
            line = GetComponent<LineRenderer>();
            line.enabled = true;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, transform.position);
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            enabled = true;

            Controls.Player.Look.performed += ctx => MoveTarget(ctx.ReadValue<Vector2>());
            Controls.Player.Fire.performed += ctx => Fire(ctx.ReadValue<float>());
            Controls.Player.SimpleFire.performed += ctx => SimpleFire(ctx.ReadValue<float>());
            
        }

        [ClientCallback]
        private void OnEnable()
        {
            Controls.Enable();
        }

        [ClientCallback]
        private void OnDisable()
        {
            Controls.Disable();
        }

        float timeHeld;
        private void Fire(float value)
        {
            if (!simpleFireHeld)
            {
                if (value > 0.5f)
                {
                    fireHeld = true;
                    freshFire = true;
                    timeHeld = 0f;
                }
                else
                {
                    fireHeld = false;
                }
            }

        }

        private void SimpleFire(float value)
        {
            if (!fireHeld)
            {
                if (value > 0.5f)
                {
                    simpleFireHeld = true;
                    freshFire = true;
                    timeHeld = 0f;
                }
                else
                {
                    simpleFireHeld = false;
                }
            }
        }

        private void MoveTarget(Vector2 mousePos)
        {
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();

            pointerOffset = mousePos - new Vector2(cam.WorldToScreenPoint(transform.position).x, cam.WorldToScreenPoint(transform.position).y);
        }


        private void Update()
        {
            if (simpleFireHeld)
            {
                Aim(new Vector2(movementController.facingDir.x, movementController.facingDir.z));
            }
            else if (fireHeld)
            {
                Aim(pointerOffset);
            }
            else if(freshFire)
            {
                freshFire = false;
                movementController.aimingVec = Vector3.zero;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, transform.position);
                movementController.movementSpeed = movementController.normalMovementSpeed;
                //do attack
                movementController.Attack(Mathf.Clamp(timeHeld, 0f, maxTimeHeld));
            }
        }

        private void Aim(Vector2 offset)
        {
            //count time held
            timeHeld += Time.deltaTime;
            //find distance to draw line based on anlge and time held
            target.transform.position = transform.position + new Vector3(offset.x, 0f, offset.y).normalized * Mathf.Clamp(timeHeld * lineSpeed, 0f, maxLineLength);

            //position line
            line.SetPosition(0, transform.position);
            line.SetPosition(1, target.transform.position);

            //change colour of line
            line.startColor = new Color(1f, 1f / (timeHeld * 2f), 1f / (timeHeld * 2f), 1f);
            line.endColor = new Color(1f, 1f / timeHeld, 1f / timeHeld, 0.6f);

            //slow down player
            movementController.movementSpeed = movementController.normalMovementSpeed / (2f + Mathf.Clamp(timeHeld, 0f, 2.5f));

            //stop rotation if the player is aiming with mouse? maybe remove this if
            if (fireHeld)
            {
                movementController.aimingVec = (target.transform.position - transform.position).normalized;
            }
        }
    }
}
