using Unity.Hierarchy;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerController : MonoBehaviour
{
    public enum EDirection { up, down, left, right }
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float friction = 2f;
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private float jumpStrength = 2f;
    [SerializeField] private float gravityStrength = 2f;
    [SerializeField] private float maxFallSpeed = 10f;
    [SerializeField] private float footStepDistance = 2f;
    [SerializeField] private AudioClip footStepAudio;
    [SerializeField] private int hitcastFidelity = 4;
    [SerializeField] private EDirection currentGravity = EDirection.down;
    private Vector3 velocity;
    private Vector3 gravityDirection;
    private Vector3 moveDirection;
    private Vector3 lastFootstepPosition;
    private Vector3 currentFootstepPosition;
    private CharacterController controller;
    private AudioSource audio;
    private BoxCollider boxCollider;
    private InputSystem_Actions inputActions;
    private InputAction move;
    private InputAction jump;
    private bool wantJump;
    private bool canJump;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        audio = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider>();
        inputActions = new InputSystem_Actions();
        lastFootstepPosition = transform.position;
        ChangeGravityDirection(currentGravity);
    }

    private void OnEnable()
    {
        move = inputActions.Player.Move;
        move.Enable();
        jump = inputActions.Player.Jump;
        jump.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
    }

    private void Update()
    {
        wantJump = jump.IsPressed();
    }

    private void FixedUpdate()
    {
        canJump = controller.isGrounded;
        if(wantJump && canJump)
        {
            Jump();
        }
        Gravity();
        if (controller.isGrounded)
        {
            CheckFootstep();
        }
        float input = move.ReadValue<float>();
        Acceleration(input);
        Friction(input);
        Move();
        controller.Move(velocity * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        velocity += -gravityDirection * jumpStrength;
    }

    private void Gravity()
    {
        velocity += gravityDirection * gravityStrength * Time.fixedDeltaTime;
        float fallSpeed = Vector3.Dot(velocity, gravityDirection);
        if(fallSpeed > maxFallSpeed || fallSpeed < -maxFallSpeed)
        {
            float difference = fallSpeed - maxFallSpeed;
            velocity -= gravityDirection * difference;
        }
    }

    private void CheckFootstep()
    {
        if ((lastFootstepPosition - transform.position).magnitude > footStepDistance)
        {
            lastFootstepPosition = transform.position;
            audio.PlayOneShot(footStepAudio);
        }
        currentFootstepPosition = transform.position;
    }

    private void Acceleration(float input)
    {
        velocity += moveDirection * input * acceleration * Time.fixedDeltaTime;
    }

    private void Friction(float input)
    {
        float moveSpeed = Vector3.Dot(velocity, moveDirection);
        if (moveSpeed > maxSpeed * Mathf.Abs(input) || moveSpeed < -maxSpeed * Mathf.Abs(input))
        {
            float difference = moveSpeed - maxSpeed * input;
            velocity -= Vector3.ClampMagnitude(moveDirection * difference, friction);
        }
    }
    private void Move()
    {
        if(velocity.z > 0)
        {

        }
        if (velocity.z < 0)
        {

        }
        if (velocity.y > 0)
        {

        }
        if (velocity.y < 0)
        {

        }
    }

    public void ChangeGravityDirection(EDirection direction)
    {
        currentGravity = direction;
        switch (direction)
        {
            case EDirection.up:
                gravityDirection = new Vector3(0, 1, 0);
                moveDirection = new Vector3(-1, 0, 0);
                break;
            case EDirection.down:
                gravityDirection = new Vector3(0, -1, 0);
                moveDirection = new Vector3(1, 0, 0);
                break;
            case EDirection.left:
                gravityDirection = new Vector3(-1, 0, 0);
                moveDirection = new Vector3(0, -1, 0);
                break;
            case EDirection.right:
                gravityDirection = new Vector3(1, 0, 0);
                moveDirection = new Vector3(0, 1, 0);
                break;
        }
    }
}
