using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    public enum Scene {A1Interactive1, A1Interactive2}
    public static Scene currentScene {get; private set;} = Scene.A1Interactive1;
    private int subSceneIndex = 0;
    [SerializeField] private GameObject[] subScenes;

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
        }
    }

    private void SetupSubScenes()
    {
        if (subScenes.Length == 0) return;

        for (int i = 0; i < subScenes.Length; i++)
        {
            subScenes[i].SetActive(i == subSceneIndex);
        }
    }
}
