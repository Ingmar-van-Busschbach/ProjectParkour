using Unity.Hierarchy;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
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
    [SerializeField] private EDirection currentGravity = EDirection.down;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private GameObject modelObject;
    [SerializeField] private float rotationSpeed = 1;
    private Quaternion modelRotation;
    private Vector3 spawnLocation;
    private Vector3 velocity;
    private Vector3 gravityDirection;
    private Vector3 moveDirection;
    private Vector3 lastFootstepPosition;
    private CharacterController controller;
    private AudioSource audio;
    private InputSystem_Actions inputActions;
    private InputAction move;
    private InputAction jump;
    private bool wantJump;
    private bool canJump;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        audio = GetComponent<AudioSource>();
        inputActions = new InputSystem_Actions();
        lastFootstepPosition = transform.position;
        spawnLocation = transform.position;
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
        //modelObject.transform.rotation = Quaternion.Lerp(transform.rotation, modelRotation, Time.deltaTime * rotationSpeed);
    }

    private void FixedUpdate()
    {
        canJump = IsGrounded();
        if (wantJump && canJump)
        {
            Jump();
        }
        if (!canJump)
        {
            Gravity();
        }
        if (controller.isGrounded)
        {
            CheckFootstep();
        }
        float input = move.ReadValue<float>();
        Acceleration(input);
        Friction(input);
        controller.Move(velocity * Time.fixedDeltaTime);
    }

    private bool IsGrounded()
    {
        Vector3 startposition = transform.position;
        startposition += gravityDirection * (controller.radius + controller.skinWidth);
        return Physics.Raycast(startposition, gravityDirection, 0.01f);
    }

    private void Jump()
    {
        Vector3 verticalVelocity = Vector3.Dot(velocity, gravityDirection) * gravityDirection;
        velocity -= verticalVelocity;
        velocity -= gravityDirection * jumpStrength;
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
            float difference = maxSpeed * input - moveSpeed;
            velocity += Vector3.ClampMagnitude(moveDirection * difference, friction * Time.fixedDeltaTime);
        }
    }
    //private bool CollisionCheck()
    //{
    //    return Physics.Raycast(transform.position + gravityDirection * (boxCollider.size.x / 2), gravityDirection, Vector3.Dot(velocity, gravityDirection) * Time.fixedDeltaTime, collisionMask);
    //}

    public void ChangeGravityDirection(EDirection direction)
    {
        currentGravity = direction;
        switch (direction)
        {
            case EDirection.up:
                gravityDirection = new Vector3(0, 1, 0);
                moveDirection = new Vector3(1, 0, 0);
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
        modelRotation = Quaternion.LookRotation(Vector3.forward, -gravityDirection);
    }

    public void Respawn()
    {
        controller.enabled = false;
        transform.position = spawnLocation;
        controller.enabled = true;
    }
}
