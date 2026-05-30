using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject givePaperInstructions;
    [SerializeField] private GameObject moveInstructions;
    private InputAction moveAction;
    private SceneDirector sceneDirector;
    private InputAction sprintAction;
    private InputAction giveAction;
    private Rigidbody rb;
    private float moveSpeed = 5f;
    private float turnSpeed = 150f;
    private float sprintBoost = 1.5f;
    private Animator anim;
    private Coroutine givePaper;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        giveAction = InputSystem.actions.FindAction("Interact");
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        sceneDirector = GameObject.Find("SceneDirector").GetComponent<SceneDirector>();
        ShowMoveInstructions();
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
        else if (other.CompareTag("PaperRange"))
        {
            ShowGiveInstructions();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PaperRange"))
        {
            ShowMoveInstructions();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PaperRange") && giveAction.triggered & isActiveAndEnabled)
        {
            givePaper ??= StartCoroutine(GivePaper());
        }
    }

    private IEnumerator GivePaper()
    {
        if (sceneDirector) sceneDirector.GoToNextSubScene();
        yield return null;
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

    private void ShowMoveInstructions()
    {
        if (moveInstructions) moveInstructions.SetActive(true);
        if (givePaperInstructions) givePaperInstructions.SetActive(false);
    }

    private void ShowGiveInstructions()
    {
        if (moveInstructions) moveInstructions.SetActive(false);
        if (givePaperInstructions) givePaperInstructions.SetActive(true);
    }
}
