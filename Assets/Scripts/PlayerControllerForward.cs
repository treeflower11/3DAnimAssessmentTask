using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class PlayerControllerForward : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction sprintAction;
    private CinemachineCamera followPlayer;
    private Rigidbody rb;
    private float moveSpeed = 5f;
    private float sprintBoost = 1.5f;
    private Animator anim;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        followPlayer = GetComponentInChildren<CinemachineCamera>();
        if (followPlayer)
        {
            followPlayer.enabled = true;
            followPlayer.Prioritize();
        }
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        float moveX = moveInput.x;

        if (sprintAction.inProgress)
        {
            moveX *= sprintBoost;
        }

        if (moveInput.x > 0)
        {
            transform.Translate(0, 0, moveX * Time.deltaTime * moveSpeed);
            anim.SetFloat("WalkSpeed", moveX);
        }
        
        anim.SetBool("IsWalking", moveInput.x > 0);
    }
}
