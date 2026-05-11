using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class PlayerController : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction sprintAction;
    private CinemachineCamera followPlayer;
    private Rigidbody rb;
    private float moveSpeed = 5f;
    private float turnSpeed = 150f;
    private float sprintBoost = 1.5f;
    private Animator anim;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        followPlayer = GetComponentInChildren<CinemachineCamera>();
        followPlayer.enabled = true;
        followPlayer.Prioritize();
    }

    void Update()
    {
        Move();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Button"))
        {
            PlayableDirector sequence = other.GetComponentInChildren<PlayableDirector>();
            sequence.Play();
        }
    }

    private void Move()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        float forwardInput = moveInput.y;
        float rightInput = moveInput.x;

        if (sprintAction.inProgress)
        {
            forwardInput *= sprintBoost;
        }

        transform.Translate(0, 0, forwardInput * Time.deltaTime * moveSpeed);
        transform.Rotate(0, turnSpeed * rightInput * Time.deltaTime, 0);
        anim.SetBool("IsWalking", forwardInput != 0);
        anim.SetFloat("WalkSpeed", forwardInput);
    }
}
