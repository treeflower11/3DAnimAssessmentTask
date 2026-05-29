using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    public enum Scene {A1Part1, A1Part2, A2, A3}
    public static Scene currentScene {get; private set;} = Scene.A1Part1;
    private int subSceneIndex = 0;
    [SerializeField] private GameObject[] subScenes;
    private PlayableDirector currentlyPlayingTimeline;
    [SerializeField] private GameObject cutsceneCoverPrefab;
    private WaitForSeconds waitForSeconds = new(2);

    private void GoToNextScene()
    {
        currentScene++;
        if ((int)currentScene >= SceneManager.sceneCountInBuildSettings) return;
        if (cutsceneCoverPrefab)
        {
            GameObject cutscene = Instantiate(cutsceneCoverPrefab);
            DontDestroyOnLoad(cutscene);
        }
        SceneManager.LoadSceneAsync((int)currentScene);
    }

    void Start()
    {
        StartCoroutine(LoadNewScene());
    }

    void Awake()
    {
        currentScene = (Scene)SceneManager.GetActiveScene().buildIndex;
    }

    private IEnumerator LoadNewScene()
    {
        GameObject cutscene = GameObject.FindWithTag("Cutscene");
        if (cutscene)
        {
            yield return waitForSeconds;
            Destroy(cutscene);
        }
        SetupSubScenes();  
    }

    public void GoToNextSubScene()
    {
        subScenes[subSceneIndex].SetActive(false);
        subSceneIndex++;
        if (subSceneIndex > subScenes.Length - 1)
        {
            GoToNextScene();
        }
        else
        {
            subScenes[subSceneIndex].SetActive(true);
            SetupSkybox();
            CheckForTimeline();
        }
    }

    private void TimelineEnds(PlayableDirector timeline)
    {
        if (currentlyPlayingTimeline == timeline)
        {
            currentlyPlayingTimeline.stopped -= TimelineEnds;
            GoToNextSubScene();
        }
    }

    private void SetupSubScenes()
    {
        if (subScenes.Length == 0) return;

        for (int i = 0; i < subScenes.Length; i++)
        {
            subScenes[i].SetActive(i == subSceneIndex);
            if (i == subSceneIndex)
            {
                SetupSkybox();
                CheckForTimeline();
            }
        }
    }

    private void CheckForTimeline()
    {
        PlayableDirector timeline = subScenes[subSceneIndex].GetComponentInChildren<PlayableDirector>();
        if (timeline)
        {
            currentlyPlayingTimeline = timeline;
            timeline.Play();
            timeline.stopped += TimelineEnds;
        }
    }

    private void SetupSkybox()
    {
        Skybox mainCamSkybox = Camera.main.GetComponent<Skybox>();
        if (mainCamSkybox)
        {
            mainCamSkybox.material = subScenes[subSceneIndex].GetComponentInChildren<Skybox>()?.material;
        }
    }
}
