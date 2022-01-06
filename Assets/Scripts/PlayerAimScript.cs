using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace Multiplayer.GameControls
{
    public class PlayerAimScript : NetworkBehaviour
    {
        public Transform target;

        private Camera cam;

        private GameControls controls;

        public Vector3 pointerOffset;

        private bool simpleFireHeld = false;

        private bool fireHeld=false;
        private bool freshFire = false;
        
        public float maxTimeHeld = 2f;

        public float aimingCameraOffset;
        public float camMoveSpeed;

        public CameraShakeScript cameraShakeScript;

        public float slowDown;


        #region shittyNewCursourMaybe
        public RectTransform cursor;
        private Image cursorImage;


        public Transform arrowTransform;
        public SpriteRenderer arrow;
        public float arrowTurnSpeed = 5f;
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
                    Cursor.visible = false;

                    cursorImage.enabled = false;
                    arrow.enabled = true;
                    fireHeld = true;
                    freshFire = true;
                    timeHeld = 0f;
                }
                else
                {
                    cursorImage.enabled = true;
                    arrow.enabled = false;
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
                    arrow.enabled = true;
                    simpleFireHeld = true;
                    freshFire = true;
                    timeHeld = 0f;
                }
                else
                {
                    cursorImage.enabled = true;
                    arrow.enabled = false;
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
                Aim(movementController.facingDir);
            }
            else if (fireHeld)
            {
                Aim(pointerOffset);
            }
            else if(freshFire)
            {
                freshFire = false;
                movementController.aimingVec = Vector3.zero;
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

        public float arrowBounceTime;
        public float arrowBouncePower;
        private void Aim(Vector3 offset)
        {
            //count time held
            timeHeld += Time.deltaTime;
            timeHeld = Mathf.Clamp(timeHeld, 0f, maxTimeHeld);
            //find distance to draw line based on anlge and time held
            //target.transform.position = transform.position + new Vector3(offset.x, 0f, offset.z).normalized * Mathf.Clamp(timeHeld * camMoveSpeed, 0f, aimingCameraOffset);
            target.transform.position = Vector3.Lerp(target.transform.position, transform.position + new Vector3(offset.x, 0f, offset.z).normalized * aimingCameraOffset, Time.deltaTime* camMoveSpeed);

            //slow down player
            movementController.movementSpeed = movementController.normalMovementSpeed / (slowDown*(2f + Mathf.Clamp(timeHeld, 0f, maxTimeHeld)));

            arrowTransform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(arrowTransform.eulerAngles.y, Vector3.SignedAngle(Vector3.right, offset, Vector3.up), Time.deltaTime * arrowTurnSpeed), 0f);

            if (timeHeld == maxTimeHeld)
            {
                float scaleNum = Mathf.Max(Mathf.Sin(Time.frameCount / arrowBounceTime) * arrowBouncePower, 0f);
                arrow.transform.localScale = Vector3.one + new Vector3(scaleNum, scaleNum, scaleNum);
            }
            else
            {
                arrow.transform.localScale = (Vector3.one * 0.35f) + (Vector3.one * 0.65f * (timeHeld / maxTimeHeld));
            }

            //stop rotation if the player is aiming with mouse? maybe remove this if
            if (fireHeld)
            {
                movementController.aimingVec = (target.transform.position - transform.position).normalized;
            }
        }
    }
}
