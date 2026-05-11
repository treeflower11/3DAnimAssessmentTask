using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbingController : MonoBehaviour
{
    private InputAction climbAction;
    private Rigidbody rb;
    private float moveSpeed = 0.25f;
    private Animator anim;
    [SerializeField] private GameObject rope;
    private float handIKWeight = 1;
    private float footIKWeight = 1;
    private SceneDirector sceneDirector;
    
    void Start()
    {
        climbAction = InputSystem.actions.FindAction("Interact");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        sceneDirector = GameObject.Find("SceneDirector")?.GetComponent<SceneDirector>();
    }

    void Update()
    {
        Climb();
    }

    void OnAnimatorIK(int layerIndex){
        if (rope)
        {
            Vector3 ropeHandPosition = new Vector3(rope.transform.position.x, transform.position.y + 1.25f, rope.transform.position.z);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, ropeHandPosition);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIKWeight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, ropeHandPosition);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, handIKWeight);

            Vector3 ropeFootPosition = ropeHandPosition + Vector3.down * 1.1f;
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, ropeFootPosition);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, footIKWeight);
            anim.SetIKPosition(AvatarIKGoal.RightFoot, ropeFootPosition);
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, footIKWeight);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NextScene"))
        {
            sceneDirector.GoToNextSubScene();
        }
    }

    private void Climb()
    {
        if (climbAction.inProgress)
        {
            transform.Translate(0, Time.deltaTime * moveSpeed, 0);
            anim.SetBool("IsClimbing", true);
            footIKWeight = 0;
            handIKWeight = 0.4f;
        }
        else
        {
            anim.SetBool("IsClimbing", false);
            footIKWeight = 1;
            handIKWeight = 1;
        }
    }
}
