using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Multiplayer.GameControls
{
    public class PlayerMovementController : NetworkBehaviour
    {
        public float normalMovementSpeed = 5f;
        public float movementSpeed = 5f;
        public float turnSpeed = 5f;
        [SerializeField] private CharacterController controller = null;

        private Vector2 prevInput;

        private GameControls controls;

        public Vector3 aimingVec = Vector3.zero;

        public Transform playerMesh;

        public SphereCollider attackCollider;

        private GameControls Controls
        {
            get
            {
                if (controls != null) { return controls; }
                return controls = new GameControls();
            }
        }

        public override void OnStartAuthority()
        {
            enabled = true;

            Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
            Controls.Player.Move.canceled += ctx => ResetMovement();
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

        private void Update()
        {
            if (knocked)
            {
                controller.Move(knockDir * Time.deltaTime);
            }
            else if (attacking)
            {
                controller.Move(attackDir *  Time.deltaTime);
            }
            else
            {
                Move();
            }
        }

        [Client]
        private void SetMovement(Vector2 movement)
        {
            prevInput = movement;
        }

        [Client]

        private void ResetMovement()
        {
            prevInput = Vector2.zero;
        }

        public Vector3 facingDir;
        [Client]
        private void Move()
        {
            //   Vector3 right = controller.transform.right;
            Vector3 right = controller.transform.right;
            Vector3 forward = controller.transform.forward;
            right.y = 0f;
            forward.y = 0f;

            Vector3 movement = right.normalized * prevInput.x + forward.normalized * prevInput.y;

            controller.Move(movement * movementSpeed * Time.deltaTime);

            if(aimingVec == Vector3.zero)
            {
                if (prevInput != Vector2.zero)
                {
                    facingDir = movement.normalized;
                }
            }
            else
            {
                facingDir = aimingVec;
            }

            if(Vector3.Dot(playerMesh.right, facingDir) < -0.5f)
            {
                playerMesh.right = Vector3.Lerp(playerMesh.right, facingDir, Time.deltaTime * turnSpeed * 2f);
            }
            else
            {
                playerMesh.right = Vector3.Lerp(playerMesh.right, facingDir, Time.deltaTime * turnSpeed);
            }
        }

        private Vector3 attackDir;
        public bool attacking;
        public float attackSpeed = 10f;
        public float attackTimeMultiplier;
        [Client]
        public void Attack(float attackAmount)
        {
           attackDir = facingDir * attackSpeed * attackAmount;
           StartCoroutine(doAttack(attackAmount * attackTimeMultiplier));
        }

        private IEnumerator doAttack(float attackTime)
        {
            attacking = true;
            attackCollider.enabled = true;
            yield return new WaitForSeconds(attackTime);
            attacking = false;
            attackCollider.enabled = false;

        }

        public bool knocked;
        private Vector3 knockDir;
        public float knockTimeMultiplier;
        [Client]
        public void Knockback(Vector3 hitFrom)
        {
            Debug.Log("KNOCKED");
            knockDir = transform.position - hitFrom;
            knockDir = new Vector3(knockDir.x, 0f, knockDir.z).normalized;
            
            StartCoroutine(doKnockback(1f*knockTimeMultiplier));
        }

        private IEnumerator doKnockback(float knockTime)
        {
            knocked = true;
            yield return new WaitForSeconds(knockTime);
            knocked = false;
        }

    }
}
