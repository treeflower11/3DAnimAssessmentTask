
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbingController : MonoBehaviour
{
    private InputAction climbAction;
    private Rigidbody rb;
    private float moveSpeed = 0.25f;
    private Animator anim;
    [SerializeField] private GameObject rope;
    [SerializeField] private GameObject keyIndicatorPrefab;
    private float handIKWeight = 1;
    private float footIKWeight = 1;
    private SceneDirector sceneDirector;
    private KeyCode[] keys = {KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z};
    private Coroutine keyInputCoroutine = null;
    private KeyCode currentKey;
    private float maxDuration = 5;
    private GameObject keyIndicator;
    
    void Awake()
    {
        // climbAction = InputSystem.actions.FindAction("Interact");
        // climbAction = new InputAction(binding: "<Keyboard>/anyKey");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        sceneDirector = GameObject.Find("SceneDirector")?.GetComponent<SceneDirector>();
    }

    void Update()
    {
        keyInputCoroutine ??= StartCoroutine(KeyInputCoroutine());
        // Climb();
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

    protected void OnEnable()
    {
        Keyboard.current.onTextInput += OnTextInput;
    }

    protected void OnDisable()
    {
        Keyboard.current.onTextInput -= OnTextInput;
    }

    private void OnTextInput(char c)
    {
        if (ConvertToLower(c).Equals(ConvertToLower(currentKey.ToString())))
        {
            Debug.Log("skill check complete!!");
            Climb();
            EndCoroutine();
        }
    }

    private void Climb()
    {
        // if (Input.GetKeyDown(currentKey))
        // {
        //     Debug.Log("skill check complete!");
        //     EndCoroutine();
        // }
        // if (climbAction.inProgress)
        // {
        //     transform.Translate(0, Time.deltaTime * moveSpeed, 0);
        //     anim.SetBool("IsClimbing", true);
        //     footIKWeight = 0;
        //     handIKWeight = 0.4f;
        // }
        // else
        // {
        //     anim.SetBool("IsClimbing", false);
        //     footIKWeight = 1;
        //     handIKWeight = 1;
        // }
    }

    private IEnumerator KeyInputCoroutine()
    {
        currentKey = GetRandomKey();
        CreateKeyIndicator(currentKey);
        float duration = 0;
        while (duration < maxDuration)
        {
            duration += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        DestroyKeyIndicator();
        EndCoroutine();
    }

    private KeyCode GetRandomKey()
    {
        return keys[Random.Range(0, keys.Length)];
    }

    private void CreateKeyIndicator(KeyCode KeyCode)
    {
        Debug.Log(currentKey.ToString());
    }

    private void DestroyKeyIndicator()
    {
        
    }

    private void EndCoroutine()
    {
        StopCoroutine(keyInputCoroutine);
        keyInputCoroutine = null;
    }

    private string ConvertToLower(string text)
    {
        return text.ToLower();
    }

    private string ConvertToLower(char text)
    {
        return text.ToString().ToLower();
    }
}
