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
        private float currentSpeed=0f;
        public float turnSpeed = 5f;
        [SerializeField] private CharacterController controller = null;

        public Vector2 prevInput;

        private GameControls controls;

        public Vector3 aimingVec = Vector3.zero;

        public Transform playerMesh;

        public SphereCollider attackCollider;

        private NetworkGamePlayer gamePlayer;

        private int playerNum;

        public bool bounceBack;

        public bool inverted=false;

        private PlayerSoundManager soundMan;

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
            Controls.Player.Pause.performed += ctx => ShowPauseScreen();

            soundMan = GetComponent<PlayerSoundManager>();
            
            gamePlayer = GetComponent<NetworkGamePlayer>();
            playerNum = gamePlayer.playerNum;
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

        int currentState;
        private void Update()
        {
            if (falling)
            {
                Fall();
                currentState = 3;
            }
            else if (knocked)
            {
                controller.Move(knockDir * knockPowerMultiplier* Time.deltaTime);
                currentState = 2;
            }
            else if (attacking)
            {
                CheckAttackZone(this.transform, facingDir);
                controller.Move(attackDir * Time.deltaTime);     
            playerMesh.eulerAngles = new Vector3(0f, Mathf.LerpAngle(playerMesh.eulerAngles.y, Vector3.SignedAngle(Vector3.right, facingDir, Vector3.up), Time.deltaTime * turnSpeed *2f), 0f);

                currentState = 1;
            }
            else
            {
                Move();
                currentState = 0;
            }

            if (gamePlayer.currentState != currentState)
            {
                gamePlayer.SetPlayerState(currentState);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Pit"))
            {
                Debug.Log("Pitted");
                soundMan.playFall();
                targetPitCentre = other.transform;
                falling = true;

            }
            else if (other.CompareTag("Pickup"))
            {
                Debug.Log("Picked");
                soundMan.playPickup();
                gamePlayer.pickUpsCurrentlyHeld++;

                gameObject.GetComponent<PlayerDebuffManager>().addDebuff(Random.Range(0,4), gamePlayer.playerNum);

                Destroy(other.gameObject);
            }
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag("Wall"))
            {
                if (knocked)
                {
                    knockDir = Vector3.Reflect(knockDir, hit.normal);
                }
                else if (attacking)
                {
                    attackDir = Vector3.Reflect(attackDir, hit.normal);
                    facingDir = attackDir.normalized;
                }
            }

            if(bounceBack && hit.gameObject.CompareTag("Player"))
            {
                attackDir = Vector3.Reflect(attackDir, hit.normal);
            }
        }


        [Client]
        private void SetMovement(Vector2 movement)
        {
            Debug.Log("CONTROLLINGGG");

            if (inverted)
            {
                prevInput = movement * -1f;
            }
            else
            {
                prevInput = movement;
            }
        }

        [Client]

        private void ResetMovement()
        {
            prevInput = Vector2.zero;
        }

        public Vector3 facingDir;
        private Vector3 prevMovement;
        [Client]
        private void Move()
        {
            //   Vector3 right = controller.transform.right;
            Vector3 right = controller.transform.right;
            Vector3 forward = controller.transform.forward;
            right.y = 0f;
            forward.y = 0f;  


            if (prevInput == Vector2.zero)
            {
                controller.Move(prevMovement * Decelerate() * Time.deltaTime);
            }
            else
            {
                prevMovement = right.normalized * prevInput.x + forward.normalized * prevInput.y;
                controller.Move(prevMovement * Accelerate() * Time.deltaTime);
            }            

            if(aimingVec == Vector3.zero)
            {
                if (prevInput != Vector2.zero)
                {
                    facingDir = prevMovement.normalized;
                }
            }
            else
            {
                facingDir = aimingVec;
            }

            playerMesh.eulerAngles = new Vector3(0f, Mathf.LerpAngle(playerMesh.eulerAngles.y, Vector3.SignedAngle(Vector3.right, facingDir, Vector3.up), Time.deltaTime * turnSpeed), 0f);
        }

        [Header("ACCELERATION")]
        public float accelerationSpeed;
        public float Accelerate()
        {
            currentSpeed = Mathf.Min(currentSpeed + (accelerationSpeed * Time.deltaTime),movementSpeed);
            return currentSpeed;
        }

        [Header("DECELERATION")]
        public float decelerationSpeed;
        public float Decelerate()
        {
            currentSpeed = Mathf.Max(currentSpeed - (decelerationSpeed * Time.deltaTime), 0f);
            return currentSpeed;
        }

        [Header("FALLING")]

        public bool falling;
        public Transform targetPitCentre;
        public float fallSpeed;
        public void Fall()
        {
            //controller.detectCollisions = false;
            //this.gameObject.layer = 10;
            controller.Move((targetPitCentre.position - transform.position).normalized * fallSpeed * Time.deltaTime);
        }

        [Header("ATTACKING")]

        public bool attacking;
        public float attackSpeed = 10f;
        public float attackTimeMultiplier;
        public float overlapRadius;
        public float overlapOffset;
        private Vector3 attackDir;

        public float minAttackDistance=1f;
        public void Attack(float attackAmount)
        {
           attackDir = (facingDir * attackSpeed * attackAmount) +(facingDir * minAttackDistance);
           StartCoroutine(doAttack(attackAmount * attackTimeMultiplier));
        }

        private IEnumerator doAttack(float attackTime)
        {
            attacking = true;
            yield return new WaitForSeconds(attackTime);
            attacking = false;
        }


        [Command]
        public void CheckAttackZone(Transform attacker, Vector3 attackVector)
        {
            Collider[] hitColliders = Physics.OverlapSphere(attacker.position + (attackVector.normalized*overlapOffset),overlapRadius);
            foreach(Collider col in hitColliders)
            {
                if (!col.CompareTag("Player") || col.transform == attacker)
                {
                    break;
                }

                col.GetComponent<PlayerMovementController>().Knockback(attacker.position, attackVector.magnitude);
            }
        }

        [Header("KNOCKBACK")]

        public bool knocked;
        private Vector3 knockDir;
        public float knockPowerMultiplier;
        public float knockTimeMultiplier;
        [ClientRpc]
        public void Knockback(Vector3 hitFrom, float power)
        {
            Debug.Log("KNOCKED");
            soundMan.playHit();
            knockDir = transform.position - hitFrom;
            knockDir = new Vector3(knockDir.x, 0f, knockDir.z).normalized * power;
            
            StartCoroutine(doKnockback(knockTimeMultiplier));
        }

        private IEnumerator doKnockback(float knockTime)
        {
            knocked = true;
            yield return new WaitForSeconds(knockTime);
            knocked = false;
        }

        public GameObject pauseScreen;
        private void ShowPauseScreen()
        {
            pauseScreen.SetActive(!pauseScreen.activeSelf);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (facingDir != Vector3.zero)
            {
                Gizmos.DrawWireSphere(transform.position + facingDir.normalized * overlapOffset, overlapRadius);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position + Vector3.right * overlapOffset, overlapRadius);
            }
        }
    }
}
