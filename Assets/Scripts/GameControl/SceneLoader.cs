using StorkStudios.CoreNest;
using UnityEngine;
using UnityEngine.SceneManagement;

using SceneEnum = StorkStudios.CoreNest.Scene;
using SceneStruct = UnityEngine.SceneManagement.Scene;

public class SceneLoader : PersistentSingleton<SceneLoader>
{
    public SceneEnum CurrentScene => currentScene;

    [SerializeField]
    [ReadOnly]
    private SceneEnum currentScene;

    public bool flag = false;

    private void Start()
    {
        currentScene = (SceneEnum)SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadScene(SceneEnum scene)
    {
        currentScene = scene;
        SceneManager.LoadScene(scene.GetBuildIndex(), LoadSceneMode.Single);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(currentScene.GetBuildIndex(), LoadSceneMode.Single);
    }
}
