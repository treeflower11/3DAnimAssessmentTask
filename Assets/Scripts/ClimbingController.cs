using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClimbingController : MonoBehaviour
{
    private float moveSpeed = 0.25f;
    private Animator anim;
    [SerializeField] private GameObject rope;
    [SerializeField] private GameObject[] keyIndicatorPrefabs;
    // [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject canvas;
    private float handIKWeight = 1;
    private float footIKWeight = 1;
    private SceneDirector sceneDirector;
    private KeyCode[] keys = {KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z};
    private Coroutine keyInputCoroutine = null;
    private KeyCode currentKey;
    private const float maxDuration = 4;
    private const float totalClimbDistance = 0.25f;
    private const float totalFallDistance = totalClimbDistance/2;
    private GameObject keyIndicator;
    private RectTransform keyIndicatorTransform;
    private float keyIndicatorBaseScalar = 1;
    private Vector2 xRange = new(0, 500);
    private Vector2 yRange = new(-300, 300);
    private List<System.Func<IEnumerator>> moveQueue = new();
    private Coroutine moveCoroutine = null;
    private float initialHeight;
    private Slider progressBar;
    private Transform checkpoint;
    private float totalDistanceToCheckpoint = 0;
    private float heightOffset = 1.5f;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
        sceneDirector = GameObject.Find("SceneDirector")?.GetComponent<SceneDirector>();
        StopMovingAnimation();
        initialHeight = transform.position.y;
        progressBar = GameObject.FindWithTag("ClimbSlider")?.GetComponent<Slider>();
        checkpoint = GameObject.FindWithTag("NextScene")?.transform;
        if (checkpoint) totalDistanceToCheckpoint = checkpoint.position.y - heightOffset - transform.position.y;
    }

    void Update()
    {
        keyInputCoroutine ??= StartCoroutine(KeyInputCoroutine());
        UpdateProgressBar();
        MoveMimic();
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

    private void UpdateProgressBar()
    {
        if (!checkpoint && !progressBar) return;

        progressBar.value = 1 - ((checkpoint.position.y - heightOffset - transform.position.y) / totalDistanceToCheckpoint);
    }

    private void OnTextInput(char c)
    {
        if (ConvertToUpper(c).Equals(ConvertToUpper(currentKey)))
        {
            AddToClimbQueue();
            EndKeyCoroutine();
        }
        else
        {
            StartFalling();
            EndKeyCoroutine();
        }
    }

    private void MoveMimic()
    {
        if (moveCoroutine == null)
        {
            if (moveQueue.Count != 0)
            {
                moveCoroutine = StartCoroutine(moveQueue[0]());
                moveQueue.RemoveAt(0);
            }
        }
    }

    private void AddToClimbQueue()
    {
        moveQueue.Add(Climb);
    }

    private void StartFalling()
    {
        EndMoveCoroutine();
        moveQueue.Clear();
        moveCoroutine = StartCoroutine(Fall());
    }

    private IEnumerator Fall()
    {
        float distance = 0;
        while (distance < totalFallDistance && transform.position.y > initialHeight)
        {
            distance += Time.deltaTime * moveSpeed;
            transform.Translate(0, Time.deltaTime * -moveSpeed, 0);
            StopMovingAnimation();
            yield return new WaitForEndOfFrame();
        }
        EndMoveCoroutine();
    }

    private IEnumerator Climb()
    {
        float distance = 0;
        while (distance < totalClimbDistance)
        {
            distance += Time.deltaTime * moveSpeed;
            transform.Translate(0, Time.deltaTime * moveSpeed, 0);
            anim.SetBool("IsClimbing", true);
            footIKWeight = 0;
            handIKWeight = 0.4f;
            yield return new WaitForEndOfFrame();
        }
        EndMoveCoroutine();
    }

    private IEnumerator KeyInputCoroutine()
    {
        currentKey = GetRandomKey();
        CreateKeyIndicator(currentKey);
        float duration = 0;
        while (duration < maxDuration)
        {
            duration += Time.deltaTime;
            ShrinkKeyIndicator((maxDuration - duration) / maxDuration);
            yield return new WaitForEndOfFrame();
        }
        StartFalling();
        EndKeyCoroutine();
    }

    private KeyCode GetRandomKey()
    {
        return keys[Random.Range(0, keys.Length)];
    }

    private void CreateKeyIndicator(KeyCode key)
    {
        GameObject keyIndicatorPrefab = GetRandomKeyIndicator();
        if (canvas && keyIndicatorPrefab)
        {
            keyIndicator = Instantiate(keyIndicatorPrefab, canvas.transform);
            keyIndicatorTransform = keyIndicator.GetComponent<RectTransform>();
            if (keyIndicatorTransform) 
            {
                keyIndicatorBaseScalar = keyIndicatorTransform.localScale.x;
                keyIndicatorTransform.localPosition = new Vector2(GetRandomX(), GetRandomY());
            }
            
            if (keyIndicator)
            {
                TMP_Text tmp = keyIndicator.GetComponentInChildren<TMP_Text>();
                tmp.text = ConvertToUpper(key);
            }
        }
    }

    private void DestroyKeyIndicator()
    {
        if (keyIndicator)
        {
            Destroy(keyIndicator);
            keyIndicator = null;
            keyIndicatorTransform = null;
        }
    }

    private void ShrinkKeyIndicator(float scalar)
    {
        if (!keyIndicatorTransform) return;
        
        keyIndicatorTransform.localScale = Vector3.one * scalar * keyIndicatorBaseScalar;
    }

    private void EndKeyCoroutine()
    {
        DestroyKeyIndicator();
        if (keyInputCoroutine != null)
        {
            StopCoroutine(keyInputCoroutine);
            keyInputCoroutine = null;
        }
    }

    private void EndMoveCoroutine()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        StopMovingAnimation();
    }

    private void StopMovingAnimation()
    {
        anim.SetBool("IsClimbing", false);
        footIKWeight = 1;
        handIKWeight = 1;
    }

    private string ConvertToUpper(KeyCode text)
    {
        return text.ToString().ToUpper();
    }

    private string ConvertToUpper(char text)
    {
        return text.ToString().ToUpper();
    }

    private float GetRandomX()
    {
        return Random.Range(xRange.x, xRange.y);
    }

    private float GetRandomY()
    {
        return Random.Range(yRange.x, yRange.y);
    }

    private GameObject GetRandomKeyIndicator()
    {
        if (keyIndicatorPrefabs.Length == 0) return null;
        return keyIndicatorPrefabs[Random.Range(0, keyIndicatorPrefabs.Length)];
    }
}
