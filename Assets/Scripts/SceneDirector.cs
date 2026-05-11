using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    public enum Scene {A1Interactive1, A1Interactive2}
    public static Scene currentScene {get; private set;} = Scene.A1Interactive1;
    private int subSceneIndex = 0;
    [SerializeField] private GameObject[] subScenes;
    private PlayableDirector currentlyPlayingTimeline;

    public static void GoToNextScene()
    {
        currentScene++;
        SceneManager.LoadSceneAsync((int)currentScene);
    }

    void Start()
    {
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
            Skybox mainCamSkybox = Camera.main.GetComponent<Skybox>();
            mainCamSkybox.material = subScenes[subSceneIndex].GetComponentInChildren<Skybox>().material;
            PlayableDirector timeline = subScenes[subSceneIndex].GetComponentInChildren<PlayableDirector>();
            if (timeline)
            {
                currentlyPlayingTimeline = timeline;
                timeline.Play();
                timeline.stopped += TimelineEnds;
            }
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
                Skybox mainCamSkybox = Camera.main.GetComponent<Skybox>();
                mainCamSkybox.material = subScenes[subSceneIndex].GetComponentInChildren<Skybox>().material;
            }
        }
    }
}
