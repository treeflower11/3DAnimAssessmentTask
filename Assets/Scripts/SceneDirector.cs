using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    public enum Scene {A1Interactive1, A1Interactive2}
    public static Scene currentScene {get; private set;} = Scene.A1Interactive1;

    public static void GoToNextScene()
    {
        currentScene++;
        SceneManager.LoadSceneAsync((int)currentScene);
    }
}
