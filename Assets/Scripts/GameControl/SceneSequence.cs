using UnityEngine;
using StorkStudios.CoreNest;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Scene Sequence", fileName = "SceneSequence")]
public class SceneSequence : ScriptableObjectSingleton<SceneSequence>
{
    [SerializeField]
    private List<Scene> scenes;

    public Scene? GetNextScene(Scene scene)
    {
        int index = scenes.IndexOf(scene);
        if (-1 < index && index < scenes.Count - 1)
        {
            return scenes[index + 1];
        }
        return null;
    }
}
