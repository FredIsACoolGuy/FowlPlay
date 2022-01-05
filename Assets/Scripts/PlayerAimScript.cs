using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace Multiplayer.GameControls
{
    public class PlayerAimScript : NetworkBehaviour
    {
        private LineRenderer line;

        public Transform target;

        private Camera cam;

        private GameControls controls;

        public Vector3 pointerOffset;

        private bool simpleFireHeld = false;

        private bool fireHeld=false;
        private bool freshFire = false;
        public float lineSpeed;
        public float maxLineLength = 5f;
        public float maxTimeHeld = 2f;

        public CameraShakeScript cameraShakeScript;

        public float slowDown;


        #region shittyNewCursourmaybe
        public RectTransform cursor;
        private Image cursorImage; 
        public RectTransform canvas;
        public float cursorOffset = 10f;
        #endregion




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
            cam = Camera.main;
            enabled = true;
            cursorImage = cursor.GetComponent<Image>();
            Controls.Player.Look.performed += ctx => MoveTarget(ctx.ReadValue<Vector2>());
            Controls.Player.Fire.performed += ctx => Fire(ctx.ReadValue<float>());
            Controls.Player.SimpleFire.performed += ctx => SimpleFire(ctx.ReadValue<float>());

           // Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //cursor.gameObject.SetActive(false);
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


        [ClientCallback]
        private void OnDestroy()
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
                    cursorImage.enabled = false;
                    fireHeld = true;
                    freshFire = true;
                    timeHeld = 0f;
                }
                else
                {
                    cursorImage.enabled = true;

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
                    cursorImage.enabled = false;

                    simpleFireHeld = true;
                    freshFire = true;
                    timeHeld = 0f;
                }
                else
                {
                    cursorImage.enabled = true;

                    simpleFireHeld = false;
                }
            }
        }

        private Vector2 storedMouse;
        private void MoveTarget(Vector2 mousePos)
        {
            //GameObject.Find("Main Camera").GetComponent<Camera>();

            // pointerOffset = mousePos - new Vector2(cam.WorldToScreenPoint(transform.position).x, cam.WorldToScreenPoint(transform.position).y);
            //if (!(simpleFireHeld || fireHeld))
            //{
                Ray ray = cam.ScreenPointToRay(mousePos);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit))
                {
                    pointerOffset = hit.point - transform.position;

                    cursor.position = cam.WorldToScreenPoint(hit.point);
                    storedMouse = mousePos;
                }
           // }
           // else
           // {
           //     cursor.position = cursor.position + new Vector3(mousePos.x - storedMouse.x, mousePos.y - storedMouse.y, 0f);
           //     storedMouse = mousePos;
           // }
            ///
            ///cursor
            ///
            //if (!simpleFireHeld && !fireHeld)
            //{
            //    Vector3 newCursorPos = new Vector3();
            //    RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas, mousePos, cam, out newCursorPos);
            //    cursor.anchoredPosition = new Vector2(mousePos.x - Screen.width / 2, mousePos.y - Screen.height / 2) / 2.2f;
                
            ////    Ray ray = cam.ScreenPointToRay(cursor.position);
            ////    RaycastHit hit = new RaycastHit();
            ////    if (Physics.Raycast(ray, out hit))
            ////    {
            ////        pointerOffset = hit.point - transform.position;
            ////    }
            //}
           // else
           // {
            //    cursor.anchoredPosition = cam.WorldToScreenPoint(transform.position + pointerOffset);
            //}
        }

        [Client]
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

                cameraShakeScript.CameraShake(0.6f, 50, timeHeld * 0.25f);
                cameraShakeScript.cameraZoom = 1.2f;
            }
            else
            {
                target.position = Vector3.Lerp(target.position, transform.position, Time.deltaTime);
            }


        }

        private void Aim(Vector3 offset)
        {
            //count time held
            timeHeld += Time.deltaTime;
            //find distance to draw line based on anlge and time held
            target.transform.position = transform.position + new Vector3(offset.x, 0f, offset.z).normalized * Mathf.Clamp(timeHeld * lineSpeed, 0f, maxLineLength);

            //position line
            line.SetPosition(0, transform.position);
            line.SetPosition(1, target.transform.position);

            //change colour of line
            line.startColor = new Color(1f, 1f / (timeHeld * 2f), 1f / (timeHeld * 2f), 1f);
            line.endColor = new Color(1f, 1f / timeHeld, 1f / timeHeld, 0.6f);

            //slow down player
            movementController.movementSpeed = movementController.normalMovementSpeed / (slowDown*(2f + Mathf.Clamp(timeHeld, 0f, maxTimeHeld)));

            //stop rotation if the player is aiming with mouse? maybe remove this if
            if (fireHeld)
            {
                movementController.aimingVec = (target.transform.position - transform.position).normalized;
            }
        }
    }
}
